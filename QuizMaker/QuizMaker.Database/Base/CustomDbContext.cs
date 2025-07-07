using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace QuizMaker.Database.Base;

public class CustomDbContext<T> : DbContext where T : DbContext
{
    public CustomDbContext() : base()
    {
    }

    public CustomDbContext(DbContextOptions<T> options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        HandleDelete();
        return base.SaveChanges();
    }

    private void HandleDelete()
    {
        var changeSet = ChangeTracker.Entries<IDeletable>()
            .Where(e => e.State == EntityState.Deleted);
        if (changeSet != null)
        {
            foreach (var entry in changeSet)
            {
                entry.State = EntityState.Modified;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.IsDeleted = true;
            }
        }
    }
}
