namespace CommunityBikeSharing.Models
{
	public class CommunityMember
	{
		public string UserId { get; set; }
		public string CommunityId { get; set; }
		public string Name { get; set; }
		public CommunityRole Role { get; set; } = CommunityRole.User;
	}

	public enum CommunityRole
	{
		CommunityAdmin,
		User
	}
}
