using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MyTrain.Data;
using MyTrain.Models;

namespace MyTrain
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private DataAccess _dataAccess;

        public Login()
        {
            InitializeComponent();
            _dataAccess = new DataAccess();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnAlreadyRegister(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            // Проверка введенных данных для авторизации
            if (string.IsNullOrWhiteSpace(UserPhoneEntry.Text) || string.IsNullOrWhiteSpace(UserPasswordEntry.Text))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, заполните все поля", "OK");
                return;
            }

            // Поиск пользователя в базе данных
            var users = await _dataAccess.GetUsersAsync();
            var user = users.FirstOrDefault(u => u.NumberPhone == UserPhoneEntry.Text && u.Password == UserPasswordEntry.Text);

            if (user != null)
            {
                // Успешная авторизация

                // Проверка роли пользователя
                if (user.RoleID == 1)
                {
                    // Перенаправление на страницу обычного пользователя
                    await DisplayAlert("Успех", "Авторизация обычного пользователя прошла успешно", "OK");
                    Profile mainPage = new Profile(user);
                    await Navigation.PushAsync(mainPage);
                }
   
            }
            else
            {
                // Неверные данные для авторизации
                await DisplayAlert("Ошибка", "Неверный номер телефона или пароль", "OK");
            }
        }
    }
}
