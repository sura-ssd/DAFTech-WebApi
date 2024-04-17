using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

    }
}
