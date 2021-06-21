using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommunityBikeSharing.Licenses;

namespace CommunityBikeSharing.ViewModels
{
	public class LicenseDetailViewModel : BaseViewModel
	{
		private License License { get; }

		public LicenseDetailViewModel(string licensePath)
		{
			License = new License(licensePath);
		}

		public string Name => License.Name;

		private string _text = string.Empty;
		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				OnPropertyChanged();
			}
		}

		public override Task InitializeAsync()
		{
			var assembly = Assembly.GetAssembly(typeof(LicensesViewModel));

			using var reader = new StreamReader(assembly.GetManifestResourceStream(License.Path) ?? Stream.Null);
			Text = reader.ReadToEnd();

			return Task.CompletedTask;
		}
	}
}
