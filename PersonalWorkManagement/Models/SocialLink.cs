using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalWorkManagement.Models
{
    public class SocialLink
    {
        [Key]
        public string SocialLinkId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Platform { get; set; }

        [Required]
        public string Url { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
} 