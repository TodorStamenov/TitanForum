namespace TitaniumForum.Data.Repositories
{
    using Data.Contracts;
    using Data.Infrastructure.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected readonly DbSet<TEntity> dbSet;
        protected readonly TitaniumForumDbContext context;

        public Repository(TitaniumForumDbContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            this.dbSet.AddRange(entities);
        }

        public void Delete(TEntity entity)
        {
            this.dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            this.dbSet.RemoveRange(entities);
        }

        public TEntity Find(int id)
        {
            return this.dbSet.Find(id);
        }

        public TEntity Find(int firstKey, int secondKey)
        {
            return this.dbSet.Find(firstKey, secondKey);
        }

        public bool Any(Expression<Func<TEntity, bool>> expression)
        {
            return this.dbSet.Any(expression);
        }

        public bool All(Expression<Func<TEntity, bool>> expression)
        {
            return this.dbSet.All(expression);
        }

        public int Count(Expression<Func<TEntity, bool>> expression = null)
        {
            return this.dbSet.CountElements(expression);
        }

        public TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            return this.ComposeQuery(
                    filter: filter,
                    orderBy: orderBy)
                .FirstOrDefault();
        }

        public IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null)
        {
            return this.ComposeQuery(
                    filter: filter,
                    orderBy: orderBy,
                    skip: skip,
                    take: take)
                .ToList();
        }

        public TResult ProjectSingle<TResult>(
            Expression<Func<TEntity, TResult>> projection,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            return this.ComposeQuery(
                    filter: filter,
                    orderBy: orderBy)
                .Select(projection)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> Project<TResult>(
            Expression<Func<TEntity, TResult>> projection,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null)
        {
            return this.ComposeQuery(
                    filter: filter,
                    orderBy: orderBy,
                    skip: skip,
                    take: take)
                .Select(projection)
                .ToList();
        }

        private IQueryable<TEntity> ComposeQuery(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null)
        {
            return this.dbSet
                .FilterElements(filter)
                .OrderElements(orderBy)
                .SkipElements(skip)
                .TakeElements(take);
        }
    }
}