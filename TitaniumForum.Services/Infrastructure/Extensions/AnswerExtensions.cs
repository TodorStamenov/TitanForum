namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Linq;

    public static class AnswerExtensions
    {
        public static IQueryable<Answer> Filter(this IQueryable<Answer> answers, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm)
                && !string.IsNullOrWhiteSpace(searchTerm))
            {
                return answers
                    .Where(q => q.Content.ToLower().Contains(searchTerm));
            }

            return answers;
        }
    }
}