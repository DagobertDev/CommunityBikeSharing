using System;
using System.Globalization;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views.Behaviors
{
	public class IsNotNullConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
