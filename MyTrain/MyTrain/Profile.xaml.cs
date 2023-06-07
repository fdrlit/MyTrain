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
            // Создание новой страницы SearchPage
            var searchPage = new Search();

            // Выполнение перехода на страницу SearchPage
            await Navigation.PushAsync(searchPage);
        }
        private async void OnTicketTapped(object sender, EventArgs e)
        {
            // Создание новой страницы SearchPage
            var ticketPage = new TicketPage();

            // Выполнение перехода на страницу SearchPage
            await Navigation.PushAsync(ticketPage);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);

            // Заполнение данных пользователя на форме
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

                // Проверка введенных данных перед сохранением
                string passportData = ProfilePassportEntry.Text;
                if (!string.IsNullOrEmpty(passportData) && !IsValidPassportData(passportData))
                {
                    await DisplayAlert("Ошибка", "Введите 10 цифр в поле Паспортные данные", "ОК");
                    return;
                }

                if (!IsValidPhoneNumber(ProfilePhoneEntry.Text))
                {
                    await DisplayAlert("Ошибка", "Введите 11 цифр в поле Номер телефона", "ОК");
                    return;
                }

                if (!IsValidPassword(ProfilePasswordEntry.Text))
                {
                    await DisplayAlert("Ошибка", "Пароль должен содержать минимум 6 символов", "ОК");
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
                        // Обновление UI
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

                // Обновление UI
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
            // Проверка, что введены только цифры и количество равно 10
            return Regex.IsMatch(input, @"^\d{10}$");
        }

        private bool IsValidPhoneNumber(string input)
        {
            // Проверка, что введены только цифры и количество равно 11
            return Regex.IsMatch(input, @"^\d{11}$");
        }

        private bool IsValidPassword(string input)
        {
            // Проверка, что пароль содержит минимум 6 символов
            return input.Length >= 6;
        }
    }
}
