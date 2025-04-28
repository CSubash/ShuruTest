using Microsoft.EntityFrameworkCore;
using TestApp.Models;

namespace TestApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<SurveyResponseAnswer> SurveyResponseAnswers { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
    }
}