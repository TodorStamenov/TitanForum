namespace TitaniumForum.Data
{
    using IdentityModels;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Migrations;
    using ModelConfigurations;
    using Models;
    using System.Data.Entity;

    public class TitaniumForumDbContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public TitaniumForumDbContext()
            : base("data source=.;initial catalog=TitaniumForum;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TitaniumForumDbContext, Configuration>());
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<SubCategory> SubCategories { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Configurations.Add(new TagConfiguration());
            builder.Configurations.Add(new TagQuestionConfiguration());
            builder.Configurations.Add(new UserAnswerConfiguration());
            builder.Configurations.Add(new UserCommentConfiguration());
            builder.Configurations.Add(new UserQuestionConfiguration());
            builder.Configurations.Add(new UserConfiguration());
            builder.Configurations.Add(new CategoryConfiguration());
            builder.Configurations.Add(new SubCategoryConfiguration());
            builder.Configurations.Add(new QuestionConfiguration());
            builder.Configurations.Add(new AnswerConfiguraiton());
        }

        public static TitaniumForumDbContext Create()
        {
            return new TitaniumForumDbContext();
        }
    }
}