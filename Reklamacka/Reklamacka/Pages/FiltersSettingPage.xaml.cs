using Reklamacka.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FiltersSettingPage : ContentPage
	{
		private FiltersSettingViewModel filterVM;
		public FiltersSettingPage()
		{
			NavigationPage.SetHasBackButton(this, true);
			InitializeComponent();

			filterVM = new FiltersSettingViewModel();
			BindingContext = filterVM;

		}

		protected override void OnDisappearing()
		{
			filterVM.OnDisappearing();
			base.OnDisappearing();
		}

		protected override bool OnBackButtonPressed()
		{
			filterVM.OnDisappearing();
			return base.OnBackButtonPressed();
		}
	}
}