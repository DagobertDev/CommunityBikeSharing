namespace CommunityBikeSharing.Models
{
	public class User
	{
		public string Email { get; set; }

		private string _username;

		public string Username
		{
			get => _username ?? Email;
			set => _username = value;
		}
	}
}
