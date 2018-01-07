namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Data.Models;
    using Models.Questions;
    using System.Collections.Generic;
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

        public static IEnumerable<ListQuestionsServiceModel> ProjectToListModel(this IQueryable<Question> questions)
        {
            return questions
                .AsEnumerable()
                .Select(q => new ListQuestionsServiceModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    DateAdded = q.DateAdded.ToLocalTime(),
                    AnswersCount = q.Answers.Count(a => !a.IsDeleted),
                    SubCategoryId = q.SubCategoryId,
                    SubCategoryName = q.SubCategory.Name,
                    AuthorUsername = q.Author.UserName,
                    AuthorProfileImage = q.Author.ProfileImage.ConvertImage(),
                    ViewCount = q.ViewCount,
                    UpVotes = q.Votes.Count(v => v.Direction == Direction.Like),
                    DownVotes = q.Votes.Count(v => v.Direction == Direction.Dislike),
                    LastUserUsername = GetLastAnswer(q)?.Author.UserName,
                    LastUserProfileImage = GetLastAnswer(q)?.Author.ProfileImage.ConvertImage(),
                    LastAnswerDate = GetLastAnswer(q)?.DateAdded.ToLocalTime()
                });
        }

        private static Answer GetLastAnswer(Question question)
        {
            if (!question.Answers.Where(a => !a.IsDeleted).Any())
            {
                return null;
            }

            return question
                .Answers
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.DateAdded)
                .Last();
        }
    }
}