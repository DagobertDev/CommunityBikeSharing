#nullable enable
using System;
using CommunityBikeSharing.Models;
using Xamarin.Forms;

namespace CommunityBikeSharing.Views.Behaviors
{
	public class MapItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate? StationTemplate { get; set; }
		public DataTemplate? BikeTemplate { get; set; }

		protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
		{
			return item switch
			{
				Station _ => StationTemplate,
				Bike _ => BikeTemplate,
				_ => throw new NotSupportedException()
			};
		}
	}
}
