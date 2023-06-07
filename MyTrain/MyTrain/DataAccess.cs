using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTrain.Models;
using Xamarin.Forms;
using Type = MyTrain.Models.Type;

namespace MyTrain.Data
{

    public class DataAccess
    {
        private readonly SqlConnection _connection;


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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении городов: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении города: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении города: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
            }
            finally
            {
                _connection.Close();
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
                // Handle the exception
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении мест: " + ex.Message, "OK");
                // Additional error handling actions
                // ...
            }
            finally
            {
                _connection.Close();
            }

            return places;
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
                // Обработка ошибки
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении мест: " + ex.Message, "OK");
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
                // Обработка ошибки
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при обновлении места: " + ex.Message, "OK");
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении места: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении ролей: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении роли: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении поездов: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении поезда: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении информации о поезде: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении вагонов: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Handle the exception
                Console.WriteLine("An error occurred while retrieving wagon details: " + ex.Message);
                // Additional error handling actions
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении вагона: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении вагонов: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении пользователей: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении пользователя: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении маршрутов: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении маршрута: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при поиске маршрутов: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении билетов: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении билета: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
                return false;
            }
            finally
            {
                _connection.Close();
            }
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при получении типов: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
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
                // Обработка исключения
                await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении типа: " + ex.Message, "OK");
                // Дополнительные действия по обработке исключения
                // ...
                return false;
            }
            finally
            {
                _connection.Close();
            }
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
        // Handle the exception
        await App.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при покупке места: " + ex.Message, "OK");
        // Additional error handling actions
        // ...
        return false;
    }
    finally
    {
        _connection.Close();
    }
}


    }
}
