namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public static class LogExtensions
    {
        public static IEnumerable<Log> Filter(this IEnumerable<Log> logs, string searchTerm)
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