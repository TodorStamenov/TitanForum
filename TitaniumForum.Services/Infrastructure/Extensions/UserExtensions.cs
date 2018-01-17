namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public static class UserExtensions
    {
        public static IEnumerable<User> InRole(this IEnumerable<User> users, string role)
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

        public static IEnumerable<User> Filter(this IEnumerable<User> users, string searchTerm)
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