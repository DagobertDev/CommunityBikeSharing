using System;

namespace CommunityBikeSharing.Models
{
	public class AuthError : Exception
	{
		public AuthError(AuthErrorReason reason)
		{
			Reason = reason;
		}

		public AuthError(AuthErrorReason reason, Exception innerException) : base(reason.ToString(), innerException)
		{
			Reason = reason;
		}

		public AuthErrorReason Reason { get; }

		public enum AuthErrorReason
		{
			MissingPassword,
			MissingEmail,
			UnknownEmailAddress,
			WrongPassword,
			Undefined,
			InvalidEmailAddress,
			EmailAlreadyUsed,
			WeakPassword,
		}
	}
}
