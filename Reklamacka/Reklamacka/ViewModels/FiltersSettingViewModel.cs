using Reklamacka.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public Command SelectChoice { get; set; }

		public FiltersSettingViewModel(ObservableCollection<FilterItem> ListTypes)
		{
			this.ListTypes = ListTypes;
			List<ProductTypes> types = Enum.GetValues(typeof(ProductTypes)).Cast<ProductTypes>().ToList();
			if (ListTypes.Count == 0)
			{
				for (int i = 0; i < types.Count; i++)
				{
					var item = new FilterItem() { Type = types[i] };
					ListTypes.Add(item);
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
