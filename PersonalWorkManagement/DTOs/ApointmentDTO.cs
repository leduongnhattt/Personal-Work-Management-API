namespace PersonalWorkManagement.DTOs
{
    public class ApointmentDTO
    {
        public string ApointmentId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateApoint { get; set; }

        public DateTime EndDateApoint { get; set; }

        public string? Location { get; set; }

        public int ReminderTime { get; set; }
    }
}
