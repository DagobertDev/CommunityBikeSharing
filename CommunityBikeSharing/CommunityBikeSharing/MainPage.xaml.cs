using Xamarin.Forms;

namespace CommunityBikeSharing
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Label.Text = $"Wilkommen beim Community Bike Sharing, {App.User.Email}!";
        }
    }
}
