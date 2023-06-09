using MyTrain.Data;
using MyTrain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace MyTrain
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutesPage : ContentPage
    {
        private DataAccess dataAccess;
        private DateTime selectedDepartureDate;
        private int selectedDepartureCityId;
        private User currentUser;
        private int selectedArrivalCityId;
        private int typeIdCoupe = 2;
        private int typeIdEconom = 1;

        public RoutesPage(DateTime departureDate, int departureCityId, int arrivalCityId, User user)
        {
            InitializeComponent();
            currentUser = user;
            dataAccess = new DataAccess();
            selectedDepartureDate = departureDate;
            selectedDepartureCityId = departureCityId;
            selectedArrivalCityId = arrivalCityId;
            LoadRoutes();
        }

        private async void LoadRoutes()
        {
            List<Route> routes = await dataAccess.GetRoutesAsync();

            foreach (var route in routes)
            {
                // Проверяем соответствие критериям маршрута
                if (route.DepartureDate.Date == selectedDepartureDate.Date &&
                    route.DepartureCityId == selectedDepartureCityId &&
                    route.ArrivalCityId == selectedArrivalCityId)
                {
                    var routeLayout = new PancakeView
                    {
                        BackgroundColor = Color.White,
                        CornerRadius = new CornerRadius(10),
                        Padding = new Thickness(10),
                        Margin = new Thickness(20, 10, 20, 0)
                    };

                    var trainId = route.TrainsId; // Получение значения TrainsId из таблицы Routes

                    var train = await dataAccess.GetTrainByIdAsync(trainId); // Получение объекта Train по его Id
                    var departureCity = await dataAccess.GetCityByIdAsync(route.DepartureCityId);
                    var arrivalCity = await dataAccess.GetCityByIdAsync(route.ArrivalCityId);
                    var trainNameLabel = new Label
                    {
                        Text = "Поезд " + train.Name,
                        FontSize = 16,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                        TextColor = Color.FromHex("#97A3E5")
                    };


                    var departureDateTimeLabel = new Label
                    {
                        Text = "Дата и время выезда: " + route.DepartureDate.ToString("g"),
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Start
                    };

                    var arrivalDateTimeLabel = new Label
                    {
                        Text = "Дата и время приезда: " + route.ArrivalDate.ToString("g"),
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Start
                    };

                    var departureCityLabel = new Label
                    {
                        Text = "Откуда: " + departureCity?.Name,
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Start
                    };

                    var arrivalCityLabel = new Label
                    {
                        Text = "Куда: " + arrivalCity?.Name,
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Start
                    };

                    var coupePriceLabel = new Label
                    {
                        Text = "Цена купе: " + route.PriceCoupe.ToString("F2"),
                        FontSize = 16,
                        HorizontalOptions = LayoutOptions.Start,
                        GestureRecognizers = { new TapGestureRecognizer { Command = new Command(async () => await NavigateToWagonsPage(trainId, typeIdCoupe, "Купе", route.Id)) } }
                    };

                    var economyPriceLabel = new Label
                    {
                        Text = "Цена эконом: " + route.PriceEconom.ToString("F2"),
                        FontSize = 16,
                        HorizontalOptions = LayoutOptions.Start,
                        GestureRecognizers = { new TapGestureRecognizer { Command = new Command(async () => await NavigateToWagonsPage(trainId, typeIdEconom, "Плацкарт", route.Id)) } }
                    };

                    var routeStackLayout = new StackLayout
                    {
                        Children = { trainNameLabel, departureDateTimeLabel, arrivalDateTimeLabel, departureCityLabel, arrivalCityLabel, coupePriceLabel, economyPriceLabel }
                    };

                    routeLayout.Content = routeStackLayout;
                    RoutesStackLayout.Children.Add(routeLayout);
                }
            }
        }

        private async Task NavigateToWagonsPage(int trainId, int typeId, string wagonType, int routeId)
        {
            // Создание новой страницы WagonsPage с передачей параметров
            var wagonsPage = new WagonsPage(trainId, wagonType, typeId, currentUser, routeId);
            await Navigation.PushAsync(wagonsPage);
        }
        private async void OnProfileTapped(object sender, EventArgs e)
        {
            // Создание новой страницы SearchPage
            var profile = new Profile(currentUser);

            // Выполнение перехода на страницу SearchPage
            await Navigation.PushAsync(profile);
        }
        private async void OnTicketTapped(object sender, EventArgs e)
        {
            var ticketPage = new TicketPage(currentUser);
            await Navigation.PushAsync(ticketPage);
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
