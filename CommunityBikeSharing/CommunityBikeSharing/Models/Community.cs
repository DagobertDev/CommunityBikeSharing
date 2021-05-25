using System;
using CommunityBikeSharing.Services.Data;
using Plugin.CloudFirestore.Attributes;

namespace CommunityBikeSharing.Models
{
	public class Community
	{
		[Id]
		public string Id { get; set; }
		public string Name { get; set; }

		[DocumentConverter(typeof(TimeSpanConverter))]
		public TimeSpan ReserveTime { get; set; } = TimeSpan.FromHours(2);
	}
}
