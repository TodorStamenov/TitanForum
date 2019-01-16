namespace TitaniumForum.Data
{
    using Contracts;
    using Models;
    using TitaniumForum.Data.Repositories;

    public class Database : IDatabase
    {
        private readonly TitaniumForumDbContext context;

        public Database(TitaniumForumDbContext context)
        {
            this.context = context;
        }

        public IRepository<Category> Categories => new Repository<Category>(this.context);

        public IRepository<SubCategory> SubCategories => new Repository<SubCategory>(this.context);

        public IRepository<Question> Questions => new Repository<Question>(this.context);

        public IRepository<Answer> Answers => new Repository<Answer>(this.context);

        public IRepository<Comment> Comments => new Repository<Comment>(this.context);

        public IRepository<Tag> Tags => new Repository<Tag>(this.context);

        public IRepository<User> Users => new Repository<User>(this.context);

        public IRepository<Role> Roles => new Repository<Role>(this.context);

        public IRepository<UserRole> UserRoles => new Repository<UserRole>(this.context);

        public IRepository<Log> Logs => new Repository<Log>(this.context);

        public void Save() => this.context.SaveChanges();

        public void Dispose() => this.context.Dispose();
    }
}