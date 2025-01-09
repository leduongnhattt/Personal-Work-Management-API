using PersonalWorkManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonalWorkManagement.DTOs
{
    public class WorkTaskDTO
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateTask { get; set; }

        public DateTime EndDateTask { get; set; }

        public string Status { get; set; }

        public int ReminderTime { get; set; }
    }
}
