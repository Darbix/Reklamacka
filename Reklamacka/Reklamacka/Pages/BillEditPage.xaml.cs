using Reklamacka.ViewModels;
using Reklamacka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using Xamarin.Essentials;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BillEditPage : ContentPage
	{
		public BillEditPage(INavigation navig, Bill bill)
		{
			InitializeComponent();
			// pri vytvoreni viewmodelu se predava aktualne zvolena polozka z MainPage
			BindingContext = new BillEditViewModel(navig, bill);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			((BillEditViewModel)BindingContext).OnAppearing();
		}
		protected override void OnDisappearing()
		{
			((BillEditViewModel)BindingContext).OnDisappearing();
			base.OnDisappearing();
		}
	}
}