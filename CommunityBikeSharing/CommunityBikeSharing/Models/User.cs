using Plugin.CloudFirestore.Attributes;

namespace CommunityBikeSharing.Models
{
	public class User
	{
		[Id]
		public string Id { get; set; }
		public string Username { get; set; }
	}
}
