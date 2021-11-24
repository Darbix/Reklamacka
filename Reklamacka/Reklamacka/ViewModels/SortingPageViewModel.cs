using Reklamacka.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Reklamacka.ViewModels
{
	public class SortingPageViewModel: INotifyPropertyChanged
	{
		private ObservableCollection<Bill> bills;
		public ObservableCollection<Bill> Bills
		{
			get => bills;
			set
			{
				bills = value;
				OnPropertyChanged(nameof(Bills));
			}
		}


		public SortingPageViewModel(INavigation Navigation)
		{

		}

		public async void OnAppearing()
		{
			Bills = new ObservableCollection<Bill>(await BaseModel.BillsDB.GetAllExpiredAsync());	
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
