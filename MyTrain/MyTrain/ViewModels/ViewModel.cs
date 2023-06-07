using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyTrain.Data;
using MyTrain.Models;
using Xamarin.Forms;

namespace MyTrain.ViewModels
{
    public class ViewModel : BaseViewModel
    {
        private readonly DataAccess _dataAccess;
        private ObservableCollection<City> _cities;

        public ObservableCollection<City> Cities
        {
            get => _cities;
            set => SetProperty(ref _cities, value);
        }

        public ViewModel()
        {
            _dataAccess = new DataAccess();
            Cities = new ObservableCollection<City>();
        }

        public async Task LoadDataAsync()
        {
            // Load data from the database and populate the Cities collection
            var cities = await _dataAccess.GetCitiesAsync();
            Cities.Clear();
            foreach (var city in cities)
            {
                Cities.Add(city);
            }
        }

        public async Task SaveCityAsync(City city)
        {
            var success = await _dataAccess.SaveCityAsync(city);
            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "City saved successfully.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save city.", "OK");
            }
        }

        // Add similar methods for other entities (e.g., SavePlaceAsync, SaveRoleAsync, etc.)
    }
}
