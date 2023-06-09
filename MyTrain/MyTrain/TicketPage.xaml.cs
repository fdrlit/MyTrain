using MyTrain.Data;
using MyTrain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTrain
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TicketPage : ContentPage
    {
        private User currentUser;
        private DataAccess dataAccess;

        public TicketPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            dataAccess = new DataAccess();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTickets();
        }
        private async void OnSearchTapped(object sender, EventArgs e)
        {
            var searchPage = new Search(currentUser);
            await Navigation.PushAsync(searchPage);
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
        private async void OnProfileTapped(object sender, EventArgs e)
        {
            var profilePage = new Profile(currentUser);
            await Navigation.PushAsync(profilePage);
        }
        private async void LoadTickets()
        {
            List<Ticket> tickets = await dataAccess.GetUserTicketsAsync(currentUser.Id);

            foreach (var ticket in tickets)
            {
                var routeId = ticket.RouteId;
                var wagonId = ticket.WagonId;
                var placeId = ticket.PlaceId;
                var userId = ticket.UserId;

                var wagonName = await dataAccess.GetWagonNameAsync(wagonId);
                //var placeNumber = await dataAccess.GetPlaceNumberAsync(placeId, currentUser.Id);
                var userName = await dataAccess.GetUserNameAsync(userId);

                var ticketLayout = new StackLayout();

                var wagonLabel = new Label { Text = "Вагон: " + wagonName };
                ticketLayout.Children.Add(wagonLabel);

                //var placeLabel = new Label { Text = "Место: " + placeNumber };
                //ticketLayout.Children.Add(placeLabel);

                var userLabel = new Label { Text = "Имя покупателя: " + userName };
                ticketLayout.Children.Add(userLabel);

                var separator = new BoxView { Color = Color.Gray, HeightRequest = 1 };
                ticketLayout.Children.Add(separator);

                TicketsStackLayout.Children.Add(ticketLayout);
            }
        }
    }
}
