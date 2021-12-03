using Reklamacka.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Reklamacka.Converters
{
	public class ExpColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			UserSettingsViewModel settings = new UserSettingsViewModel();
			if(settings.IsOnAlowColors == false)
				return Color.DarkGray;

			try
			{                                                                                   // expiracni lhuta vyprsi za: 
				if (DateTime.Compare((DateTime)value, DateTime.Today) < 0)                      // uz vyprsela
					return Color.DarkGray;
				else if (DateTime.Compare((DateTime)value, DateTime.Today.AddDays(21)) < 0)     // 21 dnu
					return Color.FromHex("#ddff0000");
				else if (DateTime.Compare((DateTime)value, DateTime.Today.AddMonths(2)) < 0)    // 2 mesice
					return Color.FromHex("#ddffa500");
				else if (DateTime.Compare((DateTime)value, DateTime.Today.AddMonths(3)) < 0)    // 3 mesice
					return Color.FromHex("#ddffff00");
				return Color.FromHex("#dd00ff00");
			}
			catch
			{
				return Color.White;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
