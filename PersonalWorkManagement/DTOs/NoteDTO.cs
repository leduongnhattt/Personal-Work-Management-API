﻿namespace PersonalWorkManagement.DTOs
{
    public class NoteDTO
    {
        public string NoteId { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
