using Reklamacka.Models;
using Reklamacka.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Reklamacka.ViewModels
{
	public class BillEditViewModel : INotifyPropertyChanged
	{

		public Command SaveNewBill { get; set; }        //!< Command k tlacitku pro ulozeni zmen/nove uctenky
		public Command DeleteNewBill { get; set; }      //!< Command k tlacitku smazani vsech polozek TODO zatim vsechny

		public Bill SelectedBill { get; set; } = null;  //!< Aktualne zvoleny item z listu na hlavni strance


		public string ProductName { get; set; }                         //!< Nazev produktu
		public DateTime PurchaseDate { get; set; } = DateTime.Today;    //!< Datum zakoupeni produktu
		public DateTime ExpirationDate { get; set; } = DateTime.Today;  //!< Doba konce platnosti zaruky
		public string Notes { get; set; }                               //!< Poznamky
		//TODO dalsi vlastnosti objektu

		// konstruktor 
		public BillEditViewModel(INavigation Navigation, Bill bill)
		{
			// nastaveni SelectedBill na item zvoleny v listu v MainPage
			SelectedBill = bill;
			// neni-li SelectedBill null, budou u editace zobrazeny data existujici polozky
			if (SelectedBill != null)
			{
				// Vyplneni kolonek v EditPage vlastnostmi existujici uctenky
				ProductName = SelectedBill.ProductName;
				PurchaseDate = SelectedBill.PurchaseDate;
				ExpirationDate = SelectedBill.ExpirationDate;
				Notes = SelectedBill.Notes;
				//TODO dalsi vlastnosti k editaci
			}

			// vytvoreni Commandu pro ulozeni
			SaveNewBill = new Command(async () =>
			{
				// pokud byl SelectedItem null, generuje se novy objekt ze zadanych dat
				if (SelectedBill == null)
				{
					SelectedBill = new Bill();
				};

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
		}

		// event informujici o zmene, udalost volat pri zmenach hodnot vlastnosti
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}

}
