using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Reklamacka.ViewModels
{
	public class MainPageViewModel : INotifyPropertyChanged
	{

		private ObservableCollection<Bill> bills;
		/// <summary>
		/// Kolekce uctenek-produktu naplnujici ListView v MainPage
		/// </summary>
		public ObservableCollection<Bill> Bills
		{
			get => bills;
			set
			{
				bills = value;
				OnPropertyChanged(nameof(Bills));
			}
		}

		private Bill selectedBill;
		/// <summary>
		/// Aktualne zvoleny item uctenky z listu
		/// </summary>
		public Bill SelectedBill
		{
			get => selectedBill;
			set
			{
				if (selectedBill != null)
					selectedBill.IsSelected = false;
				selectedBill = value;
				OnPropertyChanged(nameof(SelectedBill));
			}
		}

		private String nameToSearch;
		/// <summary>
		/// String s hodnotou pro vyhledani polozky podle nazvu
		/// </summary>
		public String NameToSearch
		{
			get => nameToSearch;
			set
			{
				nameToSearch = value;
				OnPropertyChanged(nameof(NameToSearch));
			}
		}

		public Command AddNewBill { get; set; }         //!< Command pro tlacitko pridani nove uctenky
		public Command EditBill { get; set; }           //!< Command pro tlacitko editace uctenky z listu
		public Command DeleteBill { get; set; }         //!< Command pro smazani vybrane polozky
		public Command ItemTapped { get; set; }         //!< Command pro reseni eventu kliknuti na polozku
		public Command SortingPagePush { get; set; }    //!< Command pro presun na stranku trideni
		public Command ReverseBills { get; set; }       //!< Command pro reverzi poradi uctenek

		private bool isFromOldest = false;

		// konstruktor
		public MainPageViewModel(INavigation Navigation)
		{
			// vytvoreni commandu pridani nove uctenky - presun na stranku editace
			AddNewBill = new Command(async () =>
			{
				await Navigation.PushAsync(new BillEditPage(null));
			});

			// vytvoreni commandu editace nove uctenky - presun na stranku editace existujici uctenky
			EditBill = new Command(async () =>
			{
				await Navigation.PushAsync(new BillEditPage(SelectedBill));
			});

			// inicializace tlacitka ke smazani polozky
			DeleteBill = new Command(async () =>
			{
				await BaseModel.BillsDB.DeleteItemAsync(SelectedBill);
				// obnoveni listu polozek
				OnAppearing();
			});

			// Po kliknuti je nastaven priznak IsSelected polozky na true
			ItemTapped = new Command((e) =>
			{
				if (SelectedBill == null)
					return;

				SelectedBill.IsSelected = true;
			});

			// vytvoreni commandu pro presun na stranku tridici polozky uctenek
			SortingPagePush = new Command(async () =>
			{
				await Navigation.PushAsync(new SortingPage());
			});

			ReverseBills = new Command(async () =>
			{
				if (isFromOldest)
				{
					Bills = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllItemsAsync());
					isFromOldest = false;
				}
				else
				{
					Bills = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllFromOldestAsync());
					isFromOldest = true;
				}
			});
		}


		/// <summary>
		/// Funkce volana pri eventu kliknuti na item v listu
		/// </summary>
		public async void OnAppearing()
		{
			// pri nacteni hlavni stranky se kolekce naplni novymi daty z databaze
			// TODO - mozna lze resit nejak lepe
			if (isFromOldest)
				Bills = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllFromOldestAsync());
			else
				Bills = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllItemsAsync());

			SelectedBill = null;
		}

		/// <summary>
		/// Vyhleda pozadovany objekt Bill v seznamu na zaklade nazvu
		/// </summary>
		/// <returns> Object Bill, jehoz ProductName bylo vyhledano </returns>
		public Bill GetBillByName()
		{
			return (Bill)Bills.Where(x => x.ProductName.Equals(NameToSearch)).FirstOrDefault();
		}

		// OnPropertyChanged() volat pri pozadavku na projeveni zmeny pri nejake udalosti
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
