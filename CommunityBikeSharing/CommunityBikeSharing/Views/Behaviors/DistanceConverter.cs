using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views.Behaviors
{
	public class LocationsToDistanceConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			var firstValue = (Location)values[0];
			var secondValue = (Location)values[1];

			if (firstValue == null || secondValue == null)
			{
				return string.Empty;
			}

			return $"{firstValue.CalculateDistance(secondValue, DistanceUnits.Kilometers):0.00}km";
	}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			=> throw new NotImplementedException("Going back to what you had isn't supported.");
	}
}
