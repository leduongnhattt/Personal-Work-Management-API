﻿using System.ComponentModel.DataAnnotations;

namespace PersonalWorkManagement.Models
{
    public class Apointment
    {
        [Key]
        public Guid ApointmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [Required]
        public DateTime StartDateApoint { get; set; }

        [Required]
        public DateTime EndDateApoint { get; set; }

        public int ReminderTime { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public virtual User? User { get; set; }

    }
}
