namespace TitaniumForum.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IRepository<TEntity> where TEntity : class, new()
    {
        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        bool All(Expression<Func<TEntity, bool>> expression);

        bool Any(Expression<Func<TEntity, bool>> expression);

        int Count(Expression<Func<TEntity, bool>> expression = null);

        void Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        TEntity Find(int id);

        TEntity Find(int firstKey, int secondKey);

        TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null);

        TResult ProjectSingle<TResult>(
            Expression<Func<TEntity, TResult>> projection,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        IEnumerable<TResult> Project<TResult>(
           Expression<Func<TEntity, TResult>> projection,
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           int? skip = null,
           int? take = null);
    }
}