using System.ComponentModel.DataAnnotations;

namespace PersonalWorkManagement.Models
{
    public class WorkTask
    {
        [Key]
        public Guid WorkTaskId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDateTask { get; set; }

        [Required]
        public DateTime EndDateTask { get; set; }

        [Required]
        public StatusTask Status { get; set; }

        public DateTime? ReminderTime { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public virtual User? User { get; set; }


        public Guid? ApointmentId { get; set; }  
        public virtual Apointment? Apointment { get; set; } 

        public bool IsValidDateTime => StartDateTask < EndDateTask;
    }
}
