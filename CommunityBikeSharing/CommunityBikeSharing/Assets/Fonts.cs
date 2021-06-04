using CommunityBikeSharing.Assets;
using Xamarin.Forms;

[assembly: ExportFont("Font Awesome 5 Free-Regular-400.otf", Alias = Fonts.RegularIcons)]
[assembly: ExportFont("Font Awesome 5 Free-Solid-900.otf", Alias = Fonts.SolidIcons)]

namespace CommunityBikeSharing.Assets
{
	public static class Fonts
	{
		public const string RegularIcons = nameof(RegularIcons);
		public const string SolidIcons = nameof(SolidIcons);
	}
}
