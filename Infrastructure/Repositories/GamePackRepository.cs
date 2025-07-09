using System.Linq.Expressions;
using Core.Server.Database;
using Core.Server.Database.GamePacks;
using Core.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories;

public class GamePackRepository(ApplicationDbContext dbContext) : IGamePackRepository
{
    public IQueryable<GamePack> FindList(Expression<Func<GamePack, bool>> where,
        Func<IQueryable<GamePack>, IQueryable<GamePack>>? includes = null, int? limit = null, int? skip = null)
    {
        var query = dbContext.GamePacks.Where(where);
        if (includes != null) query = includes(query);

        if (skip != null) query = query.Skip(skip.Value);

        if (limit != null) query = query.Take(limit.Value);

        return query;
    }

    public IEnumerable<GamePack> FindAll(Func<IQueryable<GamePack>, IQueryable<GamePack>>? includes = null)
    {
        var query = dbContext.GamePacks.AsQueryable();
        if (includes != null) query = includes(query);

        return query.ToList();
    }

    public GamePack? Find(Expression<Func<GamePack, bool>> where,
        Func<IQueryable<GamePack>, IQueryable<GamePack>>? includes = null)
    {
        var query = dbContext.GamePacks.Where(where);
        if (includes != null) query = includes(query);

        return query.FirstOrDefault();
    }

    public GamePack? Add(GamePack? model)
    {
        if (model == null) return model;

        dbContext.GamePacks.Add(model);
        dbContext.SaveChanges();

        return model;
    }

    public IEnumerable<GamePack> Add(IEnumerable<GamePack> models)
    {
        var modelsToAdd = models.ToList();
        dbContext.GamePacks.AddRange(modelsToAdd);
        dbContext.SaveChanges();

        return modelsToAdd;
    }

    public GamePack? Update(GamePack? model)
    {
        if (model == null) return model;

        dbContext.GamePacks.Update(model);
        dbContext.SaveChanges();

        return model;
    }

    public void Update(Expression<Func<GamePack, bool>> where,
        Expression<Func<SetPropertyCalls<GamePack>, SetPropertyCalls<GamePack>>> setProperty) => 
        dbContext.GamePacks.Where(where).ExecuteUpdate(setProperty);

    public IEnumerable<GamePack> Update(IEnumerable<GamePack> models)
    {
        var modelsToUpdate = models.ToList();
        dbContext.GamePacks.UpdateRange(modelsToUpdate);
        dbContext.SaveChanges();

        return modelsToUpdate;
    }

    public void Delete(Expression<Func<GamePack, bool>> where) => dbContext.GamePacks.Where(where).ExecuteDelete();
}