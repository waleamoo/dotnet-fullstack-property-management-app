using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class User : BaseEntity
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public byte[] Password { get; set; }
        public byte[] PasswordKey { get; set; } // sort key
    }
}
