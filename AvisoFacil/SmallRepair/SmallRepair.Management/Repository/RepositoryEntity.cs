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
        public readonly SmallRepairDbContext Context;

        public RepositoryEntity(SmallRepairDbContext context)
        {
            this.Context = context;
        }

        public IQueryable<TEntity> All<TEntity>()
            where TEntity : class
        {
            return Context.Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> All<TEntity>(Expression<Func<TEntity, bool>> expression)
            where TEntity : class
        {
            return Context.Set<TEntity>().Where(expression).AsQueryable();
        }

        public void Add<TEntity>(params TEntity[] obj)
            where TEntity : class
        {
            Context.Set<TEntity>().AddRange(obj);
        }

        public void Delete<TEntity>(params TEntity[] obj)
            where TEntity : class
        {
            Context.Set<TEntity>().RemoveRange(obj);
        }

        public TEntity Find<TEntity>(int key)
            where TEntity : class
        {
            return Context.Find<TEntity>(key);
        }        

        public TEntity Find<TEntity>(string key)
            where TEntity : class
        {
            return Context.Find<TEntity>(key);
        }

        public void Update<TEntity>(params TEntity[] obj)
            where TEntity : class
        {
            Context.Set<TEntity>().UpdateRange(obj);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
