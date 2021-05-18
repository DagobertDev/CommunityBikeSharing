#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data.Users
{
	public class UserEmailRepository : FirestoreRepository<UserEmail>, IUserEmailRepository
	{
		private readonly IFirestoreContext _context;
		private ICollectionReference UserEmails => _context.UserEmails;

		public UserEmailRepository(IFirestoreContext context)
		{
			_context = context;
		}

		public async Task<UserEmail?> Get(string email)
		{
			var snapShot = await UserEmails.Document(email).GetAsync();
			return snapShot.ToObject<UserEmail>();
		}

		protected override IDocumentReference GetDocument(UserEmail email) => UserEmails.Document(email.Id);
		protected override IDocumentReference GetNewDocument(UserEmail email) => UserEmails.Document(email.Id);
		protected override ICollectionReference GetCollection(UserEmail email) => UserEmails;
	}
}
