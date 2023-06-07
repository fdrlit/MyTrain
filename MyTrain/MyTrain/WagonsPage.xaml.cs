using MyTrain.Data;
using MyTrain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTrain
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WagonsPage : ContentPage
    {
        private DataAccess dataAccess;
        private int trainId;
        private int typeId;
        private string wagonType;
        private User currentUser;
        private int routeId;

        public WagonsPage(int trainId, string wagonType, int typeId, User currentUser, int routeId)
        {
            InitializeComponent();
            dataAccess = new DataAccess();
            this.trainId = trainId;
            this.wagonType = wagonType;
            this.typeId = typeId;
            this.currentUser = currentUser;
            this.routeId = routeId; // Добавлено значение routeId
            LoadWagons();
        }

        private async void LoadWagons()
        {
            List<Wagon> wagons = await dataAccess.GetWagonsByTrainIdAndTypeAsync(trainId, typeId);

            foreach (var wagon in wagons)
            {
                var wagonLayout = new StackLayout
                {
                    BackgroundColor = Color.White,
                    Margin = new Thickness(20, 10, 20, 0),
                    Padding = new Thickness(10),
                    Orientation = StackOrientation.Horizontal
                };

                var wagonNameLabel = new Label
                {
                    Text = "Вагон: " + wagon.Name,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    TextColor = Color.FromHex("#97A3E5")
                };

                var wagonCountLabel = new Label
                {
                    Text = "Количество мест: " + wagon.Count,
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.End
                };

                wagonLayout.Children.Add(wagonNameLabel);
                wagonLayout.Children.Add(wagonCountLabel);

                wagonLayout.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () => await NavigateToPlacesPage(wagon.Id, routeId)) // Добавлено значение routeId
                });

                WagonsStackLayout.Children.Add(wagonLayout);
            }
        }

        private async Task NavigateToPlacesPage(int wagonId, int routeId)
        {
            List<Place> places = await dataAccess.GetPlacesByWagonIdAsync(wagonId);
            var placesPage = new PlacesPage(places, currentUser, wagonId, routeId);
            await Navigation.PushAsync(placesPage);
        }
        private async void OnProfileTapped(object sender, EventArgs e)
        {
            // Создание новой страницы SearchPage
            var profile = new Profile(currentUser);

            // Выполнение перехода на страницу SearchPage
            await Navigation.PushAsync(profile);
        }
    }
}
