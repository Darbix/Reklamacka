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
				await Navigation.PushAsync(new FiltersSettingPage(ListTypes));
			});
		}

		public async void OnAppearing()
		{
			if (Bills == null)
			{
				Bills = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllAsync());
				return;
			}

			ObservableCollection<Bill> list = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllAsync());

			//types
			List<ProductTypes> listTypes = new List<ProductTypes>();
			for (int i = 0; i < ListTypes.Count; i++)
			{
				if (ListTypes[i].IsChecked)
				{
					listTypes.Add(ListTypes[i].Type);
				}
			}

			Bills = new ObservableCollection<Bill>(list.OrderBy(x => x.ExpirationDate).Where(x => listTypes.Contains(x.ProductType)).ToList());

			//Bills = list;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
