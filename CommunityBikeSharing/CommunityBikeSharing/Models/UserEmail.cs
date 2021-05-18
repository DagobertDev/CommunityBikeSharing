using Plugin.CloudFirestore.Attributes;

namespace CommunityBikeSharing.Models
{
	public class UserEmail
	{
		[Id]
		public string Id { get; set; }
		public string UserId { get; set; }
	}
}
