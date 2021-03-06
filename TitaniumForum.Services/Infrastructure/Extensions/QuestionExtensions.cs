﻿namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using Areas.Moderator.Models.Questions;
    using Data.Models;
    using Models.Questions;
    using System.Collections.Generic;
    using System.Linq;

    public static class QuestionExtensions
    {
        public static IEnumerable<ListQuestionsServiceModel> ProjectToListModel(this IEnumerable<Question> questions)
        {
            return questions
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

        public static IEnumerable<ListDeletedQuestionsServiceModel> ProjectToDeletedListModel(this IEnumerable<Question> questions)
        {
            return questions
                .Select(q => new ListDeletedQuestionsServiceModel
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
                    IsDeleted = q.IsDeleted,
                    IsSubCategoryDeleted = q.SubCategory.IsDeleted,
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