using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalWorkManagement.Models
{
    public class RefreshToken
    {
        [Key]
        public string TokenID { get; set; }

        [Required]
        public string Token { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
