namespace TitaniumForum.Data
{
    using IdentityModels;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System.Data.Entity;

    public class TitaniumForumDbContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public TitaniumForumDbContext()
            : base("data source=.;initial catalog=TitaniumForum;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            builder
                .Entity<User>()
                .HasMany(u => u.Roles)
                .WithRequired(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);

            builder
                .Entity<Role>()
                .HasMany(r => r.Users)
                .WithRequired(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId);
        }

        public static TitaniumForumDbContext Create()
        {
            return new TitaniumForumDbContext();
        }
    }
}