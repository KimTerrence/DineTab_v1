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
                        "SELECT Email, Role, FirstName, LastName FROM Users WHERE Email = @Email AND Password = @Password",
                        conn);

                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Show the real error instead of crashing
                Console.WriteLine($"❌ Login error: {ex.Message}");
            }
            return null;
        }
    }
}
