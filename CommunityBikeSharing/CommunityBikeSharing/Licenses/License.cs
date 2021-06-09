namespace CommunityBikeSharing.Licenses
{
	public class License
	{
		public License(string path)
		{
			Path = path;
		}

		public string Name => Path.Replace("CommunityBikeSharing.Licenses.", "")
			.Replace(".LICENSE", "");
		public string Path { get; }
	}
}
