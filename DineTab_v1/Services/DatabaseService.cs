using Microsoft.Data.SqlClient;
using DineTab_v1.Models;
using System.Threading.Tasks;

namespace DineTab_v1.Services
{
    public class DatabaseService
    {
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
                string query = "INSERT INTO Users (FirstName, LastName, Email, Password, Role) " +
                               "VALUES (@FirstName, @LastName, @Email, @Password, @Role)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Role", user.Role);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }
        public async Task<List<User>> GetAllStaffAsync()
        {
            var staffList = new List<User>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = "SELECT Id, FirstName, LastName, Email, Role, Password FROM Users";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        staffList.Add(new User
                        {
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Role = reader.GetString(4),
                            Password = reader.GetString(5)
                        });
                    }
                }
            }

            return staffList;
        }
    }
}
