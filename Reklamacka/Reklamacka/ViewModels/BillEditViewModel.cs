using Reklamacka.Models;
using System;
using System.Collections.Generic;
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

		private string productName;
		/// <summary>
		/// Vlastnost pro nazev produktu k uctence
		/// </summary>
		public string ProductName
		{
			get { return productName; }
			set
			{
				productName = value;
				OnPropertyChanged(nameof(ProductName));
			}
		}

		//TODO dalsi vlastnosti objektu

		// konstruktor 
		public BillEditViewModel(Bill bill)
		{
			// nastaveni SelectedBill na item zvoleny v listu v MainPage
			SelectedBill = bill;
			// neni-li SelectedBill null, budou u editace zobrazeny data existujici polozky
			if (SelectedBill != null)
			{
				ProductName = SelectedBill.ProductName;
				//TODO dalsi vlastnosti k editaci
			}

			// vytvoreni Commandu pro ulozeni
			SaveNewBill = new Command(() =>
			{
				// pokud byl SelectedItem null, generuje se novy objekt ze zadanych dat
				if (SelectedBill == null)
				{
					SelectedBill = new Bill();
				};
				SelectedBill.ProductName = ProductName;

				// ulozeni/aktualizace do databaze
				BaseModel.BillsDB.SaveItemAsync(SelectedBill);
			});

			// vytvoreni Commandu pro smazani polozkek
			DeleteNewBill = new Command(() =>
			{
				// smazani vsech polozek v databazi
				BaseModel.BillsDB.DeleteAllItems<Bill>();
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
