namespace CommunityBikeSharing.Models
{
	public class User
	{
		public string Email { get; set; }

		private string _username;

		public string Username
		{
			get => string.IsNullOrEmpty(_username) ? Email : _username;
			set => _username = value;
		}

		public string Id { get; set; }
	}
}
