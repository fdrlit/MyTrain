using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Data.SqlClient;
using MyTrain.Models;

namespace MyTrain
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Profile : ContentPage
    {
        private User currentUser;
        private SqlConnection _connection;

        public Profile(User user)
        {
            InitializeComponent();

            currentUser = user;
            _connection = DatabaseHelper.GetSqlConnection();
        }

        private async void OnSearchTapped(object sender, EventArgs e)
        {
            var searchPage = new Search(currentUser);
            await Navigation.PushAsync(searchPage);
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
        protected override bool OnBackButtonPressed()
        {
            Page previousPage = Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];

            if (previousPage is MainPage || previousPage is Login)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Ошибка", "Нельзя переходить к страницам авторизации и регистрации", "OK");
                });
            }
            else
            {
                Navigation.PopAsync();
            }

            return true; // Возвращаем true, чтобы предотвратить стандартное поведение кнопки назад
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);

            ProfileNameText.Text = currentUser.Name;
            ProfileNameEntry.Text = currentUser.Name;
            ProfileSecondNameEntry.Text = currentUser.Surname;
            ProfileMiddleNameEntry.Text = currentUser.MiddleName;
            ProfilePassportEntry.Text = currentUser.PassportData;
            ProfilePhoneEntry.Text = currentUser.NumberPhone;
            ProfilePasswordEntry.Text = currentUser.Password;
        }

        private async void OnEditSaveButtonTapped(object sender, EventArgs e)
        {
            if (ProfileNameEntry.IsEnabled)
            {
                // Режим сохранения измененных данных

                //  Проверка введенных данных перед сохранением
                string passportData = ProfilePassportEntry.Text;
                if (!string.IsNullOrEmpty(passportData) && !IsValidPassportData(passportData))
                {
                    await DisplayAlert("Ошибка", "Введите 10 цифр в поле Паспортные данные", "ОК");
                    return;
                }

                if (!IsValidPhoneNumber(ProfilePhoneEntry.Text))
                {
                    await DisplayAlert("Ошибка", "Введите от 1 до 11 цифр в поле Номер телефона", "ОК");
                    return;
                }

                if (!IsValidPassword(ProfilePasswordEntry.Text))
                {
                    await DisplayAlert("Ошибка", "Пароль должен содержать от 6 до 20 символов", "ОК");
                    return;
                }

                if (ProfileNameEntry.Text.Any(char.IsWhiteSpace) ||
                    ProfileSecondNameEntry.Text.Any(char.IsWhiteSpace) ||
                    ProfileMiddleNameEntry.Text.Any(char.IsWhiteSpace))
                {
                    await DisplayAlert("Ошибка", "Поля имени, фамилии и отчества не должны содержать пробелы", "ОК");
                    return;
                }

                if (!IsValidName(ProfileNameEntry.Text) ||
                    !IsValidName(ProfileSecondNameEntry.Text) ||
                    !IsValidName(ProfileMiddleNameEntry.Text))
                {
                    await DisplayAlert("Ошибка", "Поля имени, фамилии и отчества не должны содержать специальные символы", "ОК");
                    return;
                }

                if (ProfileNameEntry.Text.Length > 35 ||
                    ProfileSecondNameEntry.Text.Length > 35 ||
                    ProfileMiddleNameEntry.Text.Length > 35)
                {
                    await DisplayAlert("Ошибка", "Поля имени, фамилии и отчества не должны содержать более 35 символов", "ОК");
                    return;
                }

                // Сохранение измененных данных
                currentUser.Name = ProfileNameEntry.Text;
                currentUser.Surname = ProfileSecondNameEntry.Text;
                currentUser.MiddleName = ProfileMiddleNameEntry.Text;
                currentUser.PassportData = string.IsNullOrEmpty(passportData) ? null : passportData;
                currentUser.NumberPhone = ProfilePhoneEntry.Text;
                currentUser.Password = ProfilePasswordEntry.Text;

                try
                {
                    await _connection.OpenAsync();

                    var command = new SqlCommand("UPDATE [dbo].[Users] SET [Surname] = @Surname, [Name] = @Name, [MiddleName] = @MiddleName, [PassportData] = @PassportData, [NumberPhone] = @NumberPhone, [Password] = @Password WHERE [Id] = @UserId", _connection);
                    command.Parameters.AddWithValue("@Surname", currentUser.Surname);
                    command.Parameters.AddWithValue("@Name", currentUser.Name);
                    command.Parameters.AddWithValue("@MiddleName", currentUser.MiddleName);
                    if (string.IsNullOrEmpty(currentUser.PassportData))
                    {
                        command.Parameters.AddWithValue("@PassportData", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@PassportData", currentUser.PassportData);
                    }
                    command.Parameters.AddWithValue("@NumberPhone", currentUser.NumberPhone);
                    command.Parameters.AddWithValue("@Password", currentUser.Password);
                    command.Parameters.AddWithValue("@UserId", currentUser.Id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        ProfileNameText.Text = currentUser.Name;
                        EditSaveButtonText.Text = "Изменить";
                        ProfileNameEntry.IsEnabled = false;
                        ProfileSecondNameEntry.IsEnabled = false;
                        ProfileMiddleNameEntry.IsEnabled = false;
                        ProfilePassportEntry.IsEnabled = false;
                        ProfilePhoneEntry.IsEnabled = false;
                        ProfilePasswordEntry.IsEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", "Произошла ошибка при сохранении пользователя: " + ex.Message, "OK");
                }
                finally
                {
                    _connection.Close();
                }
            }
            else
            {
                // Режим редактирования данных

                ProfileNameText.Text = currentUser.Name;
                EditSaveButtonText.Text = "Сохранить";
                ProfileNameEntry.IsEnabled = true;
                ProfileSecondNameEntry.IsEnabled = true;
                ProfileMiddleNameEntry.IsEnabled = true;
                ProfilePassportEntry.IsEnabled = true;
                ProfilePhoneEntry.IsEnabled = true;
                ProfilePasswordEntry.IsEnabled = true;
            }
        }

        private bool IsValidPassportData(string input)
        {
            return Regex.IsMatch(input, @"^\d{10}$");
        }

        private bool IsValidPhoneNumber(string input)
        {
            return Regex.IsMatch(input, @"^\d{1,11}$");
        }

        private bool IsValidPassword(string input)
        {
            return input.Length >= 6 && input.Length <= 20;
        }

        private bool IsValidName(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Zа-яА-Я]+$");
        }

        private async void OnLogoutTapped(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
