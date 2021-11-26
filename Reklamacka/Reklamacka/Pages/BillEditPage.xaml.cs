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

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BillEditPage : ContentPage
	{
		public BillEditPage(Bill bill)
		{
			InitializeComponent();
			// pri vytvoreni viewmodelu se predava aktualne zvolena polozka z MainPage
			BindingContext = new BillEditViewModel(Navigation, bill);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			((BillEditViewModel)BindingContext).OnAppearing();
		}
	}
}