namespace TitaniumForum.Data
{
    using Contracts;
    using Models;
    using Repositories;
    using System;
    using System.Collections.Generic;

    public class Database : IDatabase
    {
        private readonly TitaniumForumDbContext context;
        private readonly IDictionary<Type, object> repositories;

        public Database(TitaniumForumDbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<Category> Categories => this.GetRepository<Category>();

        public IRepository<SubCategory> SubCategories => this.GetRepository<SubCategory>();

        public IRepository<Question> Questions => this.GetRepository<Question>();

        public IRepository<Answer> Answers => this.GetRepository<Answer>();

        public IRepository<Comment> Comments => this.GetRepository<Comment>();

        public IRepository<Tag> Tags => this.GetRepository<Tag>();

        public IRepository<User> Users => this.GetRepository<User>();

        public IRepository<Role> Roles => this.GetRepository<Role>();

        public IRepository<UserRole> UserRoles => this.GetRepository<UserRole>();

        public IRepository<Log> Logs => this.GetRepository<Log>();

        public void Save() => this.context.SaveChanges();

        public void Dispose() => this.context.Dispose();

        private IRepository<T> GetRepository<T>() where T : class, new()
        {
            Type repositoryType = typeof(T);

            if (!this.repositories.ContainsKey(repositoryType))
            {
                this.repositories[repositoryType] = (IRepository<T>)Activator.CreateInstance(typeof(Repository<T>), this.context);
            }

            return (IRepository<T>)this.repositories[repositoryType];
        }
    }
}