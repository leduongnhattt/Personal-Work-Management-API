﻿namespace PersonalWorkManagement.DTOs
{
    public class UpdateWorkTaskDTO
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateTask { get; set; }

        public DateTime EndDateTask { get; set; }

        public string? Status { get; set; }

        public int ReminderTime { get; set; }
    }
}
