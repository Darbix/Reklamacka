using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Reklamacka.ViewModels
{
	public class BrowserViewModel: INotifyPropertyChanged
	{
		private string sourceUrl;
		/// <summary>
		/// Puvodni nacitana Url adresa
		/// </summary>
		public string SourceUrl
		{
			get => sourceUrl;
			set
			{
				sourceUrl = value;
				OnPropertyChanged(nameof(SourceUrl));
			}
		}

		public string webLink;					//!< Nova url adresa

		public Web website;                     //!< Odkaz na objekt z EditPage

		public Command SaveUrl { get; set; }

		public BrowserViewModel(Web website)
		{
			this.website = website;
			SourceUrl = website.Link;
			if (SourceUrl == null)
				SourceUrl = "https://www.google.com";

			SaveUrl = new Command(() =>
			{
				website.Link = webLink;
				App.Current.MainPage.Navigation.PopAsync();
			});
		}

		public void GetUrl(WebNavigatingEventArgs e)
		{
			webLink = e.Url;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
