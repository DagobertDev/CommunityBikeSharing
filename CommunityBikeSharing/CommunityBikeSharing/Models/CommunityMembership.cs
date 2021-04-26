using Plugin.CloudFirestore.Attributes;

namespace CommunityBikeSharing.Models
{
	public class CommunityMembership
	{
		[Id]
		public string Id => GetId(CommunityId, UserId);
		public string UserId { get; set; }
		public string CommunityId { get; set; }
		public string Name { get; set; }
		public CommunityRole Role { get; set; } = CommunityRole.User;

		public static string GetId(Community community, User user) => GetId(community.Id, user.Id);
		public static string GetId(string communityId, string userId) => $"{userId}_{communityId}";
	}

	public enum CommunityRole
	{
		CommunityAdmin,
		User
	}
}
