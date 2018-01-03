namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Linq;

    public static class LogExtensions
    {
        public static IQueryable<Log> Filter(this IQueryable<Log> logs, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm)
                && !string.IsNullOrWhiteSpace(searchTerm))
            {
                return logs
                    .Where(l => l.Username.ToLower()
                        .Contains(searchTerm.ToLower()));
            }

            return logs;
        }
    }
}