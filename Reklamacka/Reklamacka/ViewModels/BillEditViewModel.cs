using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	public class BillEditViewModel : INotifyPropertyChanged
	{
		public Command SaveBill { get; set; }           //!< Command to save a new bill
		public Command DeleteBill { get; set; }         //!< Command to delete a selected bill (if active)
		public Command PushBrowserPage { get; set; }    //!< Load a store's website in default browser
		public Command PickPhoto { get; set; }          //!< Open a window to select a bill photo
		public Command CallNumber { get; set; }         //!< Load a phone dialer
		public Command ViewImage { get; set; }          //!< Display a bill photo
		public Command PickFile { get; set; }
		public Command AddStorePush { get; set; }
		public Command PickNoneShop { get; set; }
		public Command OpenEmailDefault { get; set; }

		public Bill SelectedBill { get; set; }          //!< Edited bill
		public string ProductName { get; set; }         //!< Goods's name 
		public DateTime PurchaseDate { get; set; }      //!< Date of purchase
		public DateTime ExpirationDate { get; set; }    //!< Date of expire
		public string Notes { get; set; }               //!< Additional notes

		private string FilePath { get; set; }
		private string DefaultFolderPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		private bool IsSaved { get; set; } = false;
		private bool SelectingFile { get; set; } = false;

		public IList<ProductTypes> BillTypes { get; set; } = LofTypes;  //!< List of product types
		private ProductTypes productType;               //!< Chosen product type
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
		/// Chosen photo
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

		private bool hasImage;
		/// <summary>
		/// Status whether bill contains a photo
		/// </summary>
		public bool HasImage
		{
			get => hasImage;
			set
			{
				hasImage = value;
				OnPropertyChanged(nameof(HasImage));
			}
		}

		public ObservableCollection<string> ShopNameList		//!< List of shop names
		{
			get => LofStoreNames;
		}

		private Store shop;										//!< Chosen shop
		public string ShopName									//!< Shop's name to display (Selected item from list of names)
		{
			get => shop != null ? shop.Name : string.Empty;
			set
			{
				shop = StoreDB.GetStoreAsync(value).Result;
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

			// NACTENI kolonek v EditPage vlastnostmi existujici uctenky
			// Nelze primy binding, protoze by se auto-savovalo
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
			FilePath = SelectedBill.FilePath;
			HasImage = SelectedBill.HasImage;

			SaveBill = new Command(async () =>
			{
				if (ProductName == null)
				{
					await App.Current.MainPage.DisplayAlert("Problem", "Product name is needed!", "Ok");
					return;
				}

				// ULOZENI
				// Vypis vlastnosti, ktere se maji ulozit do existujici/nove uctenky
				SelectedBill.ProductName = ProductName;
				SelectedBill.PurchaseDate = PurchaseDate;
				SelectedBill.ExpirationDate = ExpirationDate;
				SelectedBill.Notes = Notes;
				SelectedBill.ProductType = ProductType;
				SelectedBill.HasImage = HasImage;
				SelectedBill.FilePath = FilePath;
				if (shop != null)
					SelectedBill.ShopID = shop.ID;
				else
					SelectedBill.ShopID = 0;
				_ = await BillsDB.SaveItemAsync(SelectedBill);

				IsSaved = true;
				_ = await navigation.PopAsync();
			});

			DeleteBill = new Command(async () =>
			{
				_ = await BillsDB.DeleteItemAsync(SelectedBill);
				_ = await navigation.PopAsync();
			});

			PickPhoto = new Command(async (openCamera) =>
			{
				FileResult img;
				try
				{
					// ziskani objektu obrazkoveho vyberu
					if (((string)openCamera).Equals("1"))
					{
						img = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
						{
							Title = "Take a bill photo"
						});
					}
					else
					{
						img = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
						{
							Title = "Pick a bill photo"
						});
					}
				}
				catch
				{
					await App.Current.MainPage.DisplayAlert("Permission", "Permission is needed for loading a file", "OK");
					return;
				}

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
				if (!string.IsNullOrEmpty(Weblink))
				{
					try
					{
						await Launcher.TryOpenAsync(Weblink);
					}
					catch
					{
						await App.Current.MainPage.DisplayAlert("Invalid link", "This URL format is not valid! Try to change it in Store Management page!", "OK");
						return;
					}
				}
			});

			AddStorePush = new Command(async () =>
			{
				await navigation.PushAsync(new AddStorePage(navigation));
			});

			CallNumber = new Command(async () =>
			{
				if (!string.IsNullOrEmpty(PhoneNumber))
				{
					try
					{
						await Launcher.TryOpenAsync($"tel: {PhoneNumber}");
					}
					catch
					{
						await App.Current.MainPage.DisplayAlert("Problem", "Cannot make a phone call", "OK");
					}
				}
			});

			ViewImage = new Command(async () =>
			{
				await navigation.PushAsync(new ViewImagePage(SelectedBill));
			});

			PickFile = new Command(async () =>
			{
				string result;
				string fileName = FilePath == null ? null : Path.GetFileName(Path.Combine(FilePath));

				int maxLen = 20;
				string fileShortName = fileName;
				if (fileName != null)
					fileShortName = fileName.Length > maxLen ? fileName.Substring(0, maxLen) + "..." : fileName;

				if (fileName == null)
					result = await App.Current.MainPage.DisplayActionSheet("Attach a file", "Cancel", null, "Pick");
				else
					result = await App.Current.MainPage.DisplayActionSheet("Attach a file", "Cancel", null, "View  " + fileShortName, "Delete  " + fileShortName, "Change");

				if (result == null)
					return;
				//if (result.Equals("Cancel"))
				//	return;

				if (result.Equals("Pick") || result.Equals("Change"))
				{
					try
					{
						// Pomocny predikat k detekci, zda jde o zavreni okna => OnDisappearing, nebo jen otevreni FilePicker
						SelectingFile = true;
						FileResult fileResult = await FilePicker.PickAsync();
						SelectingFile = false;

						if (fileResult != null)
						{
							fileName = fileResult.FileName;

							// Opakovana snaha o zmenu nazvu souboru
							while (File.Exists(Path.Combine(DefaultFolderPath, fileName)))
							{
								var promptResult = await App.Current.MainPage.DisplayPromptAsync("The file already exists", "Enter a new name", "OK", "Cancel", "my_file");

								if (promptResult == null)
									return;

								// Pripojeni pripony k novemu nazvu
								fileName = promptResult + "." + fileResult.FileName.Substring(fileResult.FileName.IndexOf('.') + 1);
							}

							// Pokud jde o zmenu souboru, stary se vymaze
							if (FilePath != null)
								File.Delete(Path.Combine(FilePath));
							if (SelectedBill != null)
								SelectedBill.FilePath = null;

							File.Copy(fileResult.FullPath, Path.Combine(DefaultFolderPath, fileName));

							FilePath = Path.Combine(DefaultFolderPath, fileName);
						}
					}
					catch
					{
						await App.Current.MainPage.DisplayAlert("Unexpected Error", "There were problems with picking a file", "Cancel");
						return;
					}
				}
				else if (result.StartsWith("Delete"))
				{
					var boolDelete = await App.Current.MainPage.DisplayAlert("Delete " + fileShortName, "Would you like to delete the file?", "Ok", "Cancel");
					if (boolDelete)
					{
						File.Delete(Path.Combine(FilePath));
						FilePath = null;
						// pokud by se okno zavrelo sipkou, zustala by neplatna cesta k souboru
						if (SelectedBill != null)
							SelectedBill.FilePath = null;
					}
				}
				else if(!result.StartsWith("Cancel"))
				{
					try
					{
						await Launcher.OpenAsync(new OpenFileRequest
						{
							File = new ReadOnlyFile(Path.Combine(FilePath))
						});
					}
					catch
					{
						await App.Current.MainPage.DisplayAlert("Error", "The file does not exist", "Cancel");
						//todo clear path
					}
				}

				//todo smazat, jen pomocny vypis obsahu dir
				var allFiles = Directory.GetFiles(DefaultFolderPath);
				string finalText = "";
				for (int i = 0; i < allFiles.Length; i++)
					finalText += Path.GetFileName(allFiles[i]) + ',';
				await App.Current.MainPage.DisplayAlert("info", DefaultFolderPath + "|||" + finalText, "OK");
			});

			PickNoneShop = new Command(() =>
			{
				shop = null;
				Email = null;
				PhoneNumber = null;
				Weblink = null;
				ShopName = null;
			});

			OpenEmailDefault = new Command(async () =>
			{
				if (!string.IsNullOrEmpty(Email))
				await Launcher.OpenAsync($"mailto:{Email}");
			});
		}

		public void OnAppearing()
		{
			OnPropertyChanged(nameof(ShopNameList));
		}

		public void OnDisappearing()
		{
			if (IsSaved == false && SelectingFile == false)
			{
				if (FilePath != null && SelectedBill != null)
				{
					if (FilePath != SelectedBill.FilePath)
						File.Delete(Path.Combine(FilePath));
				}
			}
		}

		// event informujici o zmene, udalost volat pri zmenach hodnot vlastnosti
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
