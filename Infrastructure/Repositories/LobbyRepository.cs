using System.Linq.Expressions;
using Core.Server.Database;
using Core.Server.Database.Lobbies;
using Core.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories;

public class LobbyRepository(ApplicationDbContext dbContext) : ILobbyRepository
{
    public IQueryable<Lobby> FindList(Expression<Func<Lobby, bool>> where,
        Func<IQueryable<Lobby>, IQueryable<Lobby>>? includes = null, int? limit = null, int? skip = null)
    {
        var query = dbContext.Lobbies.Where(where);
        if (includes != null) query = includes(query);

        if (skip != null) query = query.Skip(skip.Value);

        if (limit != null) query = query.Take(limit.Value);

        return query;
    }

    public IEnumerable<Lobby> FindAll(Func<IQueryable<Lobby>, IQueryable<Lobby>>? includes = null)
    {
        var query = dbContext.Lobbies.AsQueryable();
        if (includes != null) query = includes(query);

        return query.ToList();
    }

    public Lobby? Find(Expression<Func<Lobby, bool>> where, Func<IQueryable<Lobby>, IQueryable<Lobby>>? includes = null)
    {
        var query = dbContext.Lobbies.Where(where);
        if (includes != null) query = includes(query);

        return query.FirstOrDefault();
    }

    public Lobby? Add(Lobby? model)
    {
        if (model == null) return model;

        dbContext.Lobbies.Add(model);
        dbContext.SaveChanges();

        return model;
    }

    public IEnumerable<Lobby> Add(IEnumerable<Lobby> models)
    {
        var modelsToAdd = models.ToList();
        dbContext.Lobbies.AddRange(modelsToAdd);
        dbContext.SaveChanges();

        return modelsToAdd;
    }

    public Lobby? Update(Lobby? model)
    {
        if (model == null) return model;

        dbContext.Lobbies.Update(model);
        dbContext.SaveChanges();

        return model;
    }

    public void Update(Expression<Func<Lobby, bool>> where,
        Expression<Func<SetPropertyCalls<Lobby>, SetPropertyCalls<Lobby>>> setProperty) =>
        dbContext.Lobbies.Where(where).ExecuteUpdate(setProperty);

    public IEnumerable<Lobby> Update(IEnumerable<Lobby> models)
    {
        var modelsToUpdate = models.ToList();
        dbContext.Lobbies.UpdateRange(modelsToUpdate);
        dbContext.SaveChanges();

        return modelsToUpdate;
    }

    public void Delete(Expression<Func<Lobby, bool>> where) => dbContext.Lobbies.Where(where).ExecuteDelete();
}