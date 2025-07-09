using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using NpgsqlTypes;
using QuizMaker.Database;
using QuizMaker.Database.Base;
using QuizMaker.Database.Entities;

namespace QuizMaker.Tests.Base;

public class TestSeeder : IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private QuizMakerContext _dbContext;
    private Faker _faker;

    public TestSeeder(QuizMakerContext dbContext, int memoryCacheSize = 100)
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = memoryCacheSize });
        _dbContext = dbContext; 
        _faker = new Faker("en");
    }

    public TestSeeder Include<TEntity>(int numberOfItems = 1, string prefixText = "") where TEntity : EntityBase
    {
        for (var i = 0; i < numberOfItems; i++)
        {
            AddEntity<TEntity>(prefixText);
        }

        return this;
    }

    private void AddEntity<TEntity>(string prefixText = "") where TEntity : EntityBase
    {
        #region Quiz

        if (typeof(TEntity) == typeof(Quiz))
        {
            var quiz = new Quiz
            {
                Name = $"{prefixText}{_faker.Lorem.Letter(200)}"
            };
            _dbContext.Quizzes.Add(quiz);
            _dbContext.SaveChanges();
            AddCacheItem(quiz, quiz.Id);
        }
        else if (typeof(TEntity) == typeof(Question))
        {
            foreach (var quiz in GetItems<Quiz>())
            {
                var question = new Question()
                {
                    QuestionPhrase = $"{prefixText}{_faker.Lorem.Sentence()}",
                    CorrectAnswer = _faker.Lorem.Sentence()
                };
                quiz.Questions.Add(question);
                _dbContext.SaveChanges();
                AddCacheItem(question, question.Id);
            }
        }

        #endregion
    }

    public IList<TEntity> GetItems<TEntity>() where TEntity : EntityBase
    {
        _memoryCache.TryGetValue(typeof(TEntity).Name, out IList<TEntity>? listOfItems);
        if (listOfItems == null)
        {
            listOfItems = new List<TEntity>();
        }

        return listOfItems;
    }

    public IList<TEntity> GetItems<TEntity>(Func<TEntity, bool> filter) where TEntity : EntityBase
    {
        _memoryCache.TryGetValue(typeof(TEntity).Name, out IList<TEntity>? listOfItems);
        if (listOfItems == null)
        {
            listOfItems = new List<TEntity>();
        }

        if (filter != null)
        {
            return listOfItems.Where(filter).ToList();
        }

        return listOfItems;
    }

    public void AddCacheItem<TEntity>(TEntity item, long id) where TEntity : EntityBase
    {
        var listOfItems = GetItems<TEntity>();
        if (!listOfItems.Select(x => x.Id).Contains(id))
        {
            item.Id = id;
            listOfItems.Add(item);
        }

        _memoryCache.Set(typeof(TEntity).Name, listOfItems, new MemoryCacheEntryOptions().SetSize(1));
    }

    public void AddItemForCleanup<TEntity>(long id) where TEntity : EntityBase, new()
    {
        var listOfItems = GetItems<TEntity>();
        listOfItems.Add(new TEntity { Id = id });
        _memoryCache.Set(typeof(TEntity).Name, listOfItems, new MemoryCacheEntryOptions().SetSize(1));
    }

    public void AddItemsForCleanup<TEntity>(IEnumerable<long> ids) where TEntity : EntityBase, new()
    {
        var listOfItems = GetItems<TEntity>().ToList();
        foreach (var id in ids)
        {
            listOfItems.Add(new TEntity { Id = id });
        }

        _memoryCache.Set(typeof(TEntity).Name, listOfItems, new MemoryCacheEntryOptions().SetSize(1));
    }

    public void Dispose()
    {
        CleanUp();
        _memoryCache.Dispose();
        _dbContext.Dispose();
    }

    private void CleanUp()
    {
        #region Quiz

        if (GetItems<Question>().Any())
        {
            var ids = GetItems<Question>().Select(x => x.Id).ToList();
            var param = new NpgsqlParameter("@ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = ids
            };
            var sql = "DELETE FROM questions WHERE id = ANY(@ids);";
            _dbContext.Database.ExecuteSqlRaw(sql, param);
        }

        if (GetItems<Quiz>().Any())
        {
            var ids = GetItems<Quiz>().Select(x => x.Id).ToList();
            var param = new NpgsqlParameter("@ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = ids
            };
            var sql = "DELETE FROM quizzes WHERE id = ANY(@ids);";
            _dbContext.Database.ExecuteSqlRaw(sql, param);
        }

        #endregion
    }
}