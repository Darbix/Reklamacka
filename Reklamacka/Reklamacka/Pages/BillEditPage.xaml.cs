using Reklamacka.ViewModels;
using Reklamacka.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
			// ((BillEditViewModel)BindingContext).OnAppearing();
		}
	}
}