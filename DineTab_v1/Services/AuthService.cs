using DineTab_v1.Models;

namespace DineTab_v1.Services
{
    public class AuthService
    {
        public User Login(string username, string password)
        {
            if (username == "admin" && password == "1234")
                return new User { Username = "AdminUser", Role = "Admin" };

            if (username == "staff" && password == "1234")
                return new User { Username = "StaffUser", Role = "Staff" };

            if (username == "employee" && password == "1234")
                return new User { Username = "EmployeeUser", Role = "Employee" };

            return null;
        }
    }
}
