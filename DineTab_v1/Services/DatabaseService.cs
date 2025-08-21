using Microsoft.Data.SqlClient;
using DineTab_v1.Models;
using System.Threading.Tasks;

namespace DineTab_v1.Services
{
    public class DatabaseService
    {
        public List<Category> Categories { get; set; } = new();

        private readonly string _connectionString =
            "Server=192.168.43.57\\SQLEXPRESS;Database=dinetab_db;User Id=appuser;Password=12345;TrustServerCertificate=True;";

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

        // Other database methods like GetMenuItemsAsync(), GetCategoriesAsync(), etc.
    }
}