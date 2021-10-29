using Reklamacka.ViewModels;
using Reklamacka.Models;
using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Reklamacka
{
	public partial class MainPage : ContentPage
	{
		private readonly MainPageViewModel mainPageViewModel;   //!< viemodel hlavni stranky
		public MainPage()
		{
			InitializeComponent();
			mainPageViewModel = new MainPageViewModel(Navigation);
			BindingContext = mainPageViewModel;
		}

		// Funkce volajici pri kazdem nacteni hlavni stranky obdobnou funkci ve viewmodelu
		protected override void OnAppearing()
		{
			base.OnAppearing();
			mainPageViewModel.OnAppearing();
		}

		// Funkce, ktera vola ListBillsItemSelected z viemodelu pri vyberu polozky z listu
		private void ListBillsItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			mainPageViewModel.ListBillsItemSelected(sender, e);
		}

	}
}
