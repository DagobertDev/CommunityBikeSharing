using Plugin.CloudFirestore.Attributes;

namespace CommunityBikeSharing.Models
{
	public class Community
	{
		[Id]
		public string Id { get; set; }
		public string Name { get; set; }
	}
}
