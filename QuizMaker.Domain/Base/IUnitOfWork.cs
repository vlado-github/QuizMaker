using Microsoft.EntityFrameworkCore;

namespace QuizMaker.Domain.Base;

public interface IUnitOfWork<T> where T : DbContext
{
    Task ExecuteAsync(Func<T, Task> callback);
    Task<TS> ExecuteAsync<TS>(Func<T, Task<TS>> callback) where TS : class;
}