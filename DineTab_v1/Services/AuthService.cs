using DineTab_v1.Models;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Net.Mail;


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
        public async Task<bool> SendPinToEmailAsync(string email)
        {
            try
            {
                // 1. Generate a random 6-digit PIN
                Random rnd = new Random();
                string pin = rnd.Next(100000, 999999).ToString();

                // 2. Save PIN in memory (or DB for real)
                _currentPin = pin;

                // 3. Configure SMTP with your Gmail and App Password
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("quineskimterrence@gmail.com", "vqam oypx gknh pjld "),
                    EnableSsl = true,
                };

                // 4. Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("quineskimterrence@gmail.com", "DineTab Support"),
                    Subject = "Your PIN for Password Reset",
                    Body = $"Your PIN is: {pin}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(email);

                // 5. Send email
                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send PIN email: {ex.Message}");
                return false;
            }
        }

        private string _currentPin; // store the latest PIN (for demo)

        public async Task<bool> VerifyPinAsync(string email, string pin)
        {
            await Task.Delay(200); // simulate async
            return pin == _currentPin;
        }


        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            try
            {
                // Validate password (8+ chars and at least 1 special character)
                if (string.IsNullOrWhiteSpace(newPassword) ||
                    newPassword.Length < 8 ||
                    !newPassword.Any(ch => !char.IsLetterOrDigit(ch))) // check for special char
                {
                    Console.WriteLine("❌ Password must be at least 8 characters long and contain a special character.");
                    return false;
                }

                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new SqlCommand(
                        "UPDATE Users SET Password = @Password WHERE Email = @Email", conn);

                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    return rowsAffected > 0; // success if at least 1 row updated
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to reset password: {ex.Message}");
                return false;
            }
        }
    }
}
