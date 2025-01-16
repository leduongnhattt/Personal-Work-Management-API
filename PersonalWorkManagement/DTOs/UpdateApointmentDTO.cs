namespace PersonalWorkManagement.DTOs
{
    public class UpdateApointmentDTO
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Location { get; set; }

        public DateTime StartDateApoint { get; set; }

        public DateTime EndDateApoint { get; set; }

        public int ReminderTime { get; set; }
    }
}
