using System;
using CommunityBikeSharing.Services.Data;
using Plugin.CloudFirestore.Attributes;
using Plugin.CloudFirestore.Converters;
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

		[DocumentConverter(typeof(LocationConverter))]
		public Location? Location { get; set; }
		public string? StationId { get; set; }
		public DateTime? ReservedUntil { get; set; }
		public string? LockId { get; set; }

		[DocumentConverter(typeof(EnumStringConverter))]
		public Lock.State LockState { get; set; } = Lock.State.None;

		[Ignored]
		public bool Lent => CurrentUser != null && ReservedUntil == null;

		[Ignored]
		public bool Reserved => ReservedUntil >= DateTime.UtcNow;

		[Ignored]
		public bool HasLock => !string.IsNullOrEmpty(LockId);
	}
}
