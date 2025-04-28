using TestApp.Models;

namespace MockData
{
    public static class MockDataGenerator
    {
        public static Survey GenerateSurvey()
        {
            return new Survey
            {
                Id = 1,
                Title = "Customer Feedback",
                Description = "This is a feedback survey."
            };
        }

        public static List<SurveyQuestion> GenerateSurveyQuestions(int surveyId)
        {
            return new List<SurveyQuestion>
            {
                new SurveyQuestion
                {
                    Id = 1,
                    SurveyId = surveyId,
                    QuestionText = "How satisfied are you with our product?",
                    QuestionType = "Radio",
                    Options = "Very Satisfied,Satisfied,Neutral,Dissatisfied,Very Dissatisfied"
                },
                new SurveyQuestion
                {
                    Id = 2,
                    SurveyId = surveyId,
                    QuestionText = "What features do you like the most?",
                    QuestionType = "Checkbox",
                    Options = "Speed,Usability,Design,Price"
                },
                new SurveyQuestion
                {
                    Id = 3,
                    SurveyId = surveyId,
                    QuestionText = "Please provide additional comments.",
                    QuestionType = "Text",
                    Options = ""
                }
            };
        }

        public static SurveyResponse GenerateSurveyResponse(int surveyId, int userId)
        {
            var response = new SurveyResponse
            {
                Id = 1,
                SurveyId = surveyId,
                UserId = userId,
                Version = 1,
                SubmittedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Answers = new List<SurveyResponseAnswer>
                {
                    new SurveyResponseAnswer
                    {
                        QuestionId = 1,
                        Answer = "Very Satisfied"
                    },
                    new SurveyResponseAnswer
                    {
                        QuestionId = 2,
                        Answer = "Speed, Usability"
                    },
                    new SurveyResponseAnswer
                    {
                        QuestionId = 3,
                        Answer = "Great product, would buy again!"
                    }
                }
            };

            return response;
        }
    }
}
