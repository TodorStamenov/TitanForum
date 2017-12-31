namespace TitaniumForum.Data.Repositories
{
    using Contracts;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly TitaniumForumDbContext context;

        public Repository(TitaniumForumDbContext context)
        {
            this.context = context;
        }

        public void Add(TEntity entity)
        {
            this.context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            this.context.Set<TEntity>().AddRange(entities);
        }

        public void Delete(TEntity entity)
        {
            this.context.Set<TEntity>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            this.context.Set<TEntity>().RemoveRange(entities);
        }

        public TEntity Find(int id)
        {
            return this.context.Set<TEntity>().Find(id);
        }

        public TEntity Find(int firstKey, int secondKey)
        {
            return this.context.Set<TEntity>().Find(firstKey, secondKey);
        }

        public IQueryable<TEntity> AllEntries()
        {
            return this.context.Set<TEntity>().AsQueryable();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.context.Set<TEntity>().AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}