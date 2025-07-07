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
        quiz.HasMany(e => e.Questions)
            .WithMany(e => e.Quizzes);

        #endregion
        
        #region Question

        var question = modelBuilder.Entity<Question>();
        question.HasKey(x => x.Id);
        question.HasQueryFilter(x => !x.IsDeleted);
        question.Property(x => x.QuestionPhrase).IsRequired();
        question.Property(x => x.CorrectAnswer).IsRequired();
        question.HasMany(x => x.Quizzes)
            .WithMany(q => q.Questions);

        #endregion
    
        base.OnModelCreating(modelBuilder);
    }
}