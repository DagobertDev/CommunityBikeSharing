using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CommunityBikeSharing.Views.Behaviors
{
	public class PositionToLocationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Location position))
			{
				return new Position();
			}

			return new Position(position.Latitude, position.Longitude);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException("Going back to what you had isn't supported.");
	}
}
