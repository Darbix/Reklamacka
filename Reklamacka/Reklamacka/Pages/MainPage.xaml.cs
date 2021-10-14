using Reklamacka.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Reklamacka
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Test presunuti se na dalsi stranku
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ItemEditPage());
		}
	}
}
