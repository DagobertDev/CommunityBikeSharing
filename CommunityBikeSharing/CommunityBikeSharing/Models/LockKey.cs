namespace CommunityBikeSharing.Models
{
	public class LockKey
	{
		public LockKey(byte[] seed, byte[] userKey)
		{
			Seed = seed;
			UserKey = userKey;
		}

		public byte[] Seed { get; }
		public byte[] UserKey { get;  }
	}
}
