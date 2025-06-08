using System.Linq.Expressions;
using Core.Database;
using Core.Database.Users;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public IQueryable<User> FindList(
        Expression<Func<User, bool>> where,
        Func<IQueryable<User>, IQueryable<User>>? includes = null,
        int? limit = null,
        int? skip = null)
    {
        var query = dbContext.Users.Where(where);
        if (includes != null) query = includes(query);

        if (skip != null) query = query.Skip(skip.Value);

        if (limit != null) query = query.Take(limit.Value);

        return query;
    }

    public IEnumerable<User> FindAll(Func<IQueryable<User>, IQueryable<User>>? includes = null)
    {
        var query = dbContext.Users.AsQueryable();
        if (includes != null) query = includes(query);

        return query.ToList();
    }

    public User? Find(Expression<Func<User, bool>> where, Func<IQueryable<User>, IQueryable<User>>? includes = null)
    {
        var query = dbContext.Users.Where(where);
        if (includes != null) query = includes(query);

        return query.FirstOrDefault();
    }

    public User? Add(User? model)
    {
        if (model == null) return model;

        dbContext.Users.Add(model);
        dbContext.SaveChanges();

        return model;
    }

    public IEnumerable<User> Add(IEnumerable<User> models)
    {
        var modelsToAdd = models.ToList();
        dbContext.Users.AddRange(modelsToAdd);
        dbContext.SaveChanges();

        return modelsToAdd;
    }

    public User? Update(User? model)
    {
        if (model == null) return model;

        dbContext.Users.Update(model);
        dbContext.SaveChanges();

        return model;
    }

    public void Update(
        Expression<Func<User, bool>> where,
        Expression<Func<SetPropertyCalls<User>, SetPropertyCalls<User>>> setProperty) =>
        dbContext.Users.Where(where).ExecuteUpdate(setProperty);

    public IEnumerable<User> Update(IEnumerable<User> models)
    {
        var modelsToUpdate = models.ToList();
        dbContext.Users.UpdateRange(modelsToUpdate);
        dbContext.SaveChanges();

        return modelsToUpdate;
    }

    public void Delete(Expression<Func<User, bool>> where) => dbContext.Users.Where(where).ExecuteDelete();
}