using System.ComponentModel;
using static Reklamacka.BaseModel;

namespace Reklamacka.Models
{
	public class ItemBill : INotifyPropertyChanged
	{
		public Bill BillItem { get; set; }
		public string StoreName { get; private set; }
		private bool isVisible = true;
		private bool isSelected = false;
		public bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;
				OnPropertyChanged(nameof(IsVisible));
			}
		}
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public ItemBill(Bill bill)
		{
			BillItem = bill;
			if (BillItem == null)
				StoreName = string.Empty;
			if (BillItem.ShopID == 0)
				StoreName = string.Empty;
			Store store = StoreDB.GetStoreAsync(BillItem.ShopID).Result;
			StoreName = store != null ? store.Name : string.Empty;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
