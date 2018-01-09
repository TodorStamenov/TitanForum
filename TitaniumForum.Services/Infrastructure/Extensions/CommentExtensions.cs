namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Linq;

    public static class CommentExtensions
    {
        public static IQueryable<Comment> Filter(this IQueryable<Comment> comments, string searchTerm)
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