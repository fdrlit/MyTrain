using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MyTrain.Data;
using MyTrain.Models;

namespace MyTrain
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private DataAccess _dataAccess;

        public MainPage()
        {
            InitializeComponent();
            _dataAccess = new DataAccess();
        }

        private async void OnAlreadyLogin(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            // Проверка введенных данных перед регистрацией
            if (string.IsNullOrWhiteSpace(UserNameEntry.Text) ||
                string.IsNullOrWhiteSpace(UserSecondNameEntry.Text) ||
                string.IsNullOrWhiteSpace(UserMiddleNameEntry.Text) ||
                string.IsNullOrWhiteSpace(UserPhoneEntry.Text) ||
                string.IsNullOrWhiteSpace(UserPasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(UserPasswordAgainEntry.Text))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, заполните все поля", "OK");
                return;
            }

            if (UserPasswordEntry.Text.Length < 6)
            {
                await DisplayAlert("Ошибка", "Пароль должен содержать минимум 6 символов", "OK");
                return;
            }

            var existingUser = (await _dataAccess.GetUsersAsync()).FirstOrDefault(u => u.NumberPhone == UserPhoneEntry.Text);
            if (existingUser != null)
            {
                // Аккаунт уже существует
                await DisplayAlert("Ошибка", "Аккаунт с таким номером телефона уже существует", "OK");
                return;
            }

            if (UserPhoneEntry.Text.Length != 11 || !long.TryParse(UserPhoneEntry.Text, out _))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, введите корректный номер телефона", "OK");
                return;
            }

            if (UserPasswordEntry.Text != UserPasswordAgainEntry.Text)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "OK");
                return;
            }

            // Создание нового пользователя
            var user = new User
            {
                Name = UserNameEntry.Text,
                Surname = UserSecondNameEntry.Text,
                MiddleName = UserMiddleNameEntry.Text,
                NumberPhone = UserPhoneEntry.Text,
                Password = UserPasswordEntry.Text,
                RoleID = 1
            };

            // Сохранение пользователя в базе данных
            await _dataAccess.SaveUserAsync(user);

            // Вывод сообщения об успешной регистрации
            await DisplayAlert("Успех", "Регистрация прошла успешно", "OK");
            await Navigation.PushAsync(new Login());
        }
    }
}
