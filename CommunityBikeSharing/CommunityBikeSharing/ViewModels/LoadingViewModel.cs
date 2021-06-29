namespace CommunityBikeSharing.ViewModels
{
	public class LoadingViewModel : BaseViewModel
	{
		public LoadingViewModel(string text)
		{
			Text = text;
		}

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
	}
}
