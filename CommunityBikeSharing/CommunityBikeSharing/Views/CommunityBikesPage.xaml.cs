using CommunityBikeSharing.ViewModels;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views
{
	public partial class CommunityBikesPage
	{
		private bool _initialized;

		public CommunityBikesPage()
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

		private void OnEditBike(object sender, ItemTappedEventArgs e)
		{
			var editBike = ((CommunityBikesViewModel)BindingContext).EditBikeCommand;

			if (editBike.CanExecute(e.Item))
			{
				editBike.Execute(e.Item);
			}
		}
	}
}

