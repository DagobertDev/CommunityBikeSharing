using Plugin.CloudFirestore.Attributes;

namespace CommunityBikeSharing.Models
{
	public class Bike
	{
		[Id]
		public string Id { get; set; }
		[Ignored]
		public string CommunityId { get; set; }
		public string Name { get; set; }
	}
}
