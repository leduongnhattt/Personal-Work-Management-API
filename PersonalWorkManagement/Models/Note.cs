using System.ComponentModel.DataAnnotations;

namespace PersonalWorkManagement.Models
{
    public class Note
    {
        [Key]
        public string NoteId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
