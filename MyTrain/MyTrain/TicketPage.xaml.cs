using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTrain
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TicketPage : ContentPage
    {
        public TicketPage()
        {
            InitializeComponent();
            AddSanatoriumButtons();
        }

        private void AddSanatoriumButtons()
        {
            var sanatorium1Button = new Button
            {
                Text = "Санаторий Талица",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            sanatorium1Button.Clicked += (sender, e) =>
            {
                Device.OpenUri(new System.Uri("https://sanatorii.net/rossiya/sverdlovskaya-oblast/talicza.html"));
            };

            var sanatorium2Button = new Button
            {
                Text = "Санаторий Аквамарин",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            sanatorium2Button.Clicked += (sender, e) =>
            {
                Device.OpenUri(new System.Uri("https://sanatorii.net/rossiya/krasnodarskij-kraj/akvamarin.html"));
            };

            var sanatorium3Button = new Button
            {
                Text = "Санаторий Радугаж",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            sanatorium3Button.Clicked += (sender, e) =>
            {
                Device.OpenUri(new System.Uri("https://sanatorii.net/nalchik/raduga.html"));
            };

            SanatoriumsStackLayout.Children.Add(sanatorium1Button);
            SanatoriumsStackLayout.Children.Add(sanatorium2Button);
            SanatoriumsStackLayout.Children.Add(sanatorium3Button);
        }
    }
}
