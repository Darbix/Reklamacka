using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Reklamacka.ViewModels
{
	public class UserSettingsViewModel: INotifyPropertyChanged
	{
		public Command ResetSettings { get; set; }

		public bool IsOnAutoDelete 
		{
			get => Preferences.Get("AutoDelete", false);
			set
			{
				Preferences.Set("AutoDelete", value);
				OnPropertyChanged(nameof(IsOnAutoDelete));
			}
		}
		public bool IsOnNotifications
		{
			get => Preferences.Get("Notifications", false);
			set
			{
				Preferences.Set("Notifications", value);
				OnPropertyChanged(nameof(IsOnNotifications));
			}
		}
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
			ResetSettings = new Command(() =>
			{
				Preferences.Clear();

				// aktualizace nastaveni v okne -> zavolaji se OnPropertyChanged
				IsOnAutoDelete = IsOnAutoDelete;
				IsOnNotifications = IsOnNotifications;
				IsOnAlowColors = IsOnAlowColors;
			});
		}


		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
