#nullable enable
using System;

namespace CommunityBikeSharing.Models.Extensions
{
	public static class BikeExtensions
	{
		public static bool IsFreeFloating(this Bike bike) => !bike.Lent && bike.StationId == null;
		public static bool IsReserved(this Bike bike) => bike.ReservedUntil != null && bike.ReservedUntil > DateTime.Now;
	}
}
