using System.Linq.Expressions;
using Core.Database;
using Core.Database.Email;
using Core.Server.Database;
using Core.Server.Database.Email;
using Core.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories;

public class EmailSendingQueueRepository(ApplicationDbContext dbContext) : IEmailSendingQueueRepository
{
    public IQueryable<EmailSendingQueue> FindList(
        Expression<Func<EmailSendingQueue, bool>> where,
        Func<IQueryable<EmailSendingQueue>, IQueryable<EmailSendingQueue>>? includes = null,
        int? limit = null,
        int? skip = null)
    {
        var query = dbContext.EmailSendingQueue.Where(where);
        if (includes != null) query = includes(query);

        if (skip != null) query = query.Skip(skip.Value);

        if (limit != null) query = query.Take(limit.Value);

        return query;
    }

    public IEnumerable<EmailSendingQueue> FindAll(
        Func<IQueryable<EmailSendingQueue>, IQueryable<EmailSendingQueue>>? includes = null)
    {
        var query = dbContext.EmailSendingQueue.AsQueryable();
        if (includes != null) query = includes(query);

        return query.ToList();
    }

    public EmailSendingQueue? Find(
        Expression<Func<EmailSendingQueue, bool>> where,
        Func<IQueryable<EmailSendingQueue>, IQueryable<EmailSendingQueue>>? includes = null)
    {
        var query = dbContext.EmailSendingQueue.Where(where);
        if (includes != null) query = includes(query);

        return query.FirstOrDefault();
    }

    public EmailSendingQueue? Add(EmailSendingQueue? model)
    {
        if (model == null) return model;

        dbContext.EmailSendingQueue.Add(model);
        dbContext.SaveChanges();

        return model;
    }

    public IEnumerable<EmailSendingQueue> Add(IEnumerable<EmailSendingQueue> models)
    {
        var modelsToAdd = models.ToList();
        dbContext.EmailSendingQueue.AddRange(modelsToAdd);
        dbContext.SaveChanges();

        return modelsToAdd;
    }

    public EmailSendingQueue? Update(EmailSendingQueue? model)
    {
        if (model == null) return model;

        dbContext.EmailSendingQueue.Update(model);
        dbContext.SaveChanges();

        return model;
    }

    public void Update(
        Expression<Func<EmailSendingQueue, bool>> where,
        Expression<Func<SetPropertyCalls<EmailSendingQueue>, SetPropertyCalls<EmailSendingQueue>>> setProperty) =>
        dbContext.EmailSendingQueue.Where(where).ExecuteUpdate(setProperty);

    public IEnumerable<EmailSendingQueue> Update(IEnumerable<EmailSendingQueue> models)
    {
        var modelsToUpdate = models.ToList();
        dbContext.EmailSendingQueue.UpdateRange(modelsToUpdate);
        dbContext.SaveChanges();

        return modelsToUpdate;
    }

    public void Delete(Expression<Func<EmailSendingQueue, bool>> where) =>
        dbContext.EmailSendingQueue.Where(where).ExecuteDelete();
}