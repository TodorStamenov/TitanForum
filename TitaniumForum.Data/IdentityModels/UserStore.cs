namespace TitaniumForum.Data.IdentityModels
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    public class UserStore : UserStore<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore(TitaniumForumDbContext context)
            : base(context)
        {
        }
    }
}