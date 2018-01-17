namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public static class CommentExtensions
    {
        public static IEnumerable<Comment> Filter(this IEnumerable<Comment> comments, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm)
                && !string.IsNullOrWhiteSpace(searchTerm))
            {
                return comments
                    .Where(q => q.Content.ToLower().Contains(searchTerm));
            }

            return comments;
        }
    }
}