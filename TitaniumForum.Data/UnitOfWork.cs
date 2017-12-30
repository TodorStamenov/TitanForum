namespace TitaniumForum.Data
{
    using Contracts;
    using Models;
    using Repositories;
    using System.Threading.Tasks;

    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<Category> categories;
        private IRepository<SubCategory> subCategories;
        private IRepository<Question> questions;
        private IRepository<Answer> answers;
        private IRepository<Comment> comments;
        private IRepository<Tag> tags;
        private IRepository<User> users;
        private IRepository<Role> roles;
        private IRepository<UserRole> userRoles;

        private readonly TitaniumForumDbContext context;

        public UnitOfWork(TitaniumForumDbContext context)
        {
            this.context = context;
        }

        public IRepository<Category> Categories
        {
            get { return this.categories ?? (this.categories = new Repository<Category>(this.context)); }
        }

        public IRepository<SubCategory> SubCategories
        {
            get { return this.subCategories ?? (this.subCategories = new Repository<SubCategory>(this.context)); }
        }

        public IRepository<Question> Questions
        {
            get { return this.questions ?? (this.questions = new Repository<Question>(this.context)); }
        }

        public IRepository<Answer> Answers
        {
            get { return this.answers ?? (this.answers = new Repository<Answer>(this.context)); }
        }

        public IRepository<Comment> Comments
        {
            get { return this.comments ?? (this.comments = new Repository<Comment>(this.context)); }
        }

        public IRepository<Tag> Tags
        {
            get { return this.tags ?? (this.tags = new Repository<Tag>(this.context)); }
        }

        public IRepository<User> Users
        {
            get { return this.users ?? (this.users = new Repository<User>(this.context)); }
        }

        public IRepository<Role> Roles
        {
            get { return this.roles ?? (this.roles = new Repository<Role>(this.context)); }
        }

        public IRepository<UserRole> UserRoles
        {
            get { return this.userRoles ?? (this.userRoles = new Repository<UserRole>(this.context)); }
        }

        public void Save()
        {
            this.context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await this.context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}