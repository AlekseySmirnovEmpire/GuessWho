using System.Linq.Expressions;
using Core.Server.Database;
using Core.Server.Database.Files;
using Core.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories;

public class FileDataRepository(ApplicationDbContext dbContext) : IFileDataRepository
{
    public IQueryable<FileData> FindList(
        Expression<Func<FileData, bool>> where, 
        Func<IQueryable<FileData>, IQueryable<FileData>>? includes = null, 
        int? limit = null, 
        int? skip = null)
    {
        var query = dbContext.Files.Where(where);
        if (includes != null) query = includes(query);

        if (skip != null) query = query.Skip(skip.Value);

        if (limit != null) query = query.Take(limit.Value);

        return query;
    }

    public IEnumerable<FileData> FindAll(Func<IQueryable<FileData>, IQueryable<FileData>>? includes = null)
    {
        var query = dbContext.Files.AsQueryable();
        if (includes != null) query = includes(query);

        return query.ToList();
    }

    public FileData? Find(
        Expression<Func<FileData, bool>> where, 
        Func<IQueryable<FileData>, IQueryable<FileData>>? includes = null)
    {
        var query = dbContext.Files.Where(where);
        if (includes != null) query = includes(query);

        return query.FirstOrDefault();
    }

    public FileData? Add(FileData? model)
    {
        if (model == null) return model;

        dbContext.Files.Add(model);
        dbContext.SaveChanges();

        return model;
    }

    public IEnumerable<FileData> Add(IEnumerable<FileData> models)
    {
        var modelsToAdd = models.ToList();
        dbContext.Files.AddRange(modelsToAdd);
        dbContext.SaveChanges();

        return modelsToAdd;
    }

    public FileData? Update(FileData? model)
    {
        if (model == null) return model;

        dbContext.Files.Update(model);
        dbContext.SaveChanges();

        return model;
    }

    public void Update(
        Expression<Func<FileData, bool>> where, 
        Expression<Func<SetPropertyCalls<FileData>, SetPropertyCalls<FileData>>> setProperty) =>
        dbContext.Files.Where(where).ExecuteUpdate(setProperty);

    public IEnumerable<FileData> Update(IEnumerable<FileData> models)
    {
        var modelsToUpdate = models.ToList();
        dbContext.Files.UpdateRange(modelsToUpdate);
        dbContext.SaveChanges();

        return modelsToUpdate;
    }

    public void Delete(Expression<Func<FileData, bool>> where) => dbContext.Files.Where(where).ExecuteDelete();
}