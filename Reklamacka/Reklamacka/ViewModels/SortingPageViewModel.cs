/**
 * @brief View model of SortingPage
 * 
 * @file SortingPageViewModel.cs
 * @author Do Hung (xdohun00), Kedra David (xkedra00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	public class SortingPageViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Static collection of selected product categories; used in Filter mode
		/// </summary>
		public static ObservableCollection<FilterItem> FilterLofTypes { get; set; }

		/// <summary>
		/// Static collection of selected store names; used in Filter mode
		/// </summary>
		public static ObservableCollection<FilterItem> FilterLofStoreNames { get; set; }

		/// <summary>
		/// Bills of goods that were purchased from selected stores will be displayed
		/// </summary>
		public static List<string> LofFilteredShopNames { get; set; }

		/// <summary>
		/// Goods of selected categories will be displayed
		/// </summary>
		public static List<ProductTypes> LofFilteredProductTypes { get; set; }

		/// <summary>
		/// Filter mode
		/// </summary>
		public static bool Intersection { get; set; }

		private List<ItemBill> bills;

		/// <summary>
		/// List of all bills in database
		/// </summary>
		public List<ItemBill> Bills
		{
			get => bills;
			set
			{
				bills = value;
				OnPropertyChanged(nameof(Bills));
			}
		}

		private ObservableCollection<ItemBill> observeBills;

		/// <summary>
		/// List of displayable bills
		/// </summary>
		public ObservableCollection<ItemBill> ObserveBills
		{
			get => observeBills;
			set
			{
				observeBills = value;
				OnPropertyChanged(nameof(ObserveBills));
			}
		}

		private ItemBill selectedItem;

		/// <summary>
		/// Selected bill
		/// </summary>
		public ItemBill SelectedItem
		{
			get => selectedItem;
			set
			{
				selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
			}
		}

		private bool nameByAlpha;

		/// <summary>
		/// Sort by name mode status
		/// </summary>
		public bool NameByAlpha
		{
			get => nameByAlpha;
			set
			{
				nameByAlpha = value;
				OnPropertyChanged(nameof(NameByAlpha));
			}
		}

		private string searchSubstring;

		/// <summary>
		/// String used in name filter
		/// </summary>
		public string SearchSubstring
		{
			get => searchSubstring;
			set
			{
				searchSubstring = value;
				OnPropertyChanged(nameof(SearchSubstring));
			}
		}

		private bool byExpDate;

		/// <summary>
		/// Sort by expiration date mode
		/// </summary>
		public bool ByExpDate
		{
			get => byExpDate;
			set
			{
				byExpDate = value;
				OnPropertyChanged(nameof(ByExpDate));
			}
		}

		/// <summary>
		/// Change ItemBill.IsSelected property
		/// </summary>
		public Command SelectBill { get; set; }

		/// <summary>
		/// Delete selected items
		/// </summary>
		public Command DeleteSelected { get; set; }

		/// <summary>
		/// Open FilterSettingsPage
		/// </summary>
		public Command PushFiltersPage { get; set; }

		/// <summary>
		/// Edit selected (tapped) bill
		/// </summary>
		public Command EditBill { get; set; }

		/// <summary>
		/// Sort visible items by expiration date
		/// </summary>
		public Command SortByExpDate { get; set; }

		/// <summary>
		/// Sort visible items by name
		/// </summary>
		public Command SortByName { get; set; }

		/// <summary>
		/// Filter items by given substring
		/// </summary>
		public Command SearchName { get; set; }

		public SortingPageViewModel(INavigation Navigation)
		{
			// init collections of bills
			var lofBills = BillsDB.GetAllAsync().Result;
			if (Bills == null)
				Bills = new List<ItemBill>();

			if (lofBills != null)
				lofBills.ForEach(bill => Bills.Add(new ItemBill(bill)));

			// reset filters
			Intersection = false;
			FilterLofStoreNames = null;
			FilterLofTypes = null;
			LofFilteredProductTypes = new List<ProductTypes>();
			LofFilteredShopNames = new List<string>();
			ByExpDate = true;
			NameByAlpha = true;

			EditBill = new Command(async (e) =>
			{
				if (!(e is ItemBill item))
					return;
				await Navigation.PushAsync(new BillEditPage(Navigation, item.BillItem));
			});

			SelectBill = new Command((s) =>
			{
				if (!(s is ItemBill item))
					return;

				item.IsSelected = !item.IsSelected;
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
			});

			SortByExpDate = new Command(async () =>
			{
				if (ObserveBills == null || !ObserveBills.Any())
					return;
				if (ByExpDate)
					ObserveBills = new ObservableCollection<ItemBill>(ObserveBills.OrderByDescending(item => item.BillItem.ExpirationDate).ToList());
				else
					ObserveBills = new ObservableCollection<ItemBill>(ObserveBills.OrderBy(item => item.BillItem.ExpirationDate).ToList());
				ByExpDate = !ByExpDate;
				await System.Threading.Tasks.Task.CompletedTask;
			});
			SortByName = new Command(async () =>
			{
				if (ObserveBills == null || !ObserveBills.Any())
					return;
				if (NameByAlpha)
					ObserveBills = new ObservableCollection<ItemBill>(ObserveBills.OrderBy(item => item.BillItem.ProductName).ToList());
				else
					ObserveBills = new ObservableCollection<ItemBill>(ObserveBills.OrderByDescending(item => item.BillItem.ProductName).ToList());
				NameByAlpha = !NameByAlpha;
				await System.Threading.Tasks.Task.CompletedTask;
			});

			SearchName = new Command(async () =>
			{
				// reset filter
				Bills.ForEach(item => item.IsVisible = true);

				// looks for name that contains SearchSubstring while ignoring cases
				// source: https://stackoverflow.com/a/15464440
				if (!string.IsNullOrWhiteSpace(SearchSubstring))
					Bills.Where(item => new CultureInfo("cs-CZ", false).CompareInfo.IndexOf(item.BillItem.ProductName, SearchSubstring, CompareOptions.IgnoreCase) < 0).ToList().ForEach(item => item.IsVisible = false);

				ObserveBills = new ObservableCollection<ItemBill>(Bills.Where(item => item.IsVisible).ToList());
				await System.Threading.Tasks.Task.CompletedTask;
			});
		}

		public async void OnAppearing()
		{
			// supporting flag
			bool valSet = false;

			// reset values
			Bills.ForEach(item =>
			{
				item.IsVisible = false;
				item.IsSelected = false;
				item.UpdateStoreName();
			});
			NameByAlpha = true;
			ByExpDate = true;

			if (Intersection)
			{
				List<ItemBill> validItems = new List<ItemBill>(Bills);
				// filter applied
				// filter by product types
				if (LofFilteredProductTypes != null && LofFilteredProductTypes.Count() > 0)
				{
					// filter out items with different type
					validItems.Where(item => LofFilteredProductTypes.Any(type => item.BillItem.ProductType == type)).ToList().ForEach(item => item.IsVisible = true);
					valSet = true;
					ObserveBills = new ObservableCollection<ItemBill>(validItems.Where(item => item.IsVisible));
				}
				validItems = new List<ItemBill>(ObserveBills);
				validItems.ForEach(item => item.IsVisible = false);

				// filter by store name
				if (LofFilteredShopNames != null && LofFilteredShopNames.Count() > 0)
				{
					// filter out items that were purchased from different store
					validItems.Where(item => LofFilteredShopNames.Any(name => item.StoreName == name)).ToList().ForEach(item => item.IsVisible = true);
					valSet = true;
					ObserveBills = new ObservableCollection<ItemBill>(validItems.Where(item => item.IsVisible));
				}
			}
			else
			{
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
				// save only visible item into observable collection of bills
				ObserveBills = new ObservableCollection<ItemBill>(Bills.Where(x => x.IsVisible));
			}

			// if all filters are empty (all in default mode)
			// set all items as visible
			if (!valSet)
			{
				Bills.ForEach(item => item.IsVisible = true);
				// save only visible item into observable collection of bills
				ObserveBills = new ObservableCollection<ItemBill>(Bills.Where(x => x.IsVisible));
			}
			await System.Threading.Tasks.Task.CompletedTask;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
