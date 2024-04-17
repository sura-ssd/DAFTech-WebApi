using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string PhoneNumber { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string? ProfileImage { get; set; }
    }
}
