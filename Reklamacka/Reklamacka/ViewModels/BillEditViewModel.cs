using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	// trida pro web k predani jako reference, aby sel web menit z dalsiho okna
	public class Web
	{
		public string Link { get; set; }
	}

	public class BillEditViewModel : INotifyPropertyChanged
	{
		public Command SaveBill { get; set; }			//!< Command to save a new bill
		public Command DeleteBill { get; set; }			//!< Command to delete a selected bill (if active)
		public Command PushBrowserPage { get; set; }	//!< Load a store's website in default browser
		public Command PickPhoto { get; set; }			//!< Open a window to select a bill photo
		public Command CallNumber { get; set; }			//!< Load a phone dialer
		public Command ViewImage { get; set; }			//!< Display a bill photo
		public Bill SelectedBill { get; set; }			//!< Edited bill
		public string ProductName { get; set; }			//!< Goods's name 
		public DateTime PurchaseDate { get; set; }		//!< Date of purchase
		public DateTime ExpirationDate { get; set; }	//!< Date of expire
		public string Notes { get; set; }				//!< Additional notes
		public bool HasImage { get; set; }				//!< Status whether bill contains a photo
		public IList<ProductTypes> BillTypes { get; set; } = LofTypes;	//!< List of product types
		private ProductTypes productType;				//!< Chosen product type
		public ProductTypes ProductType
		{
			get => productType;
			set
			{
				productType = value;
				OnPropertyChanged(nameof(ProductType));
			}
		}

		private ImageSource imgBill;					//!< Chosen photo
		/// <summary>
		/// Zdrojova data obrazku
		/// </summary>
		public ImageSource ImgBill
		{
			get => imgBill;
			set
			{
				imgBill = value;
				OnPropertyChanged(nameof(ImgBill));
			}
		}
		public ObservableCollection<string> ShopNameList { get; private set; } = LofStoreNames;	//!< List of shop names

		private Store shop;								//!< Chosen shop
		public string ShopName							//!< Shop's name to display
		{
			get => shop != null ? shop.Name : string.Empty;
			set
			{
				shop = StoreDB.GetStoreAsync(value).Result;
				Website.Link = shop != null ? shop.Link : string.Empty;
				OnPropertyChanged(nameof(ShopName));
				OnPropertyChanged(nameof(Weblink));
				OnPropertyChanged(nameof(Email));
				OnPropertyChanged(nameof(PhoneNumber));
			}
		}

		public string Weblink							//!< Shop's website to display
		{
			get => shop != null ? shop.Link : string.Empty; set { }
		}
		public Web Website = new Web();

		public string Email								//!< Shop's email to display
		{
			get => shop != null ? shop.Email : string.Empty; set { }
		}

		public string PhoneNumber						//!< Shop's contact number to display
		{
			get => shop != null ? shop.PhoneNumber : string.Empty; set { }
		}

		public BillEditViewModel(INavigation navigation, Bill bill)
		{
			SelectedBill = bill ?? new Bill();

			ProductName = SelectedBill.ProductName;
			PurchaseDate = SelectedBill.PurchaseDate;
			ExpirationDate = SelectedBill.ExpirationDate;
			Notes = SelectedBill.Notes;
			ImgBill = SelectedBill.GetImage();
			ProductType = SelectedBill.ProductType;
			Store selectedShop = StoreDB.GetStoreAsync(SelectedBill.ShopID).Result;
			if (selectedShop != null)
			{
				shop = selectedShop;
				ShopName = selectedShop.Name;
				Weblink = selectedShop.Link;
				Email = selectedShop.Email;
				PhoneNumber = selectedShop.PhoneNumber;
			}

			SaveBill = new Command(async () =>
			{
				if (ProductName == null)
				{
					await App.Current.MainPage.DisplayAlert("Problem", "Product name is needed!", "Ok");
					return;
				}
				SelectedBill.ProductName = ProductName;
				SelectedBill.IsSelected = false;
				SelectedBill.PurchaseDate = PurchaseDate;
				SelectedBill.ExpirationDate = ExpirationDate;
				SelectedBill.Notes = Notes;
				SelectedBill.ProductType = ProductType;
				SelectedBill.HasImage = HasImage;
				if (shop != null)
					SelectedBill.ShopID = shop.ID;
				_ = await BillsDB.SaveItemAsync(SelectedBill);

				_ = await navigation.PopAsync();
			});

			DeleteBill = new Command(async () =>
			{
				_ = await BillsDB.DeleteItemAsync(SelectedBill);
			});

			PickPhoto = new Command(async () =>
			{
				var img = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
				{
					Title = "Pick a bill photo"
				});
				if (img != null)
					HasImage = true;
				else
					return;

				Stream stream = await img.OpenReadAsync();
				// konverze do pole bytu, aby se dal ulozit do databaze
				byte[] buffer = new byte[16 * 1024];
				byte[] imageBytes;  // vysledna binarni data
				using (MemoryStream ms = new MemoryStream())
				{
					int read;
					while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
					{
						ms.Write(buffer, 0, read);
					}
					imageBytes = ms.ToArray();
				}

				// ulozeni bytu do vlastnosti vybrane polozky
				SelectedBill.ImgBill = imageBytes;
				// nacteni obrazku jako ImageSource do vlastnosti okna
				ImgBill = SelectedBill.GetImage();
			});

			PushBrowserPage = new Command(async () =>
			{
				await navigation.PushAsync(new BrowserPage(Website));
			});

			CallNumber = new Command(async () =>
			{
				try
				{
					await Launcher.TryOpenAsync($"tel: {PhoneNumber}");
				}
				catch
				{
					await App.Current.MainPage.DisplayAlert("Problem", "Cannot make a phone call", "OK");
				}
			});

			ViewImage = new Command(async () =>
			{
				await navigation.PushAsync(new ViewImagePage(SelectedBill));
			});
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
