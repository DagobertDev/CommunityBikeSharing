using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunityStationsPage
	{
		private bool _initialized;

		public CommunityStationsPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (!_initialized)
			{
				_initialized = true;
				await ((BaseViewModel)BindingContext).InitializeAsync();
			}
		}

		private void OnEditStation(object sender, ItemTappedEventArgs e)
		{
			var editStation = ((CommunityStationsViewModel)BindingContext).EditStationCommand;

			if (editStation.CanExecute(e.Item))
			{
				editStation.Execute(e.Item);
			}
		}
	}
}

