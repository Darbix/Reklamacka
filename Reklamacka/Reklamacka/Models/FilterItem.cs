/**
 * @brief Object of one filter option
 * 
 * @file FilterItem.cs
 * @author Kedra David (xkedra00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using System.ComponentModel;
using static Reklamacka.BaseModel;

namespace Reklamacka.Models
{
	public class FilterItem: INotifyPropertyChanged
	{
		/// <summary>
		/// Type of goods
		/// </summary>
		public ProductTypes Type { get; set; }

		/// <summary>
		/// The name of the store
		/// </summary>
		public string ShopName { get; set; }
		private bool isChecked = false;

		/// <summary>
		/// Bool hodnota urcujici, zda byl item zvolen oznacenim ve filtrech
		/// </summary>
		public bool IsChecked
		{
			get => isChecked;
			set
			{
				isChecked = value;
				OnPropertyChanged(nameof(IsChecked));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
