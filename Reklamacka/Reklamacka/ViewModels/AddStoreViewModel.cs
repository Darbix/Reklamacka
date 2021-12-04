using Xamarin.Forms;
using Reklamacka.Models;
using static Reklamacka.BaseModel;
using System.Collections.ObjectModel;
using System.Linq;
using Reklamacka.Pages;

namespace Reklamacka.ViewModels
{
	public class AddStoreViewModel : ContentView
	{
		public Command SaveStore { get; set; }			//!< Command for saving new store to the database
		public Command DeleteAll { get; set; }			//!< Command to clear store database; TODO: temporary solution
		public Command PushBrowserPage { get; set; }
		public Command DeleteStore { get; set; }
		public Web Website = new Web();

		private Store storeInstance;
		public string StoreName { get; set; }			//!< New store's name
		public string StoreLink { get; set; }			//!< New store's weblink
		public string Email { get; set; }				//!< New store's contact email
		public string PhoneNumber { get; set; }			//!< New store's contact number

		public ObservableCollection<string> ShopNameList { get; private set; } = LofStoreNames;

		private Store shop;                             //!< Chosen shop
		public string ShopName                          //!< Shop's name to display
		{
			get => shop != null ? shop.Name : string.Empty;
			set
			{
				if (value != null && !value.Equals(""))
				{
					shop = StoreDB.GetStoreAsync(value).Result;

					if (shop != null)
					{
						StoreName = shop.Name;
						StoreLink = shop.Link;
						Email = shop.Email;
						PhoneNumber = shop.PhoneNumber;

						OnPropertyChanged(nameof(StoreName));
						OnPropertyChanged(nameof(StoreLink));
						OnPropertyChanged(nameof(Email));
						OnPropertyChanged(nameof(PhoneNumber));
					}
				}
			}
		}

		public AddStoreViewModel(INavigation navig)
		{
			SaveStore = new Command(async () =>
			{
				if (string.IsNullOrWhiteSpace(StoreName))
				{
					await App.Current.MainPage.DisplayAlert("Error", "ShopName cannot be empty!", "Cancel");
					return;
				}

				if (PhoneNumber != null)
				{
					if (PhoneNumber.Length > 9)
					{
						await App.Current.MainPage.DisplayAlert("Error", "Phone number is too long (max 9 digits)", "Cancel");
						return;
					}
					else if (int.TryParse(PhoneNumber, out int res) == false)
					{
						await App.Current.MainPage.DisplayAlert("Error", "Phone number must include only numbers", "Cancel");
						return;
					}
				}

				// create a new instance of the store
				storeInstance = new Store
				{
					Name = StoreName,
					Link = StoreLink,
					Email = Email,
					PhoneNumber = PhoneNumber
				};

				if (shop != null)
				{
					storeInstance.ID = shop.ID;
				}
				else
				{
					// search for existance of store in database
					var found = StoreDB.GetStoreAsync(storeInstance.Name).Result;

					// assign an ID if found, otherwise add new store's name to list of store names
					if (found != null)
						storeInstance.ID = found.ID;
				}

				// save to database
				await StoreDB.SaveStoreInstanceAsync(storeInstance);
				LofStoreNames = new ObservableCollection<string>(StoreDB.GetStoresListAlphabetOrderAsync().Result.Select(shop => shop.Name));
				
				await navig.PopAsync();
			});

			DeleteAll = new Command(async () =>
			{
				if (!await App.Current.MainPage.DisplayAlert("Delete all", "Would you like to delete all stores?", "Yes", "No"))
					return;

				await StoreDB.ClearDatabase();
				// clear store records of every bill in the database
				BillsDB.GetAllItemsAsync().Result.ForEach(bill => bill.ShopID = 0);
				await navig.PopAsync();
			});

			DeleteStore = new Command(async () =>
			{
				if (!(await App.Current.MainPage.DisplayAlert("Delete store", "Would you like to delete the store?", "Yes", "No")) || shop == null)
					return;

				// clear store record of every bill it had this instance
				BillsDB.GetAllItemsAsync().Result.Where(bill => bill.ShopID == shop.ID).ToList().ForEach(bill => bill.ShopID = 0);

				await StoreDB.DeleteStoreAsync(shop);
				LofStoreNames = new ObservableCollection<string>(StoreDB.GetStoresListAlphabetOrderAsync().Result.Select(shop => shop.Name));
				await navig.PopAsync();
			});

			PushBrowserPage = new Command(async () =>
			{
				Website.Link = StoreLink;
				await navig.PushAsync(new BrowserPage(Website));
			});
		}

		public void OnAppearing()
		{
			StoreLink = Website.Link;
			OnPropertyChanged(nameof(StoreLink));
		}
	}
}