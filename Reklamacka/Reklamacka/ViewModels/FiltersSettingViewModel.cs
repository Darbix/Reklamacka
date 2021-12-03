using Reklamacka.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Reklamacka.BaseModel;
using static Reklamacka.ViewModels.SortingPageViewModel;

namespace Reklamacka.ViewModels
{
	public class FiltersSettingViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<FilterItem> ListTypes { get; set; }			//!< Product types filter
		public ObservableCollection<FilterItem> ListStoreNames { get; set; }	//!< Stores filter
		public Command SelectChoice { get; set; }

		public FiltersSettingViewModel()
		{
			// init filters
			if (FilterLofTypes == null)
			{
				FilterLofTypes = new ObservableCollection<FilterItem>();
				LofTypes.ToList().ForEach(x => FilterLofTypes.Add(new FilterItem
				{
					IsChecked = false,
					Type = x
				}));
			}

			if (FilterLofStoreNames == null)
			{
				FilterLofStoreNames = new ObservableCollection<FilterItem>();

				LofStoreNames.ToList().ForEach(x => FilterLofStoreNames.Add(new FilterItem
				{
					IsChecked = false,
					ShopName = x
				}));
			}
			ListTypes = FilterLofTypes;
			ListStoreNames = FilterLofStoreNames;

			SelectChoice = new Command((f) =>
			{
				if (!(f is FilterItem filterItem))
					return;

				filterItem.IsChecked = !filterItem.IsChecked;
			});
		}

		public async void OnDisappearing()
		{
			LofFilteredProductTypes = ListTypes.Where(x => x.IsChecked).Select(x => x.Type).ToList();
			LofFilteredShopNames = ListStoreNames.Where(x => x.IsChecked).Select(item => item.ShopName).ToList();
			await System.Threading.Tasks.Task.CompletedTask;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
