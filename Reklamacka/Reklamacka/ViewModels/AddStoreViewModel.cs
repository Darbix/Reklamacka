/**
 * @brief View model of add store page
 * 
 * @detail Allow user to create or edit store instance
 * 
 * @file AddStoreViewModel.cs
 * @author Do Hung (xdohun00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using Xamarin.Forms;
using Reklamacka.Models;
using static Reklamacka.BaseModel;
using System.Collections.ObjectModel;
using System.Linq;
using Reklamacka.Pages;
using System;
using Xamarin.Essentials;

namespace Reklamacka.ViewModels
{
	public class AddStoreViewModel : ContentView
	{
		/// <summary>
		/// Command for saving new store to the database
		/// </summary>
		public Command SaveStore { get; set; }

		/// <summary>
		/// Command to clear store database
		/// </summary>
		public Command DeleteAll { get; set; }

		/// <summary>
		/// Command to open store's website in default browser
		/// </summary>
		public Command PushBrowserPage { get; set; }

		/// <summary>
		/// Command to delete selected store instance from database
		/// </summary>
		public Command DeleteStore { get; set; }

		/// <summary>
		/// Command to deselect the store
		/// </summary>
		public Command Deselect { get; set; }

		/// <summary>
		/// Store's name 
		/// </summary>
		public string StoreName { get; set; }

		/// <summary>
		/// Store's website
		/// </summary>
		public string StoreLink { get; set; }

		/// <summary>
		/// Store's contact email
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Store's contact phone number
		/// </summary>
		public string PhoneNumber { get; set; }

		/// <summary>
		/// List of stored store names
		/// </summary>
		public ObservableCollection<string> ShopNameList { get; private set; } = LofStoreNames;

		/// <summary>
		/// Selected store
		/// </summary>
		private Store shop;

		/// <summary>
		/// Store name to display
		/// </summary>
		public string ShopName
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
					}
				}
				OnPropertyChanged(nameof(StoreName));
				OnPropertyChanged(nameof(StoreLink));
				OnPropertyChanged(nameof(Email));
				OnPropertyChanged(nameof(PhoneNumber));
			}
		}

		public AddStoreViewModel(INavigation navig)
		{
			SaveStore = new Command(async () =>
			{
				// store name check
				if (string.IsNullOrWhiteSpace(StoreName))
				{
					await App.Current.MainPage.DisplayAlert("Error", "Shop name cannot be empty!", "Cancel");
					return;
				}

				// phone number validation
				if (!string.IsNullOrWhiteSpace(PhoneNumber))
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

				// link validation
				if (!string.IsNullOrWhiteSpace(StoreLink))
				{
					// adds http:// if it's not defined
					var url = new UriBuilder(StoreLink).Uri;
					StoreLink = url.ToString();
				}

				// create a new instance of the store
				var storeInstance = new Store
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
				if (!string.IsNullOrWhiteSpace(StoreLink))
				{
					// add http:// to link if StoreLink doesn't specify it
					var url = new UriBuilder(StoreLink).Uri;
					StoreLink = url.ToString();
					await Launcher.TryOpenAsync(StoreLink);
				}
			});

			Deselect = new Command(() =>
			{
				shop = null;
				StoreName = null;
				StoreLink = null;
				Email = null;
				PhoneNumber = null;

				ShopName = null;
				OnPropertyChanged(nameof(ShopName));
			});
		}

		public void OnAppearing()
		{
			OnPropertyChanged(nameof(StoreLink));
		}
	}
}