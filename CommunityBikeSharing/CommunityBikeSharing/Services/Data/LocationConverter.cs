#nullable enable
using System;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Converters;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Services.Data
{
	public class LocationConverter : DocumentConverter
	{
		public LocationConverter(Type targetType) : base(targetType)
		{
			if (targetType != typeof(Location))
			{
				throw new ArgumentException("LocationConverter only works for Location");
			}
		}

		public override bool ConvertFrom(DocumentObject value, out object? result)
		{
			if (value.Type == DocumentObjectType.GeoPoint)
			{
				var geoPoint = value.GeoPoint;
				result = new Location(geoPoint.Latitude, geoPoint.Longitude);
				return true;
			}

			result = null;
			return false;
		}

		public override bool ConvertTo(object? value, out object? result)
		{
			if (value is Location location)
			{
				result = new GeoPoint(location.Latitude, location.Longitude);
				return true;
			}

			result = null;
			return false;
		}
	}
}
