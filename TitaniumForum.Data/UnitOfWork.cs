namespace TitaniumForum.Data
{
    using Contracts;
    using Models;
    using Repositories;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly TitaniumForumDbContext context = new TitaniumForumDbContext();

        private IRepository<Category> categories;
        private IRepository<SubCategory> subCategories;
        private IRepository<Question> questions;
        private IRepository<Answer> answers;
        private IRepository<Comment> comments;
        private IRepository<Tag> tags;

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

        public void Save()
        {
            this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}