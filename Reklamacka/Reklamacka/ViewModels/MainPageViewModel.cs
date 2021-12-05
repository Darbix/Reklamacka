/**
 * @brief View model of MainPage
 * 
 * @detail Display bills that are still active. User can search by name, sort by date,
 *			created new bill, edit old one or delete one.
 * 
 * @file MainPageViewModel.cs
 * @author Do Hung (xdohun00), Kedra David (xkedra00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
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
using System.Globalization;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	public class MainPageViewModel : INotifyPropertyChanged
	{

		private List<ItemBill> lofBills;

		/// <summary>
		/// List of all bills in database
		/// </summary>
		public List<ItemBill> LofBills
		{
			get => lofBills;
			set
			{
				lofBills = value;
				OnPropertyChanged(nameof(LofBills));
			}
		}
		private ObservableCollection<ItemBill> observeBill;

		/// <summary>
		/// List of displayable bills (after applying basic filters)
		/// </summary>
		public ObservableCollection<ItemBill> ObserveBill
		{
			get => observeBill;
			set
			{
				observeBill = value;
				OnPropertyChanged(nameof(ObserveBill));
			}
		}

		private ItemBill selectedItem;
		
		/// <summary>
		/// Selected bill item
		/// </summary>
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

		/// <summary>
		/// Status of bills whether the list is empty or not
		/// </summary>
		public bool IsEmpty
		{
			get => isEmpty;
			set 
			{ 
				isEmpty = value; OnPropertyChanged(nameof(IsEmpty));
			}
		}

		/// <summary>
		/// Open AddStorePage
		/// </summary>
		public Command AddShop { get; set; }

		/// <summary>
		/// Create new bill
		/// </summary>
		public Command AddNewBill { get; set; }

		/// <summary>
		/// Edit selected bill
		/// </summary>
		public Command EditBill { get; set; }

		/// <summary>
		/// Delete selected bill
		/// </summary>
		public Command DeleteBill { get; set; }

		/// <summary>
		/// Event handle of item tapped
		/// </summary>
		public Command ItemTapped { get; set; }

		/// <summary>
		/// Open SortingPage
		/// </summary>
		public Command SortingPagePush { get; set; }
		
		/// <summary>
		/// Reverse bill order
		/// </summary>
		public Command ReverseBills { get; set; }
		
		/// <summary>
		/// Show bills saved image
		/// </summary>
		public Command ViewImage { get; set; }

		/// <summary>
		/// Show side menu
		/// </summary>
		public Command MenuButtonClicked { get; set; }

		/// <summary>
		/// Open SettingsPage
		/// </summary>
		public Command SettingsPagePush { get; set; }

		/// <summary>
		/// Search bill by name
		/// </summary>
		public Command SearchBill { get; set; }

		/// <summary>
		/// Open ArchivePage
		/// </summary>
		public Command ArchivePage { get; set; }

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
				// reset
				LofBills.ForEach(bill => bill.IsVisible = true);

				// looks for name that contains SearchSubstring while ignoring cases
				// source: https://stackoverflow.com/a/15464440
				if (!string.IsNullOrWhiteSpace(NameToSearch))
					LofBills.Where(item => new CultureInfo("cs-CZ", false).CompareInfo.IndexOf(item.BillItem.ProductName, NameToSearch, CompareOptions.IgnoreCase) < 0).ToList().ForEach(item => item.IsVisible = false);

				ObserveBill = new ObservableCollection<ItemBill>(LofBills.Where(bill => bill.IsVisible));
				await Task.CompletedTask;
			});

		}

		/// <summary>
		/// Generates notification
		/// </summary>
		/// <returns>Returns true if notification was created successfully</returns>
		private async Task<bool> ShowNotif()
		{
			// created by Kedra David
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
					UserSettingsViewModel settings = new UserSettingsViewModel();
					if (settings.IsOnNotifications) 
					{
						ObserveBill.Where(x => x.BillItem.ExpirationDate <= DateTime.Today.AddDays(3)).First();
						_ = ShowNotif();
						// max 1x when app is runnung
						notified = true;
					}
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
