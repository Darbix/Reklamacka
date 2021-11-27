using Reklamacka.Models;
using Reklamacka.Pages;
using Reklamacka.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	public class BillEditViewModel : INotifyPropertyChanged
	{

		public Command SaveNewBill { get; set; }                //!< Command k tlacitku pro ulozeni zmen/nove uctenky
		public Command DeleteNewBill { get; set; }              //!< Command k tlacitku smazani vsech polozek TODO zatim vsechny
		public Command PickPhoto { get; set; }                  //!< Command k vyberu fotografie uctenky produktu
		public Command PushBrowserPage { get; set; }            //!< Command k otevreni webu
		public Command CallNumber { get; set; }					//!< Command k presunuti do telefonovaci aplikace
		public Command ViewImage { get; set; }

		public Bill SelectedBill { get; set; }                  //!< Aktualne zvoleny item z listu na hlavni strance

		public string webLink;
		public string WebLink
		{
			get => Website.Link;
			set
			{
				Website.Link = value;
				OnPropertyChanged(nameof(WebLink));
			}
		}
		public Web Website = new Web();


		public string ProductName { get; set; }         //!< Nazev produktu
		public DateTime PurchaseDate { get; set; }      //!< Datum zakoupeni produktu
		public DateTime ExpirationDate { get; set; }    //!< Doba konce platnosti zaruky
		public string Notes { get; set; }               //!< Poznamky
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public bool HasImage { get; set; }

		public IList<ProductTypes> BillTypes { get; set; } = Enum.GetValues(typeof(ProductTypes)).Cast<ProductTypes>().ToList();
		private ProductTypes productType;
		public ProductTypes ProductType
		{
			get => productType;
			set
			{
				productType = value;
				OnPropertyChanged(nameof(ProductType));
			}
		}

		private ImageSource imgBill;
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
		//TODO dalsi vlastnosti objektu

		// konstruktor 
		public BillEditViewModel(INavigation Navigation, Bill bill)
		{
			// nastaveni SelectedBill na item zvoleny v listu v MainPage
			SelectedBill = bill;

			if (SelectedBill == null)
				SelectedBill = new Bill();

			// Vyplneni kolonek v EditPage vlastnostmi existujici uctenky
			// Nelze primy binding, protoze by se auto-savovalo
			ProductName = SelectedBill.ProductName;
			PurchaseDate = SelectedBill.PurchaseDate;
			ExpirationDate = SelectedBill.ExpirationDate;
			Notes = SelectedBill.Notes;
			ImgBill = SelectedBill.GetImage();
			ProductType = SelectedBill.ProductType;
			WebLink = SelectedBill.ShopUrl;
			HasImage = SelectedBill.HasImage;
			Email = SelectedBill.Email;
			PhoneNumber = SelectedBill.PhoneNumber == 0 ? "" : SelectedBill.PhoneNumber.ToString();
			//TODO dalsi vlastnosti k editaci

			// vytvoreni Commandu pro ulozeni
			SaveNewBill = new Command(async () =>
			{
				if (ProductName == null)
				{
					await App.Current.MainPage.DisplayAlert("Problem", "Missing product info", "OK");
					return;
				}

				// Vypis vlastnosti, ktere se maji ulozit do existujici/nove uctenky
				SelectedBill.ProductName = ProductName;
				SelectedBill.IsSelected = false;
				SelectedBill.PurchaseDate = PurchaseDate;
				SelectedBill.ExpirationDate = ExpirationDate;
				SelectedBill.Notes = Notes;
				SelectedBill.ProductType = ProductType;
				SelectedBill.ShopUrl = WebLink;
				SelectedBill.HasImage = HasImage;
				SelectedBill.Email = Email;
				SelectedBill.PhoneNumber = PhoneNumber.Equals("")? 0 : Int32.Parse(PhoneNumber);

				//TODO dalsi vlastnosti


				// ulozeni/aktualizace do databaze
				await BaseModel.BillsDB.SaveItemAsync(SelectedBill);

				await Navigation.PopAsync();
			});

			// vytvoreni Commandu pro smazani polozkek
			DeleteNewBill = new Command(async () =>
			{
				// smazani vsech polozek v databazi
				await BaseModel.BillsDB.DeleteAllItems<Bill>();
			});

			PickPhoto = new Command(async () =>
			{
				// ziskani objektu obrazkoveho vyberu
				var img = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
				{
					Title = "Pick a bill photo"
				});

				if (img != null)
				{
					HasImage = true;
				}
				else
					return;

				// nacteni obrazku
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
				await Navigation.PushAsync(new BrowserPage(Website));
			});

			CallNumber = new Command(async () =>
			{
				try
				{
					await Launcher.TryOpenAsync("tel:" + PhoneNumber);
				}
				catch
				{
					await App.Current.MainPage.DisplayAlert("Problem", "Cannot make a phonecall", "OK");
				}

			});

			ViewImage = new Command(async () =>
			{
				await Navigation.PushAsync(new ViewImagePage(SelectedBill));
			});
		}

		public void OnAppearing()
		{
			// potrebna aktualizace labelu na novy web
			WebLink = Website.Link;
		}

		// event informujici o zmene, udalost volat pri zmenach hodnot vlastnosti
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}

	// trida pro web k predani jako reference, aby sel web menit z dalsiho okna
	public class Web
	{
		public string Link { get; set; }
	}

}
