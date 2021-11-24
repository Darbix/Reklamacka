using Reklamacka.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Reklamacka
{
	/// <summary>
	/// Trida pro globalni statickou databazi uctenek
	/// </summary>
	public static class BaseModel
	{

		static BillsDatabase db;
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

		/// <summary>
		/// Vycet typu produktu 
		/// </summary>
		public enum ProductTypes { Other, Clothes, Electronics, Toys /* will be added later */ }

	}
}
