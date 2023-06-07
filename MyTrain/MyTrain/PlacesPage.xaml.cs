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
    public partial class PlacesPage : ContentPage
    {
        private DataAccess dataAccess;
        private List<Place> places;
        private User currentUser;
        private int wagonId;
        private int routeId;

        public PlacesPage(List<Place> places, User currentUser, int wagonId, int routeId)
        {
            InitializeComponent();
            this.places = places;
            this.currentUser = currentUser;
            this.wagonId = wagonId;
            this.routeId = routeId;
            dataAccess = new DataAccess(); // Создание экземпляра класса DataAccess
            LoadPlaces();
        }

        private void LoadPlaces()
        {
            foreach (var place in places)
            {
                var placeLayout = new StackLayout
                {
                    BackgroundColor = Color.White,
                    Margin = new Thickness(20, 10, 20, 0),
                    Padding = new Thickness(10),
                    Orientation = StackOrientation.Horizontal
                };

                var placeNameLabel = new Label
                {
                    Text = "Место: " + place.Name,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    TextColor = Color.FromHex("#97A3E5")
                };

                var placeStatusLabel = new Label
                {
                    Text = place.UserId.HasValue ? "Продано" : "Доступно",
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.End
                };

                placeLayout.Children.Add(placeNameLabel);
                placeLayout.Children.Add(placeStatusLabel);

                if (!place.UserId.HasValue)
                {
                    placeLayout.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await PurchasePlace(place.Id))
                    });
                }

                PlacesStackLayout.Children.Add(placeLayout);
            }
        }

        private async Task PurchasePlace(int placeId)
        {
            bool success = await dataAccess.PurchasePlaceAsync(currentUser, placeId, wagonId, routeId);

            if (success)
            {
                await DisplayAlert("Успех", "Место успешно приобретено.", "OK");
                // Обновление страницы или выполнение других действий
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось приобрести место.", "OK");
                // Дополнительная обработка ошибки
            }
        }
    }
}
