/**
 * @brief Shares global objects with ohter part of the app
 * 
 * @file BaseModel.cs
 * @author Kedra David (xkedra00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using Reklamacka.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Reklamacka
{
	/// <summary>
	/// Trida pro globalni statickou databazi uctenek
	/// </summary>
	public static class BaseModel
	{
		/// <summary>
		/// Vycet typu produktu 
		/// </summary>
		public enum ProductTypes { Other, Cars, Clothes, Electronics, Shoes, Tools, Toys /* will be added later */ }
		private static ObservableCollection<string> lofStoreNames;
		public static ObservableCollection<string> LofStoreNames    //!< List of store names from the database
		{
			get
			{
				// first time loading
				if (lofStoreNames == null)
				{
					lofStoreNames = new ObservableCollection<string>(StoreDB.GetStoresListAlphabetOrderAsync().Result.Select(shop => shop.Name));
				}
				return lofStoreNames;
			}
			set
			{
				lofStoreNames = value;
			}
		}
		public static IList<ProductTypes> LofTypes { get; set; } = Enum.GetValues(typeof(ProductTypes)).Cast<ProductTypes>().ToList();

		private static BillsDatabase db;
		private static StoreDatabase storeDB;
		/// <summary>
		/// Hlavni property-objekt databaze
		/// </summary>
		public static BillsDatabase BillsDB
		{
			get
			{
				if (db == null)
				{
					// cesta k databazovemu souboru v zarizeni
					db = new BillsDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BillsDatabase.db3"));
				}
				return db;
			}
		}

		public static StoreDatabase StoreDB
		{
			get
			{
				// first time inicialization
				if (storeDB == null)
				{
					storeDB = new StoreDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ShopDatabase.db3"));
				}
				return storeDB;
			}
		}

	}
}
