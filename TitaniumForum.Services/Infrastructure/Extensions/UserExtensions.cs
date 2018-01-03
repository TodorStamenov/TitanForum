namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Linq;

    public static class UserExtensions
    {
        public static IQueryable<User> InRole(this IQueryable<User> users, string role)
        {
            if (!string.IsNullOrEmpty(role)
                && !string.IsNullOrWhiteSpace(role))
            {
                return users
                    .Where(u => u.Roles
                        .Any(r => r.Role.Name.ToLower() == role.ToLower()));
            }

            return users;
        }

        public static IQueryable<User> Filter(this IQueryable<User> users, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm)
                && !string.IsNullOrWhiteSpace(searchTerm))
            {
                return users
                    .Where(u => u.UserName.ToLower().Contains(searchTerm));
            }

            return users;
        }
    }
}