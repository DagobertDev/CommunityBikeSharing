#nullable enable
using System;
using CommunityBikeSharing.Services.Data;
using Plugin.CloudFirestore.Attributes;
using Xamarin.Essentials;

namespace CommunityBikeSharing.Models
{
	public class Station
	{
		[Id]
		public string Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		[DocumentConverter(typeof(LocationConverter))]
		public Location Location { get; set; }
		[Ignored]
		public string CommunityId { get; set; }
	}
}
