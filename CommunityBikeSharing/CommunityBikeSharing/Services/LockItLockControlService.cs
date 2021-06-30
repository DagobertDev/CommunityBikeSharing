using System;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services.Data.Locks;
using Plugin.BluetoothLE;

namespace CommunityBikeSharing.Services
{
	public class LockItLockControlService : ILockControlService
	{
		private static readonly TimeSpan MaxTurnOnBluetoothDuration = TimeSpan.FromSeconds(5);
		private static readonly TimeSpan MaxScanDuration = TimeSpan.FromSeconds(12);
		private static readonly TimeSpan MaxAuthDuration = TimeSpan.FromSeconds(3);
		private static readonly TimeSpan OpenCloseLockDuration = TimeSpan.FromSeconds(10);

		private readonly ILockKeyService _lockKeyService;
		private readonly IDialogService _dialogService;

		public LockItLockControlService(
			ILockKeyService lockKeyService,
			IDialogService dialogService)
		{
			_lockKeyService = lockKeyService;
			_dialogService = dialogService;
		}

		private readonly IAdapter _adapter = CrossBleAdapter.Current;

		private static readonly Guid LockControlServiceGuid = Guid.Parse("0000f00d-1212-efde-1523-785fef13d123");

		private static readonly Guid AuthenticationGuid = Guid.Parse("0000baab-1212-efde-1523-785fef13d123");
		private static readonly Guid AuthenticationStatusGuid = Guid.Parse("0000babc-1212-efde-1523-785fef13d123");
		private static readonly Guid ControlLockGuid = Guid.Parse("0000beee-1212-efde-1523-785fef13d123");
		private static readonly Guid LockStateGuid = Guid.Parse("0000baaa-1212-efde-1523-785fef13d123");

		private static Aes CreateAes(LockKey key) => new AesManaged
		{
			Key = key.UserKey,
			Mode = CipherMode.ECB,
			Padding = PaddingMode.Zeros,
		};

		private async Task<IDevice?> Initialize(Lock @lock, LockKey key)
		{
			if (_adapter.Status != AdapterStatus.PoweredOn)
			{
				if (!(_adapter.CanControlAdapterState() && await TurnOnBle()))
				{
					await _dialogService.ShowError("Bluetooth anschalten",
						"Schalten Sie Bluetooth ein, um das Schloss zu bedienen");
					return null;
				}

				Task<bool> TurnOnBle()
				{
					TaskCompletionSource<bool> bleResult = new TaskCompletionSource<bool>();

					_adapter.WhenStatusChanged()
						.Timeout(MaxTurnOnBluetoothDuration)
						.Subscribe(status =>
						{
							if (status == AdapterStatus.PoweredOn)
							{
								bleResult.TrySetResult(true);
							}
						}, exception =>
						{
							bleResult.TrySetResult(false);
						});

					_adapter.SetAdapterState(true);
					return bleResult.Task;
				}
			}

			var result = new TaskCompletionSource<IDevice?>();

			_adapter.ScanUntilDeviceFound(@lock.Name)
				.Take(1)
				.Timeout(MaxScanDuration)
				.Subscribe(device =>
				{
					device.WhenConnected().Take(1).Subscribe(async _ =>
					{
						var success = await Authenticate(key, device);

						if (success)
						{
							result.TrySetResult(device);
						}
						else
						{
							result.TrySetResult(null);
							await _dialogService.ShowError("Authentifizierung fehlgeschlagen",
								"Das Schloss konnte aufgrund eines Fehlers mit der Authentifizierung nicht geöffnet werden.");
						}
					}, async exception =>
					{
						result.TrySetResult(null);
						await _dialogService.ShowError("Verbindung fehlgeschlagen",
							"Die Verbindung zum Schloss ist fehlgeschlagen, obwohl das Schloss gefunden wurde.");
					});

					device.Connect(new ConnectionConfig
					{
						AutoConnect = false
					});

				}, async exception =>
				{
					await _dialogService.ShowError("Schloss wurde nicht gefunden",
						"Das Schloss konnte nicht gefunden werden.");
					result.TrySetResult(null);
				});

			return await result.Task;
		}

		// See ILockIt API documentation
		private async Task<bool> Authenticate(LockKey key, IDevice device)
		{
			var result = new TaskCompletionSource<bool>();

			var lockControlService = await device.GetKnownService(LockControlServiceGuid);

			{
				var authStatus = await lockControlService.GetKnownCharacteristics(AuthenticationStatusGuid);

				authStatus.WhenNotificationReceived()
					.Take(1)
					.Timeout(MaxAuthDuration)
					.Subscribe(notification =>
					{
						result.TrySetResult(notification.Data[0] == 0);
					}, exception =>
					{
						result.TrySetResult(false);
					});

				await authStatus.EnableNotifications();
			}

			var authenticationCharacteristic = await lockControlService.GetKnownCharacteristics(AuthenticationGuid);
			authenticationCharacteristic.WhenNotificationReceived().Take(1).Subscribe(async notification =>
			{
				var challenge = notification.Data;

				var crypto = CreateAes(key);

				challenge = crypto.CreateDecryptor().TransformFinalBlock(challenge, 0, challenge.Length);

				challenge[^1] += 1;

				challenge = crypto.CreateEncryptor().TransformFinalBlock(challenge, 0, challenge.Length);

				await authenticationCharacteristic.Write(challenge);
			});

			await authenticationCharacteristic.EnableNotifications();
			await authenticationCharacteristic.Write(key.Seed);

			return await result.Task;
		}

		private async Task<bool> OpenCloseLock(Lock @lock, bool open)
		{
			var keys = await _lockKeyService.Get(@lock);

			var device = await Initialize(@lock, keys);

			if (device == null)
			{
				return false;
			}

			var service = await device.GetKnownService(LockControlServiceGuid);
			var openCloseCharacteristic = await service.GetKnownCharacteristics(ControlLockGuid);
			var lockState = await service.GetKnownCharacteristics(LockStateGuid);

			var currentValue = await lockState.Read();

			// Check if new status is the expected one
			if ((currentValue.Data[0] == 0 && open) || (currentValue.Data[0] == 1 && !open))
			{
				device.CancelConnection();
				return true;
			}

			var data = open ? OpenPackage() : ClosePackage();

			data = CreateAes(keys).CreateEncryptor().TransformFinalBlock(data, 0, data.Length);

			var statusChange = new TaskCompletionSource<bool>();

			lockState.WhenNotificationReceived()
				.Timeout(OpenCloseLockDuration)
				.Subscribe(notification =>
				{
					// Check if new lock state is as expected. 0 == open, 1 == closed
					statusChange.TrySetResult((notification.Data[0] == 0 && open) || (notification.Data[0] == 1 && !open));
				}, exception =>
				{
					statusChange.TrySetResult(false);
				});

			await lockState.EnableNotifications();

			await openCloseCharacteristic.Write(data);

			var result = await statusChange.Task;

			device.CancelConnection();

			return result;
		}

		public Task<bool> OpenLock(Lock @lock) => OpenCloseLock(@lock, true);
		public Task<bool> CloseLock(Lock @lock) => OpenCloseLock(@lock, false);

		private byte _counter;
		private byte[] OpenPackage()
		{
			var data = new byte[16];

			_counter++;
			data[1] = _counter;
			data[0] = _counter;

			data[2] = 1;
			return data;
		}

		private byte[] ClosePackage()
		{
			var data = new byte[16];

			_counter++;
			data[1] = _counter;
			data[0] = _counter;

			data[2] = 2;
			return data;
		}
	}
}
