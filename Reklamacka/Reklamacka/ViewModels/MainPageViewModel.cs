using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.CommunityToolkit.UI.Views;
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

		private SideMenuState menuState;
		/// <summary>
		/// Stav, ve kterem je vizualizovano bocni menu
		/// </summary>
		public SideMenuState MenuState
		{
			get => menuState;
			set
			{
				menuState = value;
				OnPropertyChanged(nameof(MenuState));
			}
		}

		public Command AddShop { get; set; }
		public Command AddNewBill { get; set; }         //!< Command pro tlacitko pridani nove uctenky
		public Command EditBill { get; set; }           //!< Command pro tlacitko editace uctenky z listu
		public Command DeleteBill { get; set; }         //!< Command pro smazani vybrane polozky
		public Command ItemTapped { get; set; }         //!< Command pro reseni eventu kliknuti na polozku
		public Command SortingPagePush { get; set; }    //!< Command pro presun na stranku trideni
		public Command ReverseBills { get; set; }       //!< Command pro reverzi poradi uctenek
		public Command ViewImage { get; set; }			//!< Command k tlacitku pro zobrazeni obrazku
		public Command MenuButtonClicked { get; set; }	//!< Command k tlacitku otevirajici bocni menu
		public Command ArchivePage { get; set; }		//!< Command pro otevreni okna s proslymi uctenky

		private bool isFromOldest = false;

		// konstruktor
		public MainPageViewModel(INavigation Navigation)
		{
			AddShop = new Command(async () =>
			{
				await Navigation.PushAsync(new AddStorePage(Navigation));
			});
			// vytvoreni commandu pridani nove uctenky - presun na stranku editace
			AddNewBill = new Command(async () =>
			{
				await Navigation.PushAsync(new BillEditPage(Navigation, null));
			});

			// vytvoreni commandu editace nove uctenky - presun na stranku editace existujici uctenky
			EditBill = new Command(async () =>
			{
				await Navigation.PushAsync(new BillEditPage(Navigation, SelectedBill));
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
				if (SelectedBill.IsSelected)
					SelectedBill.IsSelected = false;
				else
					SelectedBill.IsSelected = true;
			});

			// vytvoreni commandu pro presun na stranku tridici polozky uctenek
			SortingPagePush = new Command(async () =>
			{
				await Navigation.PushAsync(new SortingPage());
			});

			ReverseBills = new Command(async() =>
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

			ViewImage = new Command(async () =>
			{
				await Navigation.PushAsync(new ViewImagePage(SelectedBill));
			});

			MenuButtonClicked = new Command(() =>
			{
				MenuState = SideMenuState.LeftMenuShown;
			});
		}


		/// <summary>
		/// Funkce volana pri eventu kliknuti na item v listu
		/// </summary>
		public async void OnAppearing()
		{
			// pri nacteni hlavni stranky se kolekce naplni novymi daty z databaze
			// TODO - mozna lze resit nejak lepe
			Bills = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllFromOldestAsync());
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
