using System.ComponentModel.DataAnnotations;

namespace QuizMaker.Database.Base;

public class EntityBase
{
    [Key]
    public long Id { get; set; }
}