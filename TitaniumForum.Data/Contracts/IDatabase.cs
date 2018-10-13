namespace TitaniumForum.Data.Contracts
{
    using System;
    using TitaniumForum.Data.Models;

    public interface IDatabase : IDisposable
    {
        IRepository<Category> Categories { get; }

        IRepository<SubCategory> SubCategories { get; }

        IRepository<Question> Questions { get; }

        IRepository<Answer> Answers { get; }

        IRepository<Comment> Comments { get; }

        IRepository<Tag> Tags { get; }

        IRepository<User> Users { get; }

        IRepository<Role> Roles { get; }

        IRepository<UserRole> UserRoles { get; }

        IRepository<Log> Logs { get; }

        void Save();
    }
}