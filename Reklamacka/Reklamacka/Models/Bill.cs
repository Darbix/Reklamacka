/**
 * @brief Bill model
 * 
 * @file Bill.cs
 * @authors Kedra David (xkedra00), Hung Do (xdohun00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using SQLite;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.IO;
using static Reklamacka.BaseModel;

namespace Reklamacka.Models
{
	public class Bill: INotifyPropertyChanged
	{
		/// <summary>
		/// Auto-incrementing database key
		/// </summary>
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		/// <summary>
		/// The name of the product
		/// </summary>
		public string ProductName { get; set; }

		/// <summary>
		/// Type of purchased goods
		/// </summary>
		public ProductTypes ProductType { get; set; }

		/// <summary>
		/// Image in bytes
		/// </summary>
		public byte[] ImgBill { get; set; }

		/// <summary>
		/// The date of purchase
		/// </summary>
		public DateTime PurchaseDate { get; set; } = DateTime.Today;

		/// <summary>
		/// The date of expiration
		/// </summary>
		public DateTime ExpirationDate { get; set; } = DateTime.Today.AddYears(2);

		/// <summary>
		/// Additional notes/thoughts
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// Store's id from store database
		/// </summary>
		public int ShopID { get; set; } = 0;

		/// <summary>
		/// Bool state whether object contains an image of a bill
		/// </summary>
		public bool HasImage { get; set; } = false;

		/// <summary>
		/// Load image from stream
		/// </summary>
		/// <returns>Image source object if ImgBill property is not null</returns>
		public ImageSource GetImage()
		{
			if (ImgBill == null)
				return null;
			return ImageSource.FromStream(() => new MemoryStream(ImgBill));
		}

		/// <summary>
		/// File path to PDF version of the bill
		/// </summary>
		public string FilePath { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
