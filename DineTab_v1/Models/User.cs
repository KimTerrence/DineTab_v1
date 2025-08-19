namespace DineTab_v1.Models
{
    public class User
    {
        public string FullName => $"{FirstName} {LastName}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Admin, Staff, Cashier
        public string Status { get; set; } // Active, Inactive, etc.
        public string PhotoPath { get; set; } // Path to user's photo, if any


         public string MaskedPassword // hash a portion of the passwrd
    {
        get
        {
            if (string.IsNullOrEmpty(Password))
                return "";

            // Show only first 3 characters, mask the rest
            if (Password.Length <= 3)
                return new string('*', Password.Length);

            return Password.Substring(0, 3) + new string('*', Password.Length - 3);
        }
    }
    }

}
