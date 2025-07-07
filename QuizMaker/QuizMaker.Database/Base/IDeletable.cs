
using Newtonsoft.Json;

namespace QuizMaker.Database.Base;

public interface IDeletable
{
    [JsonIgnore]
    bool IsDeleted { get; set; }
    
    [JsonIgnore]
    DateTime? DeletedAt { get; set; }
}