using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InsideModel.repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> All();
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        
        T Single(Expression<Func<T, bool>> expression);
        T First(Expression<Func<T, bool>> expression);
        bool Any(Expression<Func<T, bool>> expression);

        T Create();

        void Add(T entity);
        void Delete(T entity);
        void Attach(T entity);
        void Detach(T entity);
        void Reload(T entity);
        void SetState(T entity, EntityState state);

        void SaveChanges();

        void SaveChangesAsync();
    }
}
