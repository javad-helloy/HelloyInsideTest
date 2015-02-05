using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using InsideModel.repositories;

namespace Helpers.test
{
    public class LocalRepository<T> : IRepository<T> where T : class, new() 
    {
        private List<T> _collection;

        public LocalRepository()
        {
            _collection = new List<T>();
        }

        public IQueryable<T> All()
        {
            return _collection.AsQueryable();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Where(expression).AsQueryable();
        }

        public T Single(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Single(expression);
        }

        public T First(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().First(expression);
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Any(expression);
        }

        public virtual T Create()
        {
            return new T();
        }

        public void Add(T entity)
        {
           _collection.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            _collection.Remove(entity);
        }

        public virtual void Attach(T entity)
        {
            _collection.Add(entity);
        }

        public virtual void Detach(T entity)
        {
            _collection.Remove(entity);
        }

        public void Reload(T entity)
        {
            ;
        }

        public void SetState(T entity, EntityState state)
        {
            ;
        }

        public virtual void SaveChanges()
        {
            ;
        }

        public void SaveChangesAsync()
        {
            ;
        }
    }
}
