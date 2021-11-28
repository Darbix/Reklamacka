using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static Reklamacka.BaseModel;

namespace Reklamacka.Models
{
	public class FilterItem: INotifyPropertyChanged
	{
		public string Name { get; set; }
		public ProductTypes Type { get; set; }
		public string ShopUrl { get; set; }

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
