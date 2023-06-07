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
        private User user;
        public DateTime SelectedDepartureDate { get; set; }
        public int SelectedDepartureCityId { get; set; }
        public int SelectedArrivalCityId { get; set; }
        private DataAccess dataAccess;

        public Search()
        {
            InitializeComponent();
            dataAccess = new DataAccess();
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

        //private async void PassengerCountEntry_Tapped(object sender, EventArgs e)
        //{
        //    var passengerCounts = Enumerable.Range(1, 10).Select(n => n.ToString()).ToArray();

        //    var selectedCount = await DisplayActionSheet("Выберите количество пассажиров", "Отмена", null, passengerCounts);

        //    if (!string.IsNullOrEmpty(selectedCount) && selectedCount != "Отмена")
        //    {
        //        PassengerCountEntry.Text = selectedCount;
        //    }
        //}

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
            // Получите выбранные значения и сохраните их в свойствах
            SelectedDepartureDate = DepartureDatePicker.Date;

            // Перейдите на страницу RoutesPage, передав выбранные значения
            await Navigation.PushAsync(new RoutesPage(SelectedDepartureDate, SelectedDepartureCityId, SelectedArrivalCityId));
        }
        private async void OnProfileTapped(object sender, EventArgs e)
        {
            // Создание новой страницы SearchPage
            var profile = new Profile(user);

            // Выполнение перехода на страницу SearchPage
            await Navigation.PushAsync(profile);
        }
    }
}
