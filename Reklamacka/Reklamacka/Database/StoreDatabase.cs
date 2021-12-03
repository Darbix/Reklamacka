using Reklamacka.Models;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static Reklamacka.BaseModel;

namespace Reklamacka.Database
{
	public class StoreDatabase
	{
		readonly SQLiteAsyncConnection db;

		public StoreDatabase(string dbPath)
		{
			db = new SQLiteAsyncConnection(dbPath);
			db.CreateTableAsync<Store>().Wait();
		}

		// Save new Store instance to database
		public Task<int> SaveStoreInstanceAsync(Store store)
		{
			if (store != null && store.ID == 0) //todo neradi ho pak abecedne
				LofStoreNames.Add(store.Name);
			return store.ID != 0 ? db.UpdateAsync(store) : db.InsertAsync(store);
		}

		// Get list of stores from database
		public Task<List<Store>> GetStoresListAsync()
			=> db.Table<Store>().ToListAsync();

		// Return ordered list of store names
		public Task<List<Store>> GetStoresListAlphabetOrderAsync()
			=> db.Table<Store>().OrderBy(x => x.Name).ToListAsync();

		// Search for store by its ID
		public Task<Store> GetStoreAsync(int id)
			=> db.Table<Store>().Where(i => i.ID == id).FirstOrDefaultAsync();

		// Search for store by its name
		public Task<Store> GetStoreAsync(string storeName)
			=> db.Table<Store>().Where(i => i.Name == storeName).FirstOrDefaultAsync();

		// Delete store from the database
		public Task<int> DeleteStoreAsync(Store store)
		{
			// Also clear from the list of store names
			//if (store != null && LofStoreNames.Contains(store.Name))
			//	LofStoreNames.Remove(store.Name);
			return db.DeleteAsync(store);
		}

		// Clear whole database
		public Task<int> ClearDatabase()
		{
			// Also clear the list of store names
			LofStoreNames.Clear();
			return db.DeleteAllAsync<Store>();
		}
	}
}
