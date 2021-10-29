using SQLite;
using Reklamacka.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reklamacka.Models
{
	public enum WarrantyType { OneMonth, TwoYears, ThreeYears /* will be added later */ }
	public enum GoodsType { Clothes, Electronics, Toys /* will be added later */ }


	public class Bill
	{
		/// <summary>
		/// Auto-incrementujici se hodnota ID prvku v databazi
		/// </summary>
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string ProductName { get; set; }     //!< Nazev produktu v uctence


		//TODO----- netestovana cast, zatim tedy bez dat -----
		/*
		public DateTime PurchaseDate { get; private set; }
		public string WarrantyPath { get; set; }
		public Shop Shop { get; set; }
		public WarrantyType WarrantyType { get; set; }
		public string Notes { get; set; }
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
		*/
	}

}
