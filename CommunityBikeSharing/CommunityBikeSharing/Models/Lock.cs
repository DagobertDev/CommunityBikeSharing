using Plugin.CloudFirestore.Attributes;

namespace CommunityBikeSharing.Models
{
	public class Lock
	{
		[Id]
		public string Id { get; set; }
		public string CommunityId { get; set; }
		public string Name { get; set; }
		public string Key { get; set; }

		public enum State
		{
			None,
			Unknown,
			Open,
			Closed
		}
	}
}
