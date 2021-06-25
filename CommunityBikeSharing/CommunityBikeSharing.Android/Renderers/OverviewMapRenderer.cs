using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using CommunityBikeSharing.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using View = Android.Views.View;
using Android.Content;
using CommunityBikeSharing.Assets;
using CommunityBikeSharing.Controls;
using CommunityBikeSharing.Models;
using FontAwesome;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OverviewMap), typeof(OverviewMapRenderer))]

namespace CommunityBikeSharing.Droid.Renderers
{
	public class OverviewMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
	{
		private BitmapDescriptor _bikeBitmapDescriptor;
		private BitmapDescriptor _stationBitmapDescriptor;
		
		public OverviewMapRenderer(Context context) : base(context)
		{

			Task.Run(async () =>
			{
				var bikeSource = new FontImageSource
				{
					FontFamily = Fonts.SolidIcons, 
					Glyph = FontAwesomeIcons.Bicycle, 
					Color = Color.Black
				};
				
				var stationSource = new FontImageSource
				{
					FontFamily = Fonts.SolidIcons, 
					Glyph = FontAwesomeIcons.Home, 
					Color = Color.Black
				};

				var loader = new FontImageSourceHandler();
				_bikeBitmapDescriptor = BitmapDescriptorFactory.FromBitmap(await loader.LoadImageAsync(bikeSource, context));
				_stationBitmapDescriptor = BitmapDescriptorFactory.FromBitmap(await loader.LoadImageAsync(stationSource, context));
			});
		}

		public View GetInfoContents(Marker marker) => null;

		public View GetInfoWindow(Marker marker) => null;

		protected override MarkerOptions CreateMarker(Pin pin)
		{
			var marker = base.CreateMarker(pin);

			switch (pin.BindingContext)
			{
				case Bike _:
					marker.SetIcon(_bikeBitmapDescriptor);
					break;
				case Station _:
					marker.SetIcon(_stationBitmapDescriptor);
					break;
			}

			return marker;
		}
	}
}
