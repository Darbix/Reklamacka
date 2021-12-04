using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reklamacka.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ArchivePage : ContentPage
	{
		private ArchiveViewModel archiveVM;
		public ArchivePage(INavigation navig)
		{
			InitializeComponent();
			archiveVM = new ArchiveViewModel(navig);
			BindingContext = archiveVM;
		}
		protected override void OnAppearing()
		{
			archiveVM.OnAppearing();
			base.OnAppearing();
		}
	}
}