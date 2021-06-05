using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using CommunityBikeSharing.Droid.Renderers;
using CommunityBikeSharing.Views;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using View = Android.Views.View;
using Android.Content;
using CommunityBikeSharing.Models;

[assembly: ExportRenderer(typeof(OverviewMap), typeof(OverviewMapRenderer))]

namespace CommunityBikeSharing.Droid.Renderers
{
	public class OverviewMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
	{
		public OverviewMapRenderer(Context context) : base(context) { }

		public View GetInfoContents(Marker marker) => null;

		public View GetInfoWindow(Marker marker) => null;

		protected override MarkerOptions CreateMarker(Pin pin)
		{
			var marker = base.CreateMarker(pin);

			switch (pin.BindingContext)
			{
				case Bike _:
					marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.bike));
					break;
				case Station _:
					marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.home));
					break;
			}

			return marker;
		}
	}
}
