namespace TitaniumForum.Data.Contracts
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IRepository<TEntity> : IEnumerable<TEntity> where TEntity : class
    {
        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        TEntity Find(int id);

        IQueryable<TEntity> All();
    }
}