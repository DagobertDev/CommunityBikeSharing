using CommunityBikeSharing.Assets;
using CommunityBikeSharing.Controls;
using CommunityBikeSharing.iOS.Renderers;
using CommunityBikeSharing.Models;
using FontAwesome;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(OverviewMap), typeof(MapOverviewRenderer))]

namespace CommunityBikeSharing.iOS.Renderers
{
	public class MapOverviewRenderer : MapRenderer
	{
		protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
		{
			var bike = new FontImageSource
			{
				FontFamily = Fonts.SolidIcons, 
				Glyph = FontAwesomeIcons.Bicycle, 
				Color = Color.Black
			};

			var station = new FontImageSource
			{
				FontFamily = Fonts.SolidIcons, 
				Glyph = FontAwesomeIcons.Home,
			};

			var annotationView = base.GetViewForAnnotation(mapView, annotation);
			var pin = GetPinForAnnotation(annotation);

			switch (pin?.BindingContext)
			{
				case Bike _:
					annotationView.Image = GetUIImageFromImageSource(bike);
					break;
				case Station _:
					annotationView.Image = GetUIImageFromImageSource(station);
					break;
			}

			return annotationView;
		}

		private static UIImage GetUIImageFromImageSource(FontImageSource source)
		{
			var handler = new FontImageSourceHandler();
			
			// LoadImageAsync is not async, it just returns Task.FromResult
			return handler.LoadImageAsync(source).Result;
		}
	}
}
