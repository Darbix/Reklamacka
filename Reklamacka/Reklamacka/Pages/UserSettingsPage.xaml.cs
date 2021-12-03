using Reklamacka.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserSettingsPage : ContentPage
	{
		public UserSettingsPage()
		{
			InitializeComponent();

			BindingContext = new UserSettingsViewModel();
		}
	}
}