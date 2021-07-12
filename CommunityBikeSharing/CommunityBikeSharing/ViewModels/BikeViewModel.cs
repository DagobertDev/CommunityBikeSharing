using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Locks;
using Xamarin.Essentials;

namespace CommunityBikeSharing.ViewModels
{
	public class BikeViewModel : BaseViewModel
	{
		private readonly IBikeService _bikeService;
		private readonly ILockService _lockService;
		private readonly IDialogService _dialogService;
		private readonly ICommunityService _communityService;

		public BikeViewModel(
			IBikeService bikeService,
			ILockService lockService,
			IDialogService dialogService,
			ICommunityService communityService)
		{
			_bikeService = bikeService;
			_lockService = lockService;
			_dialogService = dialogService;
			_communityService = communityService;

			LendBikeCommand = CreateCommand<Bike>(LendBike, bikeService.CanLendBike);
			ReturnBikeCommand = CreateCommand<Bike>(ReturnBike, bikeService.CanReturnBike);
			ReserveBikeCommand = CreateCommand<Bike>(ReserveBike, bikeService.CanReserveBike);
			DeleteReservationCommand = CreateCommand<Bike>(DeleteReservation, bikeService.CanDeleteReservation);
			TakeBreakCommand = CreateCommand<Bike>(TakeBreak, CanTakeBreak);
			EndBreakCommand = CreateCommand<Bike>(EndBreak, CanEndBreak);
			ReportProblemCommand = CreateCommand<Bike>(ReportProblem, CanReportProblem);
		}
		
		public ICommand LendBikeCommand { get; }
		public ICommand ReturnBikeCommand { get; }
		public ICommand ReserveBikeCommand { get; }
		public ICommand DeleteReservationCommand { get; }
		public ICommand TakeBreakCommand { get; }
		public ICommand EndBreakCommand { get; }
		public ICommand ReportProblemCommand { get; }
		
		private Task LendBike(Bike bike) => _bikeService.LendBike(bike);
		private Task ReturnBike(Bike bike) => _bikeService.ReturnBike(bike);

		private async Task ReserveBike(Bike bike)
		{
			await _bikeService.ReserveBike(bike);

			var dateTime = bike.ReservedUntil!.Value.ToLocalTime();

			string formattedDateTime = dateTime.ToString(dateTime.Date == DateTime.Now.Date ? "HH:mm" : "dd.MM. hh:mm");

			await _dialogService.ShowMessage("Fahrrad reserviert",
				$"Das Fahrrad wurde bis {formattedDateTime} reserviert. " +
				"Es kann bis zu diesem Zeitpunkt nur von Ihnen ausgeliehen werden.");
		}

		private Task DeleteReservation(Bike bike) => _bikeService.DeleteReservation(bike);

		private Task TakeBreak(Bike bike) => _lockService.CloseLock(bike);
		private bool CanTakeBreak(Bike bike) => bike.Lent && bike.HasLock && bike.LockState != Lock.State.Closed;

		private Task EndBreak(Bike bike) => _lockService.OpenLock(bike);
		private bool CanEndBreak(Bike bike) => bike.Lent && bike.HasLock && bike.LockState != Lock.State.Open;

		private async Task ReportProblem(Bike bike)
		{
			var community = await _communityService.Get(bike.CommunityId);
			var mailAddress = community.SupportEmail;
			
			try
			{
				var message = new EmailMessage
				{
					Subject = "Problem mit Fahrrad",
					Body = $"Hallo Team von {community.Name},\n" +
					       $"es gibt folgendes Problem mit dem Fahrrad {bike.Name}: \n", 
					To = new List<string>{mailAddress}
				};

				await Email.ComposeAsync(message);
			}
			catch (Exception)
			{
				await _dialogService.ShowError("Versand fehlgeschlagen",
					"Es konnte keine Mail-App auf dem Gerät gefunden werden");
			}
		}
		
		private bool CanReportProblem(Bike bike) => true;
	}
}
