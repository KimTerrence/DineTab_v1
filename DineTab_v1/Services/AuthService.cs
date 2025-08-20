using DineTab_v1.Models;
using Microsoft.Data.SqlClient;

namespace DineTab_v1.Services
{
    public class AuthService
    {
        private readonly string _connectionString =
            "Server=192.168.43.57\\SQLEXPRESS;Database=dinetab_db;User Id=appuser;Password=12345;TrustServerCertificate=True;";

        public User Login(string email, string password)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "SELECT Id, Email, Role, FirstName, LastName FROM Users WHERE Email = @Email AND Password = @Password",
                        conn);

                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Get user ID for updating last login
                            int userId = Convert.ToInt32(reader["Id"]);

                            // Update LastLogin timestamp
                            UpdateLastLogin(userId);

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

        private void UpdateLastLogin(int userId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "UPDATE Users SET LastLogin = @LastLogin WHERE Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to update LastLogin: {ex.Message}");
            }
        }
    }
}
