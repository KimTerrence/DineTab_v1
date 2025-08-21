using DineTab_v1.Models;
using Microsoft.Data.SqlClient;

namespace DineTab_v1.Services
{
    public class AuthService
    {
        private readonly string _connectionString =
            "Server=192.168.43.57\\SQLEXPRESS;Database=dinetab_db;User Id=appuser;Password=12345;TrustServerCertificate=True;";

        // Async login method
        public async Task<User> LoginAsync(string email, string password)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new SqlCommand(
                        "SELECT Id, Email, Role, FirstName, LastName FROM Users WHERE Email = @Email AND Password = @Password",
                        conn);

                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int userId = Convert.ToInt32(reader["Id"]);

                            // Update LastLogin asynchronously
                            await UpdateLastLoginAsync(userId);

                            return new User
                            {
                                Id = userId,
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login error: {ex.Message}");
            }

            return null;
        }

        // Async update last login
        private async Task UpdateLastLoginAsync(int userId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new SqlCommand(
                        "UPDATE Users SET LastLogin = @LastLogin WHERE Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", userId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to update LastLogin: {ex.Message}");
            }
        }
    }
}
