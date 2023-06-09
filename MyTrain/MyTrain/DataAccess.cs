using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MyTrain.Models;
using SkiaSharp;
using Xamarin.Forms;
using Type = MyTrain.Models.Type;

namespace MyTrain.Data
{

    public class DataAccess
    {
        private readonly SqlConnection _connection;

        private async Task HandleConnectionOpenError()
        {
            await App.Current.MainPage.DisplayAlert("Ошибка", "Сервер не успевает за вами, будьте немного терпеливы!", "OK");
        }
        private async Task HandleConnectionLoginError()
        {
            await App.Current.MainPage.DisplayAlert("Ошибка", "Сервер не успевает за вами, будьте немного терпеливы!", "OK");
        }
        public DataAccess()
        {
            _connection = DatabaseHelper.GetSqlConnection();
        }

        public async Task<List<City>> GetCitiesAsync()
        {
            var cities = new List<City>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Cities]", _connection);
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var city = new City
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    cities.Add(city);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return cities;
        }

        public async Task<bool> SaveCityAsync(City city)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Cities] ([Name]) VALUES (@Name)", _connection);
                command.Parameters.AddWithValue("@Name", city.Name);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<City> GetCityByIdAsync(int cityId)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Cities] WHERE Id = @CityId", _connection);
                command.Parameters.AddWithValue("@CityId", cityId);
                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    var city = new City
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };

                    return city;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return null;
        }
        public async Task<City> GetCityById(int cityId)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Cities] WHERE Id = @CityId", _connection);
                command.Parameters.AddWithValue("@CityId", cityId);
                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    var city = new City
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };

                    reader.Close(); // Close the reader after reading the data
                    _connection.Close(); // Close the connection after reading the data

                    return city;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close(); // Close the connection in case of any exceptions
            }

            return null;
        }

        public async Task<List<Place>> GetPlacesAsync()
        {
            var places = new List<Place>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Places]", _connection);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var place = new Place
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        UserId = !reader.IsDBNull(2) ? reader.GetInt32(2) : (int?)null,
                        WagonId = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0
                    };
                    places.Add(place);
                }


            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return places;
        }
        public async Task<string> GetPlaceNumberAsync(int placeId, int userId)
        {
            try
            {
                await _connection.OpenAsync();
                var command = new SqlCommand("SELECT Places.Name FROM Places INNER JOIN Tickets ON Places.Id = Tickets.PlaceId WHERE Tickets.UserId = @UserId AND Places.Id = @PlaceId", _connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PlaceId", placeId);
                var placeName = await command.ExecuteScalarAsync();

                if (placeName != null && placeName != DBNull.Value)
                {
                    return placeName.ToString();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return string.Empty;
        }







        public async Task<List<Place>> GetPlacesByWagonIdAsync(int wagonId)
        {
            var places = new List<Place>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Places] WHERE WagonId = @wagonId", _connection);
                command.Parameters.AddWithValue("@wagonId", wagonId);

                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var place = new Place
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        UserId = !reader.IsDBNull(2) ? reader.GetInt32(2) : (int?)null,
                        WagonId = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0
                    };
                    places.Add(place);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return places;
        }

        public async Task UpdatePlaceAsync(Place place)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("UPDATE [dbo].[Places] SET UserId = @userId WHERE Id = @placeId", _connection);
                command.Parameters.AddWithValue("@userId", place.UserId);
                command.Parameters.AddWithValue("@placeId", place.Id);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<bool> SavePlaceAsync(Place place)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Places] ([Name], [UserId], [WagonId]) VALUES (@Name, @UserId, @WagonId)", _connection);
                command.Parameters.AddWithValue("@Name", place.Name);
                command.Parameters.AddWithValue("@UserId", place.UserId);
                command.Parameters.AddWithValue("@WagonId", place.WagonId);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            var roles = new List<Role>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Roles]", _connection);
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var role = new Role
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    roles.Add(role);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return roles;
        }

        public async Task<bool> SaveRoleAsync(Role role)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Roles] ([Name]) VALUES (@Name)", _connection);
                command.Parameters.AddWithValue("@Name", role.Name);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<List<Train>> GetTrainsAsync()
        {
            var trains = new List<Train>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Trains]", _connection);
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var train = new Train
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    trains.Add(train);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return trains;
        }

        public async Task<bool> SaveTrainAsync(Train train)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Trains] ([Name]) VALUES (@Name)", _connection);
                command.Parameters.AddWithValue("@Name", train.Name);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<Train> GetTrainByIdAsync(int trainId)
        {
            Train train = null;

            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Trains] WHERE Id = @TrainId", _connection);
                command.Parameters.AddWithValue("@TrainId", trainId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    train = new Train
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return train;
        }

        public async Task<List<Wagon>> GetWagonsAsync()
        {
            var wagons = new List<Wagon>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Wagons]", _connection);
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var wagon = new Wagon
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Count = reader.GetString(2),
                        TrainsId = reader.GetInt32(3),
                        TypeId = reader.GetInt32(4)
                    };
                    wagons.Add(wagon);
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return wagons;
        }
        public async Task<Wagon> GetWagonByIdAsync(int wagonId)
        {
            Wagon wagon = null;
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Wagons] WHERE [Id] = @WagonId", _connection);
                command.Parameters.AddWithValue("@WagonId", wagonId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    wagon = new Wagon
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Count = reader.GetString(2),
                        TrainsId = reader.GetInt32(3),
                        TypeId = reader.GetInt32(4)
                        // Set other properties as needed
                    };
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return wagon;
        }
        public async Task<bool> SaveWagonAsync(Wagon wagon)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Wagons] ([Name], [Count], [TrainsId], [TypeId]) VALUES (@Name, @Count, @TrainsId, @TypeId)", _connection);
                command.Parameters.AddWithValue("@Name", wagon.Name);
                command.Parameters.AddWithValue("@Count", wagon.Count);
                command.Parameters.AddWithValue("@TrainsId", wagon.TrainsId);
                command.Parameters.AddWithValue("@TypeId", wagon.TypeId);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<List<Wagon>> GetWagonsByTrainIdAndTypeAsync(int trainId, int typeId)
        {
            var wagons = new List<Wagon>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Wagons] WHERE [TrainsId] = @TrainId AND [TypeId] = @TypeId", _connection);
                command.Parameters.AddWithValue("@TrainId", trainId);
                command.Parameters.AddWithValue("@TypeId", typeId);

                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var wagon = new Wagon
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Count = reader.GetString(2),
                        TrainsId = reader.GetInt32(3),
                        TypeId = reader.GetInt32(4)
                    };
                    wagons.Add(wagon);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return wagons;
        }

        public async Task<List<User>> GetUsersAsync(string phoneNumber = null)
        {
            var users = new List<User>();
            try
            {
                await _connection.OpenAsync();

                string query = "SELECT * FROM [dbo].[Users]";
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    query += " WHERE NumberPhone = @NumberPhone";
                }

                var command = new SqlCommand(query, _connection);
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    command.Parameters.AddWithValue("@NumberPhone", phoneNumber);
                }

                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Surname = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                        MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
                        NumberPhone = reader.IsDBNull(4) ? null : reader.GetString(4),
                        PassportData = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Password = reader.IsDBNull(6) ? null : reader.GetString(6),
                        RoleID = reader.GetInt32(7)
                    };
                    users.Add(user);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is connecting"))
                {
                    await HandleConnectionLoginError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return users;
        }

        public async Task<bool> SaveUserAsync(User user)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Users] ([Surname], [Name], [MiddleName], [NumberPhone], [PassportData], [Password], [RoleID]) VALUES (@Surname, @Name, @MiddleName, @NumberPhone, @PassportData, @Password, @RoleID)", _connection);
                command.Parameters.AddWithValue("@Surname", user.Surname);
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@MiddleName", user.MiddleName);
                command.Parameters.AddWithValue("@NumberPhone", user.NumberPhone);
                if (user.PassportData != null)
                    command.Parameters.AddWithValue("@PassportData", user.PassportData);
                else
                    command.Parameters.AddWithValue("@PassportData", DBNull.Value);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@RoleID", user.RoleID == null ? DBNull.Value : (object)user.RoleID);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<List<Route>> GetRoutesAsync()
        {
            var routes = new List<Route>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Routes]", _connection);
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var route = new Route
                    {
                        Id = reader.GetInt32(0),
                        DepartureDate = reader.GetDateTime(1),
                        DepartureCityId = reader.GetInt32(2),
                        ArrivalCityId = reader.GetInt32(3),
                        ArrivalDate = reader.GetDateTime(4),
                        TrainsId = reader.GetInt32(5),
                        PriceCoupe = reader.GetDecimal(6),
                        PriceEconom = reader.GetDecimal(7)
                    };
                    routes.Add(route);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return routes;
        }

        public async Task<bool> SaveRouteAsync(Route route)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Routes] ([DepartureDate], [DepartureCityId], [ArrivalCityId], [ArrivalDate], [TrainsId], [PriceCoupe], [PriceEconom]) VALUES (@DepartureDate, @DepartureCityId, @ArrivalCityId, @ArrivalDate, @TrainsId, @PriceCoupe, @PriceEconom)", _connection);
                command.Parameters.AddWithValue("@DepartureDate", route.DepartureDate);
                command.Parameters.AddWithValue("@DepartureCityId", route.DepartureCityId);
                command.Parameters.AddWithValue("@ArrivalCityId", route.ArrivalCityId);
                command.Parameters.AddWithValue("@ArrivalDate", route.ArrivalDate);
                command.Parameters.AddWithValue("@TrainsId", route.TrainsId);
                command.Parameters.AddWithValue("@PriceCoupe", route.PriceCoupe);
                command.Parameters.AddWithValue("@PriceEconom", route.PriceEconom);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<List<Route>> SearchRoutesAsync(string departureCity, string arrivalCity, int passengerCount, DateTime departureDate)
        {
            var routes = new List<Route>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Routes] WHERE DepartureCityId = @DepartureCityId AND ArrivalCityId = @ArrivalCityId AND DepartureDate >= @DepartureDate", _connection);
                command.Parameters.AddWithValue("@DepartureCityId", departureCity);
                command.Parameters.AddWithValue("@ArrivalCityId", arrivalCity);
                command.Parameters.AddWithValue("@DepartureDate", departureDate);

                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var route = new Route
                    {
                        Id = reader.GetInt32(0),
                        DepartureDate = reader.GetDateTime(1),
                        DepartureCityId = reader.GetInt32(2),
                        ArrivalCityId = reader.GetInt32(3),
                        ArrivalDate = reader.GetDateTime(4),
                        TrainsId = reader.GetInt32(5),
                        PriceCoupe = reader.GetDecimal(6),
                        PriceEconom = reader.GetDecimal(7)
                    };
                    routes.Add(route);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return routes;
        }


        public async Task<List<Ticket>> GetTicketsAsync()
        {
            var tickets = new List<Ticket>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Tickets]", _connection);
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var ticket = new Ticket
                    {
                        Id = reader.GetInt32(0),
                        RouteId = reader.GetInt32(1),
                        WagonId = reader.GetInt32(2),
                        PlaceId = reader.GetInt32(3),
                        UserId = reader.GetInt32(4)
                    };
                    tickets.Add(ticket);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return tickets;
        }

        public async Task<bool> SaveTicketAsync(Ticket ticket)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Tickets] ([RouteId], [WagonId], [PlaceId], [UserId]) VALUES (@RouteId, @WagonId, @PlaceId, @UserId)", _connection);
                command.Parameters.AddWithValue("@RouteId", ticket.RouteId);
                command.Parameters.AddWithValue("@WagonId", ticket.WagonId);
                command.Parameters.AddWithValue("@PlaceId", ticket.PlaceId);
                command.Parameters.AddWithValue("@UserId", ticket.UserId);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<List<Ticket>> GetTicketsByUserId(int userId)
        {
            var tickets = new List<Ticket>();

            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Tickets] WHERE [UserId] = @UserId", _connection);
                command.Parameters.AddWithValue("@UserId", userId);

                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var ticket = new Ticket
                    {
                        Id = reader.GetInt32(0),
                        RouteId = reader.GetInt32(1),
                        WagonId = reader.GetInt32(2),
                        PlaceId = reader.GetInt32(3),
                        UserId = reader.GetInt32(4)
                    };

                    tickets.Add(ticket);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return tickets;
        }


        public async Task<List<Type>> GetTypesAsync()
        {
            var types = new List<Type>();
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Types]", _connection);
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var type = new Type
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    types.Add(type);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return types;
        }

        public async Task<bool> SaveTypeAsync(Type type)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[Types] ([Name]) VALUES (@Name)", _connection);
                command.Parameters.AddWithValue("@Name", type.Name);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<List<Ticket>> GetUserTicketsAsync(int userId)
        {
            var tickets = new List<Ticket>();

            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM Tickets WHERE UserId = @UserId", _connection);
                command.Parameters.AddWithValue("@UserId", userId);

                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var ticket = new Ticket
                    {
                        Id = reader.GetInt32(0),
                        PlaceId = reader.GetInt32(1),
                        WagonId = reader.GetInt32(2),
                        RouteId = reader.GetInt32(3),
                        UserId = reader.GetInt32(4)
                    };

                    tickets.Add(ticket);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return tickets;
        }

        // Метод для получения названия поезда по его ID
        public async Task<string> GetTrainNameAsync(int trainId)
        {
            string trainName = string.Empty;

            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT Name FROM Trains WHERE Id = @TrainId", _connection);
                command.Parameters.AddWithValue("@TrainId", trainId);

                var result = await command.ExecuteScalarAsync();

                if (result != null)
                {
                    trainName = result.ToString();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return trainName;
        }

        // Метод для получения номера места по его ID
       

        // Метод для получения названия вагона по его ID
        public async Task<string> GetWagonNameAsync(int wagonId)
        {
            string wagonName = string.Empty;

            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT Name FROM Wagons WHERE Id = @WagonId", _connection);
                command.Parameters.AddWithValue("@WagonId", wagonId);

                var result = await command.ExecuteScalarAsync();

                if (result != null)
                {
                    wagonName = result.ToString();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return wagonName;
        }

        // Метод для получения названия города по его ID
        public async Task<string> GetCityNameAsync(int cityId)
        {
            string cityName = string.Empty;

            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT Name FROM Cities WHERE Id = @CityId", _connection);
                command.Parameters.AddWithValue("@CityId", cityId);

                var result = await command.ExecuteScalarAsync();

                if (result != null)
                {
                    cityName = result.ToString();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return cityName;
        }

        // Метод для получения названия класса вагона по его ID
        public async Task<string> GetWagonClassNameAsync(int wagonId)
        {
            string wagonClassName = string.Empty;

            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT T.Name FROM Wagons W INNER JOIN Types T ON W.TypeId = T.Id WHERE W.Id = @WagonId", _connection);
                command.Parameters.AddWithValue("@WagonId", wagonId);

                var result = await command.ExecuteScalarAsync();

                if (result != null)
                {
                    wagonClassName = result.ToString();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return wagonClassName;
        }
        public async Task<bool> PurchasePlaceAsync(User currentUser, int placeId, int wagonId, int routeId)
{
    try
    {
        await _connection.OpenAsync();

        var command = new SqlCommand("INSERT INTO [dbo].[Tickets] (RouteId, WagonId, PlaceId, UserId) VALUES (@routeId, @wagonId, @placeId, @userId)", _connection);
        command.Parameters.AddWithValue("@routeId", routeId); // Используется переданное значение routeId
        command.Parameters.AddWithValue("@wagonId", wagonId); // Используется переданное значение wagonId
        command.Parameters.AddWithValue("@placeId", placeId);
        command.Parameters.AddWithValue("@userId", currentUser.Id);

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }
    catch (Exception ex)
    {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return false;
    }
    finally
    {
        _connection.Close();
    }
}

        public async Task<Train> GetTrainById(int trainId)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Trains] WHERE [Id] = @TrainId", _connection);
                command.Parameters.AddWithValue("@TrainId", trainId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    var train = new Train
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };

                    return train;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return null;
        }

        public async Task<Wagon> GetWagonById(int wagonId)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Wagons] WHERE [Id] = @WagonId", _connection);
                command.Parameters.AddWithValue("@WagonId", wagonId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    var wagon = new Wagon
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Count = reader.GetString(2),
                        TrainsId = reader.GetInt32(3),
                        TypeId = reader.GetInt32(4)
                    };

                    return wagon;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return null;
        }

        public async Task<City> GetCityByIdd(int cityId)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Cities] WHERE [Id] = @CityId", _connection);
                command.Parameters.AddWithValue("@CityId", cityId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    var city = new City
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };

                    return city;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return null;
        }
        public async Task<string> GetUserNameAsync(int userId)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT [Name] FROM [dbo].[Users] WHERE [Id] = @UserId", _connection);
                command.Parameters.AddWithValue("@UserId", userId);

                var result = await command.ExecuteScalarAsync();

                return result?.ToString();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<Route> GetRouteByIdAsync(int routeId)
        {
            try
            {
                await _connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM [dbo].[Routes] WHERE [Id] = @RouteId", _connection);
                command.Parameters.AddWithValue("@RouteId", routeId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    var route = new Route
                    {
                        Id = reader.GetInt32(0),
                        DepartureDate = reader.GetDateTime(1),
                        DepartureCityId = reader.GetInt32(2),
                        ArrivalCityId = reader.GetInt32(3),
                        ArrivalDate = reader.GetDateTime(4),
                        TrainsId = reader.GetInt32(5),
                        PriceCoupe = reader.GetDecimal(6),
                        PriceEconom = reader.GetDecimal(7)
                    };

                    return route;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The connection was not closed. The connection's current state is open"))
                {
                    await HandleConnectionOpenError();
                }
                else
                {
                    // Обработка других исключений
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
                }
            }
            finally
            {
                _connection.Close();
            }

            return null; // Если маршрут не найден
        }

    }
}
