using Plugin.CloudFirestore.Attributes;
using Plugin.CloudFirestore.Converters;

namespace CommunityBikeSharing.Models
{
	public class CommunityMembership
	{
		[Id]
		public string Id
		{
			get => GetId(CommunityId, UserId);
			set {} // Needed to prevent errors when deserializing.
		}

		public string UserId { get; set; }
		public string CommunityId { get; set; }
		public string Name { get; set; }

		[DocumentConverter(typeof(EnumStringConverter))]
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
