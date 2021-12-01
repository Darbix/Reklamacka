using Xamarin.Forms;

using Reklamacka.Models;
using static Reklamacka.BaseModel;

namespace Reklamacka.ViewModels
{
	public class AddStoreViewModel : ContentView
	{
		public Command SaveStore { get; set; }	//!< Command for saving new store to the database
		public Command DeleteAll { get; set; }	//!< Command to clear store database; TODO: temporary solution
		private Store storeInstance;
		public string StoreName { get; set; }	//!< New store's name
		public string StoreLink { get; set; }	//!< New store's weblink
		public string Email { get; set; }		//!< New store's contact email
		public string PhoneNumber { get; set; }	//!< New store's contact number
		public AddStoreViewModel(INavigation navig)
		{
			SaveStore = new Command(async () =>
			{
				// create a new instance of the store
				storeInstance = new Store
				{
					Name = StoreName,
					Link = StoreLink,
					Email = Email,
					PhoneNumber = PhoneNumber
				};

				// search for existance of store in database
				var found = StoreDB.GetStoreAsync(storeInstance.Name).Result;

				// assign an ID if found, otherwise add new store's name to list of store names
				if (found != null)
					storeInstance.ID = found.ID;

				// save to database
				await StoreDB.SaveStoreInstanceAsync(storeInstance);
				await navig.PopAsync();
			});

			DeleteAll = new Command(async () =>
			{
				await StoreDB.ClearDatabase();
				// clear store records of every bill in the database
				BillsDB.GetAllItemsAsync().Result.ForEach(bill => bill.ShopID = 0);
				await navig.PopAsync();
			});
		}
	}
}