namespace TitaniumForum.Data.IdentityModels
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    public class RoleStore : RoleStore<Role, int, UserRole>
    {
        public RoleStore(TitaniumForumDbContext context)
            : base(context)
        {
        }
    }
}