using Reklamacka.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reklamacka.Database
{
	public class BillsDatabase
	{
		readonly SQLiteAsyncConnection db;

		public BillsDatabase(string dbPath)
		{
			// vytvoreni souboru databaze na dbPath
			db = new SQLiteAsyncConnection(dbPath);

			// vytvoreni tabulky pro objekty Bills v databazi
			db.CreateTableAsync<Bill>().Wait();
		}


		//Insert and Update new record  
		public Task<int> SaveItemAsync(Bill bill)
		{
			return bill.ID != 0 ? db.UpdateAsync(bill) : db.InsertAsync(bill);
		}

		//Delete  
		public Task<int> DeleteItemAsync(Bill bill)
		{
			return db.DeleteAsync(bill);
		}

		public Task<int> DeleteAllItems<T>()
		{
			return db.DeleteAllAsync<Bill>();
		}

		//Get a list of all the notes
		public Task<List<Bill>> GetAllItemsAsync()
		{
			return db.Table<Bill>().ToListAsync();
		}
	}
}
