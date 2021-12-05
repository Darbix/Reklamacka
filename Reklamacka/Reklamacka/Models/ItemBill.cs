/**
 * @brief Bill item with additional bool properties
 * 
 * @file ItemBill.cs
 * @author Do Hung (xdohun00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using System.ComponentModel;
using static Reklamacka.BaseModel;

namespace Reklamacka.Models
{
	public class ItemBill : INotifyPropertyChanged
	{
		/// <summary>
		/// Bill instance
		/// </summary>
		public Bill BillItem { get; set; }
		
		/// <summary>
		/// Bill's store name
		/// </summary>
		public string StoreName { get; private set; }
		private bool isVisible = true;
		private bool isSelected = false;

		/// <summary>
		/// Item's visibility
		/// </summary>
		public bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;
				OnPropertyChanged(nameof(IsVisible));
			}
		}
		
		/// <summary>
		/// Item selection status
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

		/// <summary>
		/// Initialization of item
		/// </summary>
		/// <param name="bill">Instance of the bill</param>
		public ItemBill(Bill bill)
		{
			BillItem = bill;
			if (BillItem == null)
				StoreName = string.Empty;
			if (BillItem.ShopID == 0)
				StoreName = string.Empty;
			UpdateStoreName();
		}

		/// <summary>
		/// Updates the name of the store
		/// </summary>
		public void UpdateStoreName()
		{
			if (BillItem == null)
				return;
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