using Reklamacka.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SortingPage : ContentPage
	{
		readonly SortingPageViewModel sortingPageViewModel;

		public SortingPage()
		{
			InitializeComponent();

			sortingPageViewModel = new SortingPageViewModel(Navigation);

			BindingContext = sortingPageViewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			sortingPageViewModel.OnAppearing();
		}
	}
}