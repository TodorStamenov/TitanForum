namespace TitaniumForum.Data.Models
{
    using IdentityModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}