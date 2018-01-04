namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Linq;

    public static class QuestionExtensions
    {
        public static IQueryable<Question> Filter(this IQueryable<Question> questions, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm)
                && !string.IsNullOrWhiteSpace(searchTerm))
            {
                return questions
                    .Where(q => q.Title.ToLower().Contains(searchTerm)
                    || q.Content.ToLower().Contains(searchTerm)
                    || q.Tags.Any(t => t.Tag.Name.ToLower().Contains(searchTerm)));
            }

            return questions;
        }
    }
}