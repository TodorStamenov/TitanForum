namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public static class AnswerExtensions
    {
        public static IEnumerable<Answer> Filter(this IEnumerable<Answer> answers, string searchTerm)
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