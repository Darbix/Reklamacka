using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Reklamacka.ViewModels;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddStorePage : ContentPage
	{
		public AddStorePage(INavigation navig)
		{
			InitializeComponent();
			BindingContext = new AddStoreViewModel(navig);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			((AddStoreViewModel)BindingContext).OnAppearing();
		}
	}
}