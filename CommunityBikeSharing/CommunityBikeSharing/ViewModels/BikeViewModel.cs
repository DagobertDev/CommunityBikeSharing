using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;
using CommunityBikeSharing.Services.Data.Bikes;
using CommunityBikeSharing.Services.Data.Communities;
using CommunityBikeSharing.Services.Data.Locks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CommunityBikeSharing.ViewModels
{
	public class BikeViewModel : BaseViewModel
	{
		public static IEnumerable<object> GetNavigationParameters() => Enumerable.Empty<object>();
		
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

			LendBikeCommand = new Command<Bike>(LendBike, bikeService.CanLendBike);
			ReturnBikeCommand = new Command<Bike>(ReturnBike, bikeService.CanReturnBike);
			ReserveBikeCommand = new Command<Bike>(ReserveBike, bikeService.CanReserveBike);
			DeleteReservationCommand = new Command<Bike>(DeleteReservation, bikeService.CanDeleteReservation);
			TakeBreakCommand = new Command<Bike>(TakeBreak, CanTakeBreak);
			EndBreakCommand = new Command<Bike>(EndBreak, CanEndBreak);
			ReportProblemCommand = new Command<Bike>(async bike => await ReportProblem(bike), CanReportProblem);
		}
		
		public Command<Bike> LendBikeCommand { get; }
		public Command<Bike> ReturnBikeCommand { get; }
		public Command<Bike> ReserveBikeCommand { get; }
		public Command<Bike> DeleteReservationCommand { get; }
		public Command<Bike> TakeBreakCommand { get; }
		public Command<Bike> EndBreakCommand { get; }
		public Command<Bike> ReportProblemCommand { get; }
		
		private async void LendBike(Bike bike)
		{
			await _bikeService.LendBike(bike);
		}

		private async void ReturnBike(Bike bike)
		{
			await _bikeService.ReturnBike(bike);
		}

		private async void ReserveBike(Bike bike)
		{
			await _bikeService.ReserveBike(bike);

			var dateTime = bike.ReservedUntil!.Value.ToLocalTime();

			string formattedDateTime = dateTime.ToString(dateTime.Date == DateTime.Now.Date ? "HH:mm" : "dd.MM, hh:mm");

			await _dialogService.ShowMessage("Fahrrad reserviert",
				$"Das Fahrrad wurde bis {formattedDateTime} reserviert. " +
				"Es kann bis zu diesem Zeitpunkt nur von Ihnen ausgeliehen werden.");
		}

		private async void DeleteReservation(Bike bike)
		{
			await _bikeService.DeleteReservation(bike);
		}

		private async void TakeBreak(Bike bike)
		{
			await _lockService.CloseLock(bike);
		}

		private bool CanTakeBreak(Bike bike) => bike.Lent && bike.HasLock && bike.LockState != Lock.State.Closed;

		private async void EndBreak(Bike bike)
		{
			await _lockService.OpenLock(bike);
		}

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
