using DineTab_v1.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DineTab_v1.Services
{
    public class DatabaseService
    {
        public List<Category> Categories { get; set; } = new();

        private readonly string _connectionString =
            "Server=192.168.43.57\\SQLEXPRESS;Database=dinetab_db;User Id=appuser;Password=12345;TrustServerCertificate=True;";

        //Dashboard
        public async Task<decimal> GetDailyRevenueAsync()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT ISNULL(SUM(Total),0) FROM Orders WHERE CAST(CreatedAt AS DATE)=CAST(GETDATE() AS DATE)", conn);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            });
        }

        public async Task<int> GetDineIn()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Orders WHERE OrderType = 'Dine In' ", conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });        }


        public async Task<int> GetTakeOut()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Orders WHERE OrderType = 'Take Out'", conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
        }

        public async Task<int> GetPending()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Orders WHERE Status = 'Pending'", conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
        }

        public async Task<int> GetReady()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Orders WHERE Status = 'Ready'", conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
        }

        public async Task<int> GetPreparing()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Orders WHERE Status = 'Preparing'", conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
        }

        public async Task<int> GetActiveOrdersAsync()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Orders WHERE Status IN ('Pending','Cooking','Ready')", conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
        }

        public async Task<int> CountCompletedOrdersAsync()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Orders WHERE Status = 'Complete'", conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
        }
        public async Task<int> GetActiveStaffAsync()
        {
            return await Task.Run(() =>
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand("SELECT COUNT(*) FROM Users ", conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
        }


        //Generate Report
        public async Task<List<SoldItemReport>> GetSoldItemsAsync()
        {
            var soldItems = new List<SoldItemReport>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                    SELECT 
                    o.OrderNumber,
                    o.CreatedAt,
                    o.OrderType,
                    SUM(oi.Quantity) AS TotalItem,
                    o.Total
                FROM Orders o
                LEFT JOIN OrderItems oi ON oi.Id = o.Id
                GROUP BY o.OrderNumber, o.CreatedAt, o.OrderType, o.Total
                ORDER BY o.CreatedAt DESC
            ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            soldItems.Add(new SoldItemReport
                            {
                                OrderNo = Convert.ToString(reader["OrderNumber"]),
                                OrderDate = Convert.ToDateTime(reader["CreatedAt"]),
                                Type = reader["OrderType"].ToString(),
                                TotalItem = Convert.ToInt32(reader["TotalItem"]),
                                TotalPrice = Convert.ToDecimal(reader["Total"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return soldItems;
        }


        public async Task<bool> AddStaffAsync(User user)
        {

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Check if email already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", user.Email);

                    int exists = (int)await checkCmd.ExecuteScalarAsync();
                    if (exists > 0)
                    {
                        // if  Duplicate email cancel inserting
                        return false;
                    }
                }

                //  Insert if not duplicate
                string query = "INSERT INTO Users (FirstName, LastName, Email, Password, Role, filename) " +
                               "VALUES (@FirstName, @LastName, @Email, @Password, @Role ,@ProfileImageFile)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@ProfileImageFile", user.ProfileImageFile ?? "");

                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        //Retrive stadff info funtion
        public async Task<List<User>> GetAllStaffAsync()
        {
            var staffList = new List<User>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = "SELECT Id, FirstName, LastName, Email, Role, Password, filename FROM Users";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        staffList.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Role = reader.GetString(4),
                            Password = reader.GetString(5),
                            ProfileImageFile = reader["filename"] as string //  filename
                        });
                    }
                }
            }

            return staffList;
        }


        //Update Staff Info function
        public async Task<bool> UpdateStaffAsync(User user)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql;

                if (!string.IsNullOrEmpty(user.Password))
                {
                    sql = @"UPDATE Users 
                    SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Role=@Role, Status=@Status, Password=@Password
                    WHERE Id=@Id";
                }
                else
                {
                    sql = @"UPDATE Users 
                    SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Role=@Role, Status=@Status
                    WHERE Id=@Id";
                }

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Status", user.Status);
                cmd.Parameters.AddWithValue("@Id", user.Id);

                if (!string.IsNullOrEmpty(user.Password))
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ UpdateStaffAsync error: {ex.Message}");
                return false;
            }
        }

        //delete staff
        public async Task<bool> DeleteStaffAsync(int staffId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "DELETE FROM Users WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", staffId);
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        //Add  Menu Item
        public async Task<bool> AddMenuItemAsync(Item item)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = @"INSERT INTO Items (Name, Price, CategoryId, Availability, Spicy, Image)
                         VALUES (@ItemName, @Price, @CategoryId, @Availability, @Spicy, @Image)";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
                cmd.Parameters.AddWithValue("@Availability", item.Availability);
                cmd.Parameters.AddWithValue("@Spicy", item.Spicy);
                cmd.Parameters.AddWithValue("@Image", item.Image ?? new byte[0]);

                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddMenuItemAsync error: {ex.Message}");
                return false;
            }
        }


        // Update an existing menu item
        public async Task<bool> UpdateMenuItemAsync(Item item)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Items
                SET Name = @name,
                    Price = @price,
                    CategoryId = @categoryId,
                    Availability = @availability,
                    Spicy = @spicy,
                    Image = @image
                WHERE Id = @id;
            ";

            cmd.Parameters.AddWithValue("@name", item.ItemName);
            cmd.Parameters.AddWithValue("@price", item.Price);
            cmd.Parameters.AddWithValue("@categoryId", item.CategoryId);
            cmd.Parameters.AddWithValue("@availability", item.Availability);
            cmd.Parameters.AddWithValue("@spicy", item.Spicy);
            cmd.Parameters.AddWithValue("@image", item.Image ?? new byte[0]);
            cmd.Parameters.AddWithValue("@id", item.Id);

            var result = await cmd.ExecuteNonQueryAsync();
            return result > 0;
        }
  
        // Get all categories
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = new List<Category>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT Id, Name FROM Categories";
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                categories.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            return categories;
        }


        // Add new category
        public async Task<int> AddCategoryAsync(string categoryName)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = @"INSERT INTO Categories (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", categoryName);

                // Returns the new Id
                int newId = (int)await cmd.ExecuteScalarAsync();
                return newId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddCategoryAsync error: {ex.Message}");
                return 0; // 0 means failed
            }
        }


        // Delete category
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = "DELETE FROM Categories WHERE Id = @Id";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", categoryId);

                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteCategoryAsync error: {ex.Message}");
                return false;
            }
        }


        public async Task LoadCategoriesAsync()
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT Id, Name FROM Categories";
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            Categories.Clear();
            while (await reader.ReadAsync())
            {
                Categories.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
        }


        // Get all items
        public async Task<List<Item>> GetMenuItemsAsync()
        {
            var items = new List<Item>();

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = @"
                SELECT i.Id, i.Name, i.Price, i.CategoryId, i.Availability, i.Spicy, i.Image, c.Name AS CategoryName
                FROM Items i
                LEFT JOIN Categories c ON i.CategoryId = c.Id";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(new Item
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ItemName = reader.GetString(reader.GetOrdinal("Name")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                        Availability = reader.GetString(reader.GetOrdinal("Availability")),
                        Spicy = reader.GetString(reader.GetOrdinal("Spicy")),
                        CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName"))
                                       ? "Unknown"
                                       : reader.GetString(reader.GetOrdinal("CategoryName")),
                        Image = reader.IsDBNull(reader.GetOrdinal("Image"))
                                ? null
                                : (byte[])reader["Image"]
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMenuItemsAsync error: {ex.Message}");
            }

            return items;
        }

        // Delete a menu item by ID
        public async Task<bool> DeleteMenuItemAsync(int itemId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Items WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", itemId);

                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return false;
            }
        }


        //Place Order

        // Insert Order, get back OrderId
        public async Task<int> InsertOrderAsync(string orderNumber, string orderType, decimal total)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "INSERT INTO Orders (OrderNumber, OrderType, Total, CreatedAt) OUTPUT INSERTED.Id VALUES (@OrderNumber, @OrderType, @Total, GETDATE())";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);
            cmd.Parameters.AddWithValue("@OrderType", orderType);
            cmd.Parameters.AddWithValue("@Total", total);
            return (int)await cmd.ExecuteScalarAsync();
        }

        // Insert OrderItems linked to orderId
        public async Task InsertOrderItemsAsync(int orderId, ObservableCollection<OrderItem> items)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            foreach (var item in items)
            {
                var query = "INSERT INTO OrderItems (Id, ItemId, Quantity, Price, Name) VALUES (@OrderId, @ItemId, @Quantity, @Price, @Name)";
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.Parameters.AddWithValue("@Name", item.Name);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // Get all orders
        public async Task<List<Order>> GetOrdersAsync()
        {
            var list = new List<Order>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT Id, OrderNumber, OrderType, Total, CreatedAt FROM Orders WHERE Status = 'Pending' ORDER BY CreatedAt DESC";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Order
                {
                    OrderId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),                 
                    OrderNumber = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),  
                    OrderType = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Total = reader.IsDBNull(3) ? 0m : reader.GetDecimal(3),
                    CreatedAt = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4)
                });
            }
            return list;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var list = new List<Order>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Orders ORDER BY CreatedAt DESC";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var order = new Order
                {
                    OrderId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    OrderNumber = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    OrderType = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Total = reader.IsDBNull(3) ? 0m : reader.GetDecimal(3),
                    CreatedAt = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                    Status = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Items = new ObservableCollection<OrderItem>() // prevent null
                };

                list.Add(order);
            }

            // Now load items for each order
            foreach (var order in list)
            {
                order.Items = await GetOrderItemsAsync(order.OrderId);
            }

            return list;
        }

        // Get items of a specific order
        public async Task<ObservableCollection<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            var items = new ObservableCollection<OrderItem>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT ItemId, Name, Quantity, Price FROM OrderItems WHERE Id=@OrderId";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new OrderItem
                {
                    ItemId = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Quantity = reader.GetInt32(2),
                    Price = reader.GetDecimal(3)
                });
            }
            return items;
        }

        //update if Order paid
        public async Task<bool> OrderExistsAsync(string orderNumber)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            string sql = "SELECT COUNT(*) FROM Orders WHERE OrderNumber = @OrderNumber";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task UpdateOrderPaymentStatusAsync(string orderNumber, string paymentStatus)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            string sql = "UPDATE Orders SET Status = @PaymentStatus WHERE OrderNumber = @OrderNumber";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
            cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> InsertPaidOrderAsync(string orderNumber, string orderType, decimal total)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO Orders (OrderNumber, OrderType, Total, CreatedAt, Status) OUTPUT INSERTED.Id VALUES (@OrderNumber, @OrderType, @Total, GETDATE(), 'Paid')";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);
            cmd.Parameters.AddWithValue("@OrderType", orderType);
            cmd.Parameters.AddWithValue("@Total", total);
            return (int)await cmd.ExecuteScalarAsync();
        }

        //Update Order Status
        public async Task UpdateOrderPreparingAsync(int orderId, string status, DateTime? targetTime)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
        UPDATE Orders
        SET Status = @Status,
            TargetTime = @TargetTime
        WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Id", orderId);

            // Handle nullable DateTime
            if (targetTime.HasValue)
                command.Parameters.AddWithValue("@TargetTime", targetTime.Value);
            else
                command.Parameters.AddWithValue("@TargetTime", DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        //get all coplted orders

        public async Task<List<Order>> GetCompletedOrdersAsync()
        {
            var orders = new List<Order>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand("SELECT * FROM Orders WHERE Status = 'Complete' ORDER BY CreatedAt DESC", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        orders.Add(new Order
                        {
                            OrderId = reader.GetInt32(reader.GetOrdinal("Id")),
                            OrderNumber = reader.GetString(reader.GetOrdinal("OrderNumber")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            // Fill other fields if needed
                        });
                    }
                }
            }

            return orders;
        }
        public async Task<List<Order>> GetAllCompletedOrdersAsync()
        {
            var orders = new List<Order>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand("SELECT * FROM Orders WHERE Status = 'Complete' ORDER BY CreatedAt DESC", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        orders.Add(new Order
                        {
                            OrderId = reader.GetInt32(reader.GetOrdinal("Id")),
                            OrderNumber = reader.GetString(reader.GetOrdinal("OrderNumber")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            Items = new ObservableCollection<OrderItem>() // initialize to avoid null
                        });
                    }
                }

                // Populate items for each completed order
                foreach (var order in orders)
                {
                    order.Items = await GetOrderItemsAsync(order.OrderId);
                }
            }

            return orders;
        }


        // Other database methods like GetMenuItemsAsync(), GetCategoriesAsync(), etc.
    }
}