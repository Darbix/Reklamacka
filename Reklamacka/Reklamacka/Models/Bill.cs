using SQLite;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.IO;
using static Reklamacka.BaseModel;

namespace Reklamacka.Models
{
	public enum WarrantyType { OneMonth, TwoYears, ThreeYears /* will be added later */ }

	public class Bill: INotifyPropertyChanged
	{
		/// <summary>
		/// Auto-incrementujici se hodnota ID prvku v databazi
		/// </summary>
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string ProductName { get; set; }     //!< Nazev produktu v uctence
		public ProductTypes ProductType { get; set; }


		public byte[] ImgBill { get; set; }

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
		//public string WarrantyPath { get; set; }

		public string Notes { get; set; }

		public int ShopID { get; set; } = 0;

		public bool HasImage { get; set; } = false;

		public ImageSource GetImage()
		{
			if (ImgBill == null)
				return null;
			return ImageSource.FromStream(() => new MemoryStream(ImgBill));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
