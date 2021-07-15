using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views.Behaviors
{
	public class LocationsToDistanceConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values.Length != 2)
			{
				return string.Empty;
			}

			if (!(values[0] is Location firstValue) || !(values[1] is Location secondValue))
			{
				return string.Empty;
			}

			var distance = firstValue.CalculateDistance(secondValue, DistanceUnits.Kilometers);

			return distance switch
			{
				< 1 => $"{distance * 1000 :F0} m",
				< 10 => $"{distance :F1} km",
				_ => $"{distance :F0} km"
			};
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			=> throw new NotImplementedException("Going back to what you had isn't supported.");
	}
}
