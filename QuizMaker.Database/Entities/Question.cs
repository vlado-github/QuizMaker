using Newtonsoft.Json;
using QuizMaker.Database.Base;

namespace QuizMaker.Database.Entities;

public class Question : EntityBase, IDeletable
{
    public string QuestionPhrase { get; set; }
    public string CorrectAnswer { get; set; }
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; } = null;

    #region Navigation Properties

    [JsonIgnore]
    public virtual IList<Quiz> Quizzes { get; set; } = [];

    #endregion
    
}