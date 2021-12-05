/**
 * @brief View model of UserSettingsPage
 * 
 * @file UserSettingsViewModel.cs
 * @author Do Hung (xdohun00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Reklamacka.Pages;

namespace Reklamacka.ViewModels
{
	public class UserSettingsViewModel: INotifyPropertyChanged
	{
		public string GitHubLink { get; private set; } = @"https://github.com/Darbix/Reklamacka";
		public string Authors { get; private set; } = "Kedra David (xkedra00)\nDo Hung (xdohun00)";

		/// <summary>
		/// Resets settings
		/// </summary>
		public Command ResetSettings { get; set; }

		/// <summary>
		/// Open repo in default web browser
		/// </summary>
		public Command ShowRepoPage { get; set; }

		/// <summary>
		/// Enable auto-delete
		/// </summary>
		public bool IsOnAutoDelete 
		{
			get => Preferences.Get("AutoDelete", false);
			set
			{
				Preferences.Set("AutoDelete", value);
				OnPropertyChanged(nameof(IsOnAutoDelete));
			}
		}

		/// <summary>
		/// Enable notification
		/// </summary>
		public bool IsOnNotifications
		{
			get => Preferences.Get("Notifications", false);
			set
			{

				if (IsOnNotifications == false && value == true)
					_ = ShowNotif();

				Preferences.Set("Notifications", value);
				OnPropertyChanged(nameof(IsOnNotifications));
			}
		}

		private async Task<bool> ShowNotif()
		{
			var notification = new NotificationRequest
			{
				ReturningData = "Notifications enabled",
				Title = "Notifications have been successfully enabled",
				Schedule = { NotifyTime = DateTime.Now.AddSeconds(2)}
			};

			return await NotificationCenter.Current.Show(notification);
		}

		/// <summary>
		/// Enable color 
		/// </summary>
		public bool IsOnAlowColors
		{
			get => Preferences.Get("AlowColors", true);
			set
			{
				Preferences.Set("AlowColors", value);
				OnPropertyChanged(nameof(IsOnAlowColors));
			}
		}

		public UserSettingsViewModel()
		{
			ResetSettings = new Command(async () =>
			{
				if (!await App.Current.MainPage.DisplayAlert("Reset settings", "Settings will be restored to default", "OK", "Cancel"))
					return;

				Preferences.Clear();

				// aktualizace nastaveni v okne -> zavolaji se OnPropertyChanged
				IsOnAutoDelete = IsOnAutoDelete;
				IsOnNotifications = IsOnNotifications;
				IsOnAlowColors = IsOnAlowColors;
			});

			ShowRepoPage = new Command(async () =>
			{
				await Launcher.TryOpenAsync(GitHubLink);
			});
		}


		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
