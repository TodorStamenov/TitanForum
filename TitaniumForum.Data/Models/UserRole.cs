namespace TitaniumForum.Data.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;

    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; }

        public Role Role { get; set; }
    }
}