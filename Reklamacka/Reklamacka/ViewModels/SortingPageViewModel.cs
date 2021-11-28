using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	public class SortingPageViewModel : INotifyPropertyChanged
	{
		private ObservableCollection<Bill> bills;
		public ObservableCollection<Bill> Bills
		{
			get => bills;
			set
			{
				bills = value;
				OnPropertyChanged(nameof(Bills));
			}
		}

		public Command SelectBill { get; set; }
		public Command DeleteSelected { get; set; }
		public Command PushFiltersPage { get; set; }

		public ObservableCollection<FilterItem> ListTypes { get; set; } = new ObservableCollection<FilterItem>();
		public ObservableCollection<FilterItem> ListUrls { get; set; } = new ObservableCollection<FilterItem>();


		public SortingPageViewModel(INavigation Navigation)
		{
			SelectBill = new Command((s) =>
			{
				if (!(s is Bill bill))
					return;

				if (bill.IsSelected)
					bill.IsSelected = false;
				else
					bill.IsSelected = true;
			});

			DeleteSelected = new Command(async () =>
			{
				var yesDelete = await App.Current.MainPage.DisplayAlert("Delete items", "are you sure you want to delete these items?", "Yes", "No");
				if (!yesDelete)
					return;
				for (int i = 0; i < Bills.Count; i++)
				{
					Bill toDelete = Bills[i];
					if (toDelete.IsSelected)
						await BaseModel.BillsDB.DeleteItemAsync(toDelete);
				}
				OnAppearing();
			});

			PushFiltersPage = new Command(async () =>
			{
				await Navigation.PushAsync(new FiltersSettingPage(
					new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllAsync()), ListTypes, ListUrls));
			});
		}

		public async void OnAppearing()
		{
			if (Bills == null)
			{
				Bills = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllAsync());
				return;
			}

			// Listy obsahujici pouze atributy, ktere maji polozky splnovat
			List<ProductTypes> listTypes = new List<ProductTypes>();
			List<string> listUrls = new List<string>();


			// TODO !!

			// ---TYPY---
			try
			{
				// Vyber vsech, pokud zadny neni vybrany, nebo prave vybranych
				if (ListTypes.First(x => x.IsChecked == true) != null)
				{
					for (int i = 0; i < ListTypes.Count; i++)
					{
						if (ListTypes[i].IsChecked)
						{
							listTypes.Add(ListTypes[i].Type);
						}
					}
				}
			}
			catch
			{
				//neni-li nic vybrane, pridaji se vsechny filtry sekce, jako by byly vybrane
				if (listTypes.Count == 0)
				{
					for (int i = 0; i < ListTypes.Count; i++)
						listTypes.Add(ListTypes[i].Type);
				}
			}

			// ---URL adresy---
			try
			{
				// Pokud nebyl zvolen alespon 1 filter sekce, jsou vybrany vsechny
				if (ListUrls.First(x => x.IsChecked == true) != null)
				{
					for (int i = 0; i < ListUrls.Count; i++)
					{
						if (ListUrls[i].IsChecked)
						{
							listUrls.Add(ListUrls[i].ShopUrl);
						}
					}
				}
			}
			catch
			{
				for (int i = 0; i < ListUrls.Count; i++)
					listUrls.Add(ListUrls[i].ShopUrl);
				listUrls.Add(null);
			}


			// Sortovany list objektu bill
			Collection<Bill> list = new Collection<Bill>(await BaseModel.BillsDB.GetAllAsync());
			Bills = new ObservableCollection<Bill>(list.Where(x => listUrls.Contains(x.ShopUrl) && listTypes.Contains(x.ProductType)).ToList());

		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
