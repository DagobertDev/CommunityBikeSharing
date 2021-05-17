#nullable enable
using CommunityBikeSharing.Services.Data;
using Plugin.CloudFirestore.Attributes;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Models
{
	public class Bike
	{
		[Id]
		public string Id { get; set; }
		[Ignored]
		public string CommunityId { get; set; }
		public string Name { get; set; }
		public string? CurrentUser { get; set; }
		[Ignored]
		public bool InUse => CurrentUser != null;

		[DocumentConverter(typeof(LocationConverter))]
		public Location? Location { get; set; }
		public string StationId { get; set; }
	}
}
