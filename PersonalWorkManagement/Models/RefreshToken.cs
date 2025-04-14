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

        public string? IPAddress { get; set; }

        public string? UserAgent { get; set; }

        public string? PreviousToken { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? RevokedReason { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
