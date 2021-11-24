using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.IO;
using static Reklamacka.BaseModel;

namespace Reklamacka.Models
{
	public enum WarrantyType { OneMonth, TwoYears, ThreeYears /* will be added later */ }
	//public enum ProductTypes { Clothes, Electronics, Toys /* will be added later */ }

	public class Bill: INotifyPropertyChanged
	{
		/// <summary>
		/// Auto-incrementujici se hodnota ID prvku v databazi
		/// </summary>
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string ProductName { get; set; }     //!< Nazev produktu v uctence
		public ProductTypes ProductType { get; set; }

		public Byte[] ImgBill { get; set; }

		private bool isSelected = false;
		/// <summary>
		/// Bool hodnota urcujici, zda byl item zvolen kliknutim v MainPage
		/// </summary>
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}



		public DateTime PurchaseDate { get; set; } = DateTime.Today;
		public DateTime ExpirationDate { get; set; } = DateTime.Today;
		public string WarrantyPath { get; set; }

		public string Notes { get; set; }

		public ImageSource GetImage()
		{
			return ImageSource.FromStream(() => new MemoryStream(ImgBill));
		}




		//TODO----- netestovana cast, zatim tedy bez dat -----

		//public Shop ProductShop { get; set; }
		public string ShopName { get; private set; }
		public string ShopLink { get; private set; }
		public string ShopNotes { get; set; }


		public WarrantyType WarrantyType { get; set; }
		public bool IsArchived { get; set; }

		/// <summary>
		/// Checks whether an item is still under warranty
		/// </summary>
		/// <returns>true if item is still under warranty</returns>
		public bool IsInWarranty()
		{
			return DateTime.Compare(DateTime.Now, GetWarrantyEndDate()) < 0;
		}

		/// <summary>
		/// Gets number of days left before warranty expires
		/// </summary>
		/// <returns>Number of days before warranty expires</returns>
		public double TimeLeft()
		{
			return (GetWarrantyEndDate() - DateTime.Now).TotalDays;
		}

		/// <summary>
		/// Gets last date where warranty is still valid
		/// </summary>
		/// <returns>Last day of valid warranty</returns>
		private DateTime GetWarrantyEndDate()
		{
			switch (WarrantyType)
			{
				case WarrantyType.OneMonth:
					return PurchaseDate.AddMonths(1);
				case WarrantyType.TwoYears:
					return PurchaseDate.AddYears(2);
				case WarrantyType.ThreeYears:
					return PurchaseDate.AddYears(3);
				default:
					return DateTime.Now;
			}
		}
		


		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
