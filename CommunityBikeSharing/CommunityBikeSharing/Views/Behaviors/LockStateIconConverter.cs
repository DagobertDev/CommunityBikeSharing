using System;
using System.Globalization;
using CommunityBikeSharing.Models;
using FontAwesome;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views.Behaviors
{
	public class LockStateIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Lock.State)value switch
			{
				Lock.State.None => string.Empty,
				Lock.State.Unknown => string.Empty,
				Lock.State.Open => FontAwesomeIcons.LockOpen,
				Lock.State.Closed => FontAwesomeIcons.Lock,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
