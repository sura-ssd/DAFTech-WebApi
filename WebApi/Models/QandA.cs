using System;

namespace WebApi.Models
{
    public class QuestionAnswer
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Question { get; set; }
        public bool IsAnswered { get; set; }
        public string? Answer { get; set; }
        public DateTime Timestamp { get; set; }

        
    }

    public class AdminAnswer
    {
        public int Id { get; set; }
        public int QuestionAnswerId { get; set; } // Foreign key to the QuestionAnswer table
        public string Answer { get; set; }
        public DateTime Timestamp { get; set; }
        public string AdminName { get; set; }
        
    }
}