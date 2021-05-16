using System;
using System.Globalization;
using CommunityBikeSharing.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CommunityBikeSharing.Views.Behaviors
{
	public class PositionToLocationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Location location))
			{
				return default(Position);
			}

			return location.ToPosition();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException("Going back to what you had isn't supported.");
	}
}
