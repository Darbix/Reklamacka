using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
		private string defaultUrl = "https://www.google.com";


		public BrowserViewModel(Web website)
		{
			this.website = website;
			SourceUrl = CorrectUrl(website.Link);
			if (SourceUrl == null)
				SourceUrl = defaultUrl;

			string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
			bool result = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).IsMatch(SourceUrl);

			if (!result)
			{
				_= Alert("URL is not valid");
				SpareSearch(website.Link);
			}


			SaveUrl = new Command(() =>
			{
				string correctUrl = CorrectUrl(webLink);
				website.Link = correctUrl.Substring(8);
				App.Current.MainPage.Navigation.PopAsync();
			});
		}

		public void GetUrl(WebNavigatingEventArgs e)
		{
			webLink = e.Url;
		}

		private void SpareSearch(string url)
		{
			SourceUrl = "https://www.google.com/search?q=" + url;
		}

		public void Navigated(object sender, WebNavigatedEventArgs e)
		{
			switch (e.Result)
			{
				case WebNavigationResult.Failure:
				case WebNavigationResult.Timeout:
					SpareSearch(website.Link);
					break;
			}
		}

		private string CorrectUrl(string url)
		{
			if (url == null)
				return null;

			if (url.StartsWith("http://")) 
			{
				url = url.Substring(4);
				url = "https" + url;
			}
			else if (!url.StartsWith("https://"))
			{
				url = "https://" + url; 
			}
			// jinak tezko opravit

			Uri result;
			if(Uri.TryCreate(url, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttps) || (result.Scheme == Uri.UriSchemeHttp))
			{
				return url;
			}
			else
			{
				_ = Alert("Incorrect website URL");
				return null;
			}
		}

		private async Task Alert(string msg)
		{
			await App.Current.MainPage.DisplayAlert("Error", msg, "Cancel");
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
