using Microsoft.EntityFrameworkCore;
using SmallRepair.Management.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SmallRepair.Management.Repository
{
    public class RepositoryEntity
    {
        private readonly SmallRepairDbContext _context;

        public RepositoryEntity(SmallRepairDbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> All<TEntity>()
            where TEntity : class
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> All<TEntity>(Expression<Func<TEntity, bool>> expression)
            where TEntity : class
        {
            return _context.Set<TEntity>().Where(expression).AsQueryable();
        }

        public void Add<TEntity>(params TEntity[] obj)
            where TEntity : class
        {
            _context.Set<TEntity>().AddRange(obj);
        }

        public void Delete<TEntity>(params TEntity[] obj)
            where TEntity : class
        {
            _context.Set<TEntity>().RemoveRange(obj);
        }

        public TEntity Find<TEntity>(int key)
            where TEntity : class
        {
            return _context.Find<TEntity>(key);
        }

        public void Update<TEntity>(params TEntity[] obj)
            where TEntity : class
        {
            _context.Set<TEntity>().UpdateRange(obj);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
