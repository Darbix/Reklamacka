using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Reklamacka.Pages;
using Reklamacka.Database;
using System.IO;

namespace Reklamacka
{
	public partial class App : Application
	{

		public App()
		{
			InitializeComponent();
			MainPage = new NavigationPage(new MainPage());
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
