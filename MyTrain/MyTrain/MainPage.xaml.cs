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

            // Проверка наличия пробелов и специальных символов
            if (HasSpecialCharactersOrWhitespace(UserNameEntry.Text) ||
                HasSpecialCharactersOrWhitespace(UserSecondNameEntry.Text) ||
                HasSpecialCharactersOrWhitespace(UserMiddleNameEntry.Text) ||
                HasSpecialCharactersOrWhitespace(UserPhoneEntry.Text))
            {
                await DisplayAlert("Ошибка", "Поля не должны содержать пробелы или специальные символы", "OK");
                return;
            }

            // Проверка длины полей
            if (UserNameEntry.Text.Length > 35 ||
                UserSecondNameEntry.Text.Length > 35 ||
                UserMiddleNameEntry.Text.Length > 35 ||
                UserPhoneEntry.Text.Length != 11 ||
                UserPasswordEntry.Text.Length > 20 ||
                UserPasswordAgainEntry.Text.Length > 20)
            {
                await DisplayAlert("Ошибка", "Превышена максимальная длина поля", "OK");
                return;
            }

            // Проверка совпадения паролей
            if (UserPasswordEntry.Text != UserPasswordAgainEntry.Text)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "OK");
                return;
            }

            var existingUser = (await _dataAccess.GetUsersAsync()).FirstOrDefault(u => u.NumberPhone == UserPhoneEntry.Text);
            if (existingUser != null)
            {
                // Аккаунт уже существует
                await DisplayAlert("Ошибка", "Аккаунт с таким номером телефона уже существует", "OK");
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

        private bool HasSpecialCharactersOrWhitespace(string input)
        {
            return input.Any(c => char.IsWhiteSpace(c) || char.IsPunctuation(c));
        }
    }
}
