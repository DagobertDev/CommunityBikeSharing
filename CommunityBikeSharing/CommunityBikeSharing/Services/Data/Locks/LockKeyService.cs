using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;

namespace CommunityBikeSharing.Services.Data.Locks
{
	// TODO: Move this to the backend
	public class LockKeyService : ILockKeyService
	{
		public Task<LockKey> Get(Lock @lock) => Task.FromResult(GenerateKeys(@lock.Key));

		private static LockKey GenerateKeys(string secret)
		{
			// 1.Get internal key of the lock
			var internKey = StringToByteArray(secret);

			// 2.Create array with 16 Byte and fill with random values:
			var seed = new byte[16];

			var random = new Random();
			random.NextBytes(seed);

			// 3.Concatenate internKey and seed:
			var outputStream = new MemoryStream();
			outputStream.Write(seed);
			outputStream.Write(internKey);

			var tempKey = outputStream.ToArray();

			// 4.Calculate SHA-1 of tempKey and fill with zeros
			using var sha1 = SHA1.Create();

			var adminKey = sha1.ComputeHash(tempKey);
			var filler = new byte[4];
			outputStream = new MemoryStream();
			outputStream.Write(adminKey);
			outputStream.Write(filler);

			adminKey = outputStream.ToArray();

			// 5.Calculate SHA-1 of adminKey and fill with zeros
			var userKey = sha1.ComputeHash(adminKey);
			outputStream = new MemoryStream();
			outputStream.Write(userKey);
			outputStream.Write(filler);

			userKey = outputStream.ToArray();

			return new LockKey(seed, userKey);
		}

		private static byte[] StringToByteArray(string hex)
		{
			try
			{
				return Enumerable.Range(0, hex.Length)
					.Where(x => x % 2 == 0)
					.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
					.ToArray();
			}
			catch
			{
				return Array.Empty<byte>();
			}
		}
	}
}
