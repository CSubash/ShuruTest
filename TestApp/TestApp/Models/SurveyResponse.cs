using System;
using System.Collections.Generic;

namespace TestApp.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int UserId { get; set; }
        public int Version { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<SurveyResponseAnswer> Answers { get; set; }
    }

    public class SurveyResponseAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }

        public int SurveyResponseId { get; set; }
    }
}
