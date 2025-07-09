using Microsoft.EntityFrameworkCore;
using QuizMaker.Database.Base;
using QuizMaker.Database.Entities;

namespace QuizMaker.Database;

public class QuizMakerContext : CustomDbContext<QuizMakerContext>
{
    public QuizMakerContext()
    {
    }
    
    public QuizMakerContext(DbContextOptions<QuizMakerContext> options) : base(options)
    {
    }

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Quiz

        var quiz = modelBuilder.Entity<Quiz>();
        quiz.HasKey(x => x.Id);
        quiz.HasQueryFilter(x => !x.IsDeleted);
        quiz.Property(x => x.Name).HasMaxLength(250).IsRequired();
        quiz.HasMany(x => x.Questions)
            .WithMany(x => x.Quizzes)
            .UsingEntity<Dictionary<string, object>>(
                "QuizQuestion",
                x => x.HasOne<Question>().WithMany().HasForeignKey("QuestionId"),
                x => x.HasOne<Quiz>().WithMany().HasForeignKey("QuizId"),
                x =>
                {
                    x.HasKey("QuizId", "QuestionId");
                    x.ToTable("quiz_questions");
                });

        #endregion
        
        #region Question

        var question = modelBuilder.Entity<Question>();
        question.HasKey(x => x.Id);
        question.HasQueryFilter(x => !x.IsDeleted);
        question.Property(x => x.QuestionPhrase).IsRequired();
        question.Property(x => x.CorrectAnswer).IsRequired();

        #endregion
    
        base.OnModelCreating(modelBuilder);
    }
}