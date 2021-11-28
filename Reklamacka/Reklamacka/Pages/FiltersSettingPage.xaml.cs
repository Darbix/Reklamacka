using Reklamacka.Models;
using Reklamacka.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FiltersSettingPage : ContentPage
	{
		public FiltersSettingPage(
			ObservableCollection<Bill> AllBills,
			ObservableCollection<FilterItem> ListTypes,
			ObservableCollection<FilterItem> ListUrls)
		{
			InitializeComponent();

			BindingContext = new FiltersSettingViewModel(AllBills, ListTypes, ListUrls);

		}
	}
}