using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Repositories;

public interface IRepository<T> where T : class
{
    public IQueryable<T> FindList(
        Expression<Func<T, bool>> where,
        Func<IQueryable<T>, IQueryable<T>>? includes = null,
        int? limit = null,
        int? skip = null);

    public IEnumerable<T> FindAll(Func<IQueryable<T>, IQueryable<T>>? includes = null);

    public T? Find(Expression<Func<T, bool>> where, Func<IQueryable<T>, IQueryable<T>>? includes = null);

    public T? Add(T? model);

    public IEnumerable<T> Add(IEnumerable<T> models);

    public T? Update(T? model);

    public void Update(
        Expression<Func<T, bool>> where,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperty);

    public IEnumerable<T> Update(IEnumerable<T> models);

    public void Delete(Expression<Func<T, bool>> where);
}