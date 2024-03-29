﻿using Plugin.CloudFirestore;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace CommunityBikeSharing.Services
{
	public static class Extensions
	{
		public static Position ToPosition(this Location location) =>
			new Position(location.Latitude, location.Longitude);

		public static Location ToLocation(this Position position) =>
			new Location(position.Latitude, position.Longitude);

		public static GeoPoint ToGeoPoint(this Location location) =>
			new GeoPoint(location.Latitude, location.Longitude);
	}
}
