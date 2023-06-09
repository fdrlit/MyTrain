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
            dataAccess = new DataAccess();
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
                        Command = new Command(async () =>
                        {
                            bool proceed = await DisplayAlert("Подтверждение", "Вы уверены, что хотите приобрести билет на это место?", "Да", "Нет");
                            if (proceed)
                            {
                                await PurchasePlace(place.Id);
                            }
                        })
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
                // Update the places list in the PlacesPage instance
                var purchasedPlace = places.Find(p => p.Id == placeId);
                if (purchasedPlace != null)
                {
                    purchasedPlace.UserId = currentUser.Id;
                }

                await DisplayAlert("Успех", "Место успешно приобретено.", "OK");

                // Update the display of places on the page
                UpdatePlacesLayout();
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось приобрести место.", "OK");
            }
        }

        private void UpdatePlacesLayout()
        {
            // Clear the current places on the page
            PlacesStackLayout.Children.Clear();

            // Reload the updated places
            LoadPlaces();
        }

        private async void OnProfileTapped(object sender, EventArgs e)
        {
            var profile = new Profile(currentUser);
            await Navigation.PushAsync(profile);
        }

        private async void OnTicketTapped(object sender, EventArgs e)
        {
            var search = new TicketPage(currentUser);
            await Navigation.PushAsync(search);
        }

        private async void OnSearchTapped(object sender, EventArgs e)
        {
            var search = new Search(currentUser);
            await Navigation.PushAsync(search);
        }
        private async void OnBackButtonTapped(object sender, EventArgs e)
        {
            Page previousPage = Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];

            if (previousPage is MainPage || previousPage is Login)
            {
                await DisplayAlert("Ошибка", "Нельзя переходить к страницам авторизации и регистрации", "OK");
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

    }
}
