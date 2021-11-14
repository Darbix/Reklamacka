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
		public SortingPage()
		{
			InitializeComponent();
			BindingContext = new SortingPageViewModel();
		}
	}
}