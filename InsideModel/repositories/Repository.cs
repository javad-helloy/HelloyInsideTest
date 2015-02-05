using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using InsideModel.Models;
using Ninject;

namespace InsideModel.repositories
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private InsideContext _context;
        private Func<InsideContext, DbSet<T>> _dbsetFinder;

        [Inject]
        public InsideContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public Repository(Func<InsideContext, DbSet<T>> dbsetFinder)
        {
            _dbsetFinder = dbsetFinder;
        }

        public Repository(Func<InsideContext, DbSet<T>> dbsetFinder, InsideContext context)
        {
            _dbsetFinder = dbsetFinder;
            Context = context;
        }

        public IQueryable<T> All()
        {
            return _dbsetFinder.Invoke(_context);
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbsetFinder.Invoke(_context).Where(expression);
        }

        public T Single(Expression<Func<T, bool>> expression)
        {
            return _dbsetFinder.Invoke(_context).Single(expression);
        }

        public T First(Expression<Func<T, bool>> expression)
        {
            return _dbsetFinder.Invoke(_context).First(expression);
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return _dbsetFinder.Invoke(_context).Any(expression);
        }

        public T Create()
        {
            return new T();
        }

        public void Add(T entity)
        {
            _dbsetFinder.Invoke(_context).Add(entity);
        }

        public void Delete(T entity)
        {
            _dbsetFinder.Invoke(_context).Remove(entity);
        }

        public void Attach(T entity)
        {
            _dbsetFinder.Invoke(_context).Attach(entity);
        }

        public void Detach(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public void SetState(T entity, EntityState state)
        {
            _context.Entry(entity).State = state;
        }

        public void Reload(T entity)
        {
            _context.Entry(entity).Reload();
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void SaveChangesAsync()
        {
            _context.SaveChangesAsync();
        }

    }
}
