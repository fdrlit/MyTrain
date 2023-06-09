using MyTrain.Data;
using MyTrain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTrain
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Search : ContentPage
    {
        private User currentUser;
        public DateTime SelectedDepartureDate { get; set; }
        public int SelectedDepartureCityId { get; set; }
        public int SelectedArrivalCityId { get; set; }
        private DataAccess dataAccess;

        public Search(User user)
        {
            InitializeComponent();
            currentUser = user;
            dataAccess = new DataAccess();

            // Ограничение выбора дат в календаре
            DepartureDatePicker.MinimumDate = DateTime.Today;
            DepartureDatePicker.MaximumDate = DateTime.Today.AddMonths(2);
            DepartureDatePicker.Date = DateTime.Today;
            SelectedDepartureDate = DateTime.Today;
            DepartureDateEntry.Text = SelectedDepartureDate.ToString("d");

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void DepartureCityEntry_Tapped(object sender, EventArgs e)
        {
            var cities = await dataAccess.GetCitiesAsync();

            var cityNames = cities.Select(city => city.Name).ToArray();

            var selectedCity = await DisplayActionSheet("Выберите город отправления", "Отмена", null, cityNames);

            if (!string.IsNullOrEmpty(selectedCity) && selectedCity != "Отмена")
            {
                DepartureCityEntry.Text = selectedCity;

                var city = cities.FirstOrDefault(c => c.Name == selectedCity);
                if (city != null)
                {
                    SelectedDepartureCityId = city.Id;
                }
            }
        }

        private async void ArrivalCityEntry_Tapped(object sender, EventArgs e)
        {
            var cities = await dataAccess.GetCitiesAsync();

            var cityNames = cities.Select(city => city.Name).ToArray();

            var selectedCity = await DisplayActionSheet("Выберите город прибытия", "Отмена", null, cityNames);

            if (!string.IsNullOrEmpty(selectedCity) && selectedCity != "Отмена")
            {
                ArrivalCityEntry.Text = selectedCity;

                var city = cities.FirstOrDefault(c => c.Name == selectedCity);
                if (city != null)
                {
                    SelectedArrivalCityId = city.Id;
                }
            }
        }

        private void OnDatePickerTapped(object sender, EventArgs e)
        {
            DepartureDatePicker.Focus();
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            DepartureDateEntry.Text = e.NewDate.ToString("d");
            DepartureDatePicker.IsVisible = false;
        }

        private async void FindButton_Clicked(object sender, EventArgs e)
        {
            if (SelectedDepartureCityId == 0 || SelectedArrivalCityId == 0)
            {
                await DisplayAlert("Ошибка", "Пожалуйста, выберите город отправления и город прибытия.", "OK");
                return;
            }

            if (SelectedDepartureCityId == SelectedArrivalCityId)
            {
                await DisplayAlert("Ошибка", "Город отправления и город прибытия не могут быть одинаковыми.", "OK");
                return;
            }


            // Получите выбранные значения и сохраните их в свойствах
            SelectedDepartureDate = DepartureDatePicker.Date;

            // Перейдите на страницу RoutesPage, передав выбранные значения
            await Navigation.PushAsync(new RoutesPage(SelectedDepartureDate, SelectedDepartureCityId, SelectedArrivalCityId, currentUser));
        }

        private async void OnProfileTapped(object sender, EventArgs e)
        {
            // Создание новой страницы Profile
            var profile = new Profile(currentUser);

            // Выполнение перехода на страницу Profile
            await Navigation.PushAsync(profile);
        }

        private async void SwapImage_Tapped(object sender, EventArgs e)
        {
            string departureCityText = DepartureCityEntry.Text;
            DepartureCityEntry.Text = ArrivalCityEntry.Text;
            ArrivalCityEntry.Text = departureCityText;

            var cities = await dataAccess.GetCitiesAsync();

            var departureCityName = DepartureCityEntry.Text;
            var arrivalCityName = ArrivalCityEntry.Text;

            var departureCity = cities.FirstOrDefault(city => city.Name == departureCityName);
            var arrivalCity = cities.FirstOrDefault(city => city.Name == arrivalCityName);

            if (departureCity != null)
            {
                SelectedDepartureCityId = departureCity.Id;
            }

            if (arrivalCity != null)
            {
                SelectedArrivalCityId = arrivalCity.Id;
            }
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