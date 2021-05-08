using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityBikeSharing.Models;
using CommunityBikeSharing.Services;

namespace CommunityBikeSharing.ViewModels
{
	public class OverviewViewModel : BaseViewModel
	{
		private readonly IBikeService _bikeService;

		private readonly NotifyCollectionChangedEventHandler _bikesChanged;

		private ObservableCollection<Bike> _allBikes;
		public ObservableCollection<Bike> AllBikes
		{
			get => _allBikes;
			set
			{
				if (_allBikes != null)
				{
					_allBikes.CollectionChanged -= _bikesChanged;
				}

				if (value != null)
				{
					value.CollectionChanged += _bikesChanged;
				}

				_allBikes = value;
				OnPropertyChanged();
				_bikesChanged.Invoke(null, null);
			}
		}

		public IEnumerable<Bike> BikesSorted => AllBikes?
			.OrderBy(bike => bike.Name);

		public OverviewViewModel(IBikeService bikeService)
		{
			_bikeService = bikeService;

			_bikesChanged = (sender, args) =>
			{
				OnPropertyChanged(nameof(BikesSorted));
				OnPropertyChanged(nameof(SummaryVisible));
			};
		}

		public string Heading => "Verfügbare Fahrräder:";
		public string Summary => "Zurzeit sind keine Fahrräder verfügbar. " +
		                         "Treten Sie einer Community bei, um Fahrräder auszuleihen.";

		public bool SummaryVisible => AllBikes?.Count == 0;

		public override Task InitializeAsync()
		{
			AllBikes = _bikeService.GetAvailableBikes();

			return Task.CompletedTask;
		}
	}
}
