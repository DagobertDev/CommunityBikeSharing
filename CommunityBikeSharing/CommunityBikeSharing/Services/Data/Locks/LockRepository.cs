#nullable enable
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data.Locks
{
	public class LockRepository : FirestoreRepository<Lock>, ILockRepository
	{
		private readonly IFirestoreContext _context;
		private ICollectionReference Locks(string community) => _context.Locks(community);

		public LockRepository(
			IFirestoreContext contextContext)
		{
			_context = contextContext;
		}

		protected override IDocumentReference GetDocument(Lock model) => _context.Locks(model.CommunityId).Document(model.Id);
		protected override ICollectionReference GetCollection(Lock model) => _context.Locks(model.CommunityId);

		public async Task<Lock> Get(string community, string id)
		{
			var snap = await Locks(community).Document(id).GetAsync();
			var @lock = snap.ToObject<Lock>();
			@lock!.CommunityId = community;
			return @lock;
		}
	}
}
