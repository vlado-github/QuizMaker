using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace QuizMaker.Domain.Base;

public class UnitOfWork<T> : IUnitOfWork<T> where T : DbContext
{
    private readonly T _dbContext;
    private readonly ILogger<UnitOfWork<T>> _logger;

    public UnitOfWork(T dbContext,
        ILogger<UnitOfWork<T>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(Func<T, Task> callback)
    {
        using (var scope = new TransactionScope(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            _logger.LogInformation("UnitOfWork: transaction started");

            await callback(_dbContext);

            await _dbContext.SaveChangesAsync();
            scope.Complete();

            _logger.LogInformation("UnitOfWork: transaction completed");
        }
    }

    public async Task<TS> ExecuteAsync<TS>(Func<T, Task<TS>> callback) where TS : class
    {
        TS result = default(TS);

        using (var scope = new TransactionScope(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            _logger.LogInformation("UnitOfWork: transaction started");

            result = await callback(_dbContext);

            await _dbContext.SaveChangesAsync();
            scope.Complete();

            _logger.LogInformation("UnitOfWork: transaction completed");
        }
        return result;
    }
}