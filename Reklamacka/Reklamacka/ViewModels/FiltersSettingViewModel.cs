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

namespace Reklamacka.ViewModels
{
	public class FiltersSettingViewModel
	{

		//public List<ProductTypes> ListTypes { get; set; } = Enum.GetValues(typeof(ProductTypes)).Cast<ProductTypes>().ToList();

		public ObservableCollection<FilterItem> ListTypes { get; set; }// = new ObservableCollection<FilterItem>();
		public ObservableCollection<FilterItem> ListUrls { get; set; }// = new ObservableCollection<FilterItem>();

		public Command SelectChoice { get; set; }
		public ObservableCollection<Bill> AllBills { get; set; }

		public FiltersSettingViewModel(
			ObservableCollection<Bill> AllBills,
			ObservableCollection<FilterItem> ListTypes,
			ObservableCollection<FilterItem> ListUrls)
		{
			this.AllBills = AllBills;
			this.ListUrls = ListUrls;
			this.ListTypes = ListTypes;

			if (AllBills == null || ListUrls == null || ListTypes == null)
				return;

			// Naplneni kolonek vyberu filtru typy
			List<ProductTypes> types = Enum.GetValues(typeof(ProductTypes)).Cast<ProductTypes>().ToList();
			if (ListTypes.Count == 0)
			{
				for (int i = 0; i < types.Count; i++)
				{
					var item = new FilterItem() { Type = types[i] };
					ListTypes.Add(item);
				}
			}

			// Naplneni kolonek vyberu filtru url adresami obchodu
			if (ListUrls.Count == 0)
			{
				for (int i = 0; i < AllBills.Count; i++)
				{
					if (AllBills[i].ShopUrl == null)
						continue;
					var item = new FilterItem() { ShopUrl = AllBills[i].ShopUrl };
					ListUrls.Add(item);
				}
			}


			// zvoleni Filtru za-checknutim
			SelectChoice = new Command((f) =>
			{
				if (!(f is FilterItem filterItem))
					return;

				if (filterItem.IsChecked)
					filterItem.IsChecked = false;
				else
					filterItem.IsChecked = true;
			});
		}
	}
}
