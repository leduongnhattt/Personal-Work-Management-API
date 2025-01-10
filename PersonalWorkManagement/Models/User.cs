using System.ComponentModel.DataAnnotations;

namespace PersonalWorkManagement.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Username must be least 100 characters")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public string SDT { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
