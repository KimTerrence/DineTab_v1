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

    }
}
