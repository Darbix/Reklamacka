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
			try
			{
				if (DateTime.Compare((DateTime)value, DateTime.Today) < 0) 
					return Color.DarkGray;
				else if (DateTime.Compare((DateTime)value, DateTime.Today.AddDays(21)) < 0)
					return Color.Red;
				else if (DateTime.Compare((DateTime)value, DateTime.Today.AddMonths(2)) < 0)
					return Color.Orange;
				else if (DateTime.Compare((DateTime)value, DateTime.Today.AddMonths(3)) < 0)
					return Color.Yellow;
				return Color.Lime;
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
