using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	public class SortingPageViewModel : INotifyPropertyChanged
	{
		public static ObservableCollection<FilterItem> FilterLofTypes { get; set; }								//!< Static collection of selected product types; used in Filter mode
		public static ObservableCollection<FilterItem> FilterLofStoreNames { get; set; }						//!< Static collection of selected store names; used in Filter mode
		public static List<string> LofFilteredShopNames { get; set; } = new List<string>();						//!< Bills of goods that were purchased from these stores will be displayed
		public static List<ProductTypes> LofFilteredProductTypes { get; set; }									//!< Goods of selected categories will be displayed
		private List<ItemBill> bills;
		public List<ItemBill> Bills						//!< List of bills with additional properties
		{
			get => bills;
			set
			{
				bills = value;
				OnPropertyChanged(nameof(Bills));
			}
		}

		private ObservableCollection<ItemBill> observeBills;
		public ObservableCollection<ItemBill> ObserveBills		//!< Displayable bills
		{
			get => observeBills;
			set
			{
				observeBills = value;
				OnPropertyChanged(nameof(ObserveBills));
			}
		}

		public Command SelectBill { get; set; }
		public Command DeleteSelected { get; set; }
		public Command PushFiltersPage { get; set; }

		public SortingPageViewModel(INavigation Navigation)
		{
			// init collections of bills
            var lofBills = BillsDB.GetAllAsync().Result;
			if (Bills == null)
                Bills = new List<ItemBill>();

			if (lofBills != null)
				lofBills.ForEach(bill => Bills.Add(new ItemBill(bill)));

			// reset filters
			FilterLofStoreNames = null;
			FilterLofTypes = null;
			LofFilteredProductTypes = new List<ProductTypes>();
			LofFilteredShopNames = new List<string>();

			SelectBill = new Command((s) =>
			{
				if (!(s is ItemBill item))
					return;

				item.IsSelected = !item.IsSelected;
				Console.WriteLine("Bill {0} is {1}", item.BillItem.ProductName, item.IsSelected ? "selected" : "not selected");
			});

			DeleteSelected = new Command(async () =>
			{
				bool yesDelete = await App.Current.MainPage.DisplayAlert("Delete items", "are you sure you want to delete these items?", "Yes", "No");
				if (!yesDelete)
					return;

				// delete selected items
				Bills.Where(item => item.IsSelected).ToList().ForEach(item => { BillsDB.DeleteItemAsync(item.BillItem); Bills.Remove(item); });
				OnAppearing();
			});

			PushFiltersPage = new Command(async () =>
			{
				await Navigation.PushAsync(new FiltersSettingPage());
				//	new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllAsync()), ListTypes, ListShopNames));
			});
		}

		public async void OnAppearing()
		{
			// supporting flag
			bool valSet = false;

			// reset values
            Bills.ForEach(item => item.IsVisible = false);
            Bills.ForEach(item => item.IsSelected = false);

			// filter applied
			// filter by product types
			if (LofFilteredProductTypes != null && LofFilteredProductTypes.Count() > 0)
			{
				// make visible items that are the same category as selected types
				Bills.Where(item => LofFilteredProductTypes.Any(type => item.BillItem.ProductType == type)).ToList().ForEach(item => item.IsVisible = true);
				valSet = true;
			}
			// filter by store name
			if (LofFilteredShopNames != null && LofFilteredShopNames.Count() > 0)
			{
				// make visible items that were purchase from selected stores
				Bills.Where(item => LofFilteredShopNames.Any(name => item.StoreName == name)).ToList().ForEach(item => item.IsVisible = true);
				valSet = true;
			}

			// if all filters are empty (all in default mode)
			// set all items as visible
			if (!valSet)
				Bills.ForEach(item => item.IsVisible = true);

			// save only visible item into observable collection of bills
			ObserveBills = new ObservableCollection<ItemBill>(Bills.Where(x => x.IsVisible));
			await System.Threading.Tasks.Task.CompletedTask;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
