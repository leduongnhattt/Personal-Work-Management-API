using System.ComponentModel.DataAnnotations;

namespace PersonalWorkManagement.Models
{
    public class Note
    {
        [Key]
        public Guid NoteId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
