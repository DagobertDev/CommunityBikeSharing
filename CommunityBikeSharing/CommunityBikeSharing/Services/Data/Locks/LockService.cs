#nullable enable
using System;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data.Bikes;

namespace CommunityBikeSharing.Services.Data.Locks
{
	public class LockService : ILockService
	{
		private readonly IFirestoreContext _context;
		private readonly ILockRepository _lockRepository;
		private readonly IBikeRepository _bikeRepository;
		private readonly ILockControlService _lockControlService;

		public LockService(
			IFirestoreContext context,
			ILockRepository lockRepository,
			IBikeRepository bikeRepository,
			ILockControlService lockControlService)
		{
			_context = context;
			_lockRepository = lockRepository;
			_bikeRepository = bikeRepository;
			_lockControlService = lockControlService;
		}

		public Task<Lock> Get(Bike bike)
		{
			var community = bike.CommunityId;
			var lockId = bike.LockId;

			if (lockId == null)
			{
				throw new NullReferenceException(nameof(Bike.LockId));
			}

			return _lockRepository.Get(community, lockId);
		}

		public async Task Add(Bike bike, string name, string key)
		{
			var @lock = new Lock
			{
				Name = name,
				Key = key,
				CommunityId = bike.CommunityId
			};

			@lock = await _lockRepository.Add(@lock);

			await _bikeRepository.Update(bike, nameof(Bike.LockId), @lock.Id);
		}

		public Task Remove(Bike bike)
		{
			var @lock = new Lock
			{
				Id = bike.LockId ?? throw new NullReferenceException(nameof(Bike.LockId)),
				CommunityId = bike.CommunityId,
			};

			return _context.RunTransactionAsync(transaction =>
			{
				_bikeRepository.Update(bike, nameof(Bike.LockId), null);
				_lockRepository.Delete(@lock, transaction);
			});
		}

		public async Task<bool> OpenLock(Bike bike)
		{
			var @lock = await Get(bike);
			return await _lockControlService.OpenLock(@lock);
		}

		public async Task<bool> CloseLock(Bike bike)
		{
			var @lock = await Get(bike);
			return await _lockControlService.CloseLock(@lock);
		}
	}
}
