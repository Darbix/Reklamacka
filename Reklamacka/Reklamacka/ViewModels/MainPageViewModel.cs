using Plugin.LocalNotification;
using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	public class MainPageViewModel : INotifyPropertyChanged
	{

		private List<ItemBill> lofBills;
		public List<ItemBill> LofBills							//!< List of all bills in database
		{
			get => lofBills;
			set
			{
				lofBills = value;
				OnPropertyChanged(nameof(LofBills));
			}
		}
		private ObservableCollection<ItemBill> observeBill;
		public ObservableCollection<ItemBill> ObserveBill		//!< List of displayable bills (after applying basic filters)
		{
			get => observeBill;
			set
			{
				observeBill = value;
				OnPropertyChanged(nameof(ObserveBill));
			}
		}

		private ItemBill selectedItem;
		public ItemBill SelectedItem
		{
			get => selectedItem;
			set
			{
				if (selectedItem != null)
					selectedItem.IsSelected = false;
				selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
			}
		}

		private string nameToSearch = "";
		/// <summary>
		/// String s hodnotou pro vyhledani polozky podle nazvu
		/// </summary>
		public string NameToSearch
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

		private bool isEmpty = true;
		public bool IsEmpty
		{
			get => isEmpty;
			set { isEmpty = value; OnPropertyChanged(nameof(IsEmpty));
			}
		}

		public Command AddShop { get; set; }
		public Command AddNewBill { get; set; }         //!< Command pro tlacitko pridani nove uctenky
		public Command EditBill { get; set; }           //!< Command pro tlacitko editace uctenky z listu
		public Command DeleteBill { get; set; }         //!< Command pro smazani vybrane polozky
		public Command ItemTapped { get; set; }         //!< Command pro reseni eventu kliknuti na polozku
		public Command SortingPagePush { get; set; }    //!< Command pro presun na stranku trideni
		public Command ReverseBills { get; set; }       //!< Command pro reverzi poradi uctenek
		public Command ViewImage { get; set; }          //!< Command k tlacitku pro zobrazeni obrazku
		public Command MenuButtonClicked { get; set; }  //!< Command k tlacitku otevirajici bocni menu
		public Command SettingsPagePush { get; set; }
		public Command SearchBill { get; set; }
		public Command ArchivePage { get; set; }        //!< Command pro otevreni okna s proslymi uctenkami

		private bool isFromOldest = false;

		// konstruktor
		public MainPageViewModel(INavigation Navigation)
		{
			// init
			if (LofBills == null)
				LofBills = new List<ItemBill>();
			if (ObserveBill == null)
				ObserveBill = new ObservableCollection<ItemBill>();

			AddShop = new Command(async () =>
			{
				await Navigation.PushAsync(new AddStorePage(Navigation));
			});

			ArchivePage = new Command(async () =>
			{
				await Navigation.PushAsync(new ArchivePage(Navigation));
			});
			// vytvoreni commandu pridani nove uctenky - presun na stranku editace
			AddNewBill = new Command(async () =>
			{
				await Navigation.PushAsync(new BillEditPage(Navigation, null));
			});

			// vytvoreni commandu editace nove uctenky - presun na stranku editace existujici uctenky
			EditBill = new Command(async () =>
			{
				await Navigation.PushAsync(new BillEditPage(Navigation, SelectedItem.BillItem));
			});

			// inicializace tlacitka ke smazani polozky
			DeleteBill = new Command(async () =>
			{
				await BillsDB.DeleteItemAsync(SelectedItem.BillItem);
				// obnoveni listu polozek
				OnAppearing();
			});

			// Po kliknuti je nastaven priznak IsSelected polozky na true
			ItemTapped = new Command((e) =>
			{
				if (SelectedItem != null)
					SelectedItem.IsSelected = !SelectedItem.IsSelected;
			});

			// vytvoreni commandu pro presun na stranku tridici polozky uctenek
			SortingPagePush = new Command(async () =>
			{
				await Navigation.PushAsync(new SortingPage());
			});

			SettingsPagePush = new Command(async () =>
			{
				await Navigation.PushAsync(new UserSettingsPage());
			});

			ReverseBills = new Command(() =>
			{
				// reordering list
				if (isFromOldest)
					LofBills = LofBills.OrderBy(item => item.BillItem.ExpirationDate).ToList();
				else
					LofBills = LofBills.OrderByDescending(item => item.BillItem.ExpirationDate).ToList();

				isFromOldest = !isFromOldest;
				// updating displayable collection
				ObserveBill = new ObservableCollection<ItemBill>(LofBills.Where(bill => bill.IsVisible));
			});

			ViewImage = new Command(async () =>
			{
				await Navigation.PushAsync(new ViewImagePage(SelectedItem.BillItem));
			});

			MenuButtonClicked = new Command(() =>
			{
				MenuState = SideMenuState.LeftMenuShown;
			});

			SearchBill = new Command(async () =>
			{
				if (!string.IsNullOrWhiteSpace(NameToSearch))
					LofBills.ForEach(bill => bill.IsVisible = bill.BillItem.ProductName.Contains(NameToSearch));
				else
					LofBills.ForEach(bill => bill.IsVisible = true);

				ObserveBill = new ObservableCollection<ItemBill>(LofBills.Where(bill => bill.IsVisible));
				await System.Threading.Tasks.Task.CompletedTask;
			});

		}

		private async Task<bool> ShowNotif()
		{
			var notification = new NotificationRequest
			{
				ReturningData = "Notifications enabled",
				Title = "A product bill will be expired in 3 days",
				Schedule = { NotifyTime = DateTime.Now.AddSeconds(2) }
			};

			return await NotificationCenter.Current.Show(notification);
		}

		private static bool notified;

		/// <summary>
		/// Funkce volana pri eventu kliknuti na item v listu
		/// </summary>
		public async void OnAppearing()
		{
			MenuState = SideMenuState.MainViewShown;
			SelectedItem = null;

			// update list of bills
			LofBills.Clear();
			if (isFromOldest)
				BillsDB.GetAllFromOldestAsync().Result.ToList().ForEach(bill => LofBills.Add(new ItemBill(bill)));
			else
				BillsDB.GetAllItemsAsync().Result.ToList().ForEach(bill => LofBills.Add(new ItemBill(bill)));

			// sort displayable bills
			ObserveBill = new ObservableCollection<ItemBill>(LofBills.Where(bill => bill.IsVisible));
			await Task.CompletedTask;

			if (ObserveBill.Count == 0)
				IsEmpty = true;
			else IsEmpty = false;

			// expiration notification
			if (!notified)
			{
				try
				{
					ObserveBill.Where(x => x.BillItem.ExpirationDate <= DateTime.Today.AddDays(3)).First();
					_ = ShowNotif();
					// max 1x when app is runnung
					notified = true;
				}
				catch { }
			}
		}

		// OnPropertyChanged() volat pri pozadavku na projeveni zmeny pri nejake udalosti
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
