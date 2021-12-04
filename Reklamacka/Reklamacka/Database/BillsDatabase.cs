using Reklamacka.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			if(bill != null && bill.FilePath != null)
			{
				try { File.Delete(Path.Combine(bill.FilePath)); }
				catch { }
			}
			return db.DeleteAsync(bill);
		}

		public Task<int> DeleteAllItems<T>()
		{
			var getDb = this.GetAllAsync().Result.Where(bill => !string.IsNullOrWhiteSpace(bill.FilePath)).ToList();
			getDb.ForEach(bill => 
			{
				try { File.Delete(Path.Combine(bill.FilePath)); }
				catch { }
			});
			return db.DeleteAllAsync<Bill>();
		}

		public Task<Bill> GetItemAsync(int id)
			=> db.Table<Bill>().Where(i => i.ID == id).FirstOrDefaultAsync();

		public Task<Bill> GetItemAsync(string name)
			=> db.Table<Bill>().Where(i => i.ProductName == name).FirstOrDefaultAsync();

		// Funkce k ziskani itemu jako list serazenych od nejnovejsich
		public Task<List<Bill>> GetAllItemsAsync()
		{
			return db.Table<Bill>().OrderBy(x => x.ExpirationDate).Where(x => x.ExpirationDate >= DateTime.Today).ToListAsync();
		}

		// Funkce k ziskani itemu jako list serazenych od nejstarsich
		public Task<List<Bill>>GetAllFromOldestAsync()
		{
			return db.Table<Bill>().OrderByDescending(x => x.ExpirationDate).Where(x => x.ExpirationDate >= DateTime.Today).ToListAsync();
		}

		// Funkce k ziskani proslych itemu od nejnovejsich
		public Task<List<Bill>> GetAllExpiredAsync()
		{
			return db.Table<Bill>().OrderBy(x => x.ExpirationDate).Where(x => x.ExpirationDate < DateTime.Today).ToListAsync();
		}

		// Funkce k ziskani proslych itemu jako list serazenych od nejstarsich
		public Task<List<Bill>> GetAllExpiredFromOldestAsync()
		{
			return db.Table<Bill>().OrderByDescending(x => x.ExpirationDate).Where(x => x.ExpirationDate < DateTime.Today).ToListAsync();
		}

		public Task<List<Bill>> GetAllAsync()
		{
			return db.Table<Bill>().OrderBy(x => x.ExpirationDate).ToListAsync();
			
		}

	}
}
