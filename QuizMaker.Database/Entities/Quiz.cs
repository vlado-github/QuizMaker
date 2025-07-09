using QuizMaker.Database.Base;

namespace QuizMaker.Database.Entities;

public class Quiz : EntityBase, IDeletable
{
    public string Name { get; set; }
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; } = null;
    
    #region Navigation Properties

    public virtual IList<Question> Questions { get; set; } = [];

    #endregion
}