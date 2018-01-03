namespace TitaniumForum.Data.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;

    public class UserRole : IdentityUserRole<int>
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}