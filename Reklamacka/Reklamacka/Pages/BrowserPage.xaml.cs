using Reklamacka.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BrowserPage : ContentPage
	{
		public Web website;
		public BrowserPage(Web website)
		{
			InitializeComponent();
			BindingContext = new BrowserViewModel(website);
			this.website = website;
		}

		private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
		{
			((BrowserViewModel)BindingContext).GetUrl(e);
		}

		private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
		{
			((BrowserViewModel)BindingContext).Navigated(sender, e);
		}
	}
}