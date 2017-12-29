namespace TitaniumForum.Data.Migrations
{
    using Common;
    using IdentityModels;
    using Microsoft.AspNet.Identity;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TitaniumForumDbContext>
    {
        private const int AdminsCount = 1;
        private const int ModeratorsCount = 3;
        private const int UsersCount = 100;
        private const int CategoriesCount = 5;
        private const int SubCategoriesCount = CategoriesCount * 5;
        private const int QuestionsCount = SubCategoriesCount * 5;
        private const int AnswersCount = QuestionsCount * 5;
        private const int CommentsCount = QuestionsCount;
        private const int TagsCount = 50;
        private const int MinTagsPerQuestion = 3;
        private const int MaxTagsPerQuestion = 10;
        private const int MinViewsPerQuestion = 0;
        private const int MaxViewsPerQuestion = 200;
        private const int MinVotesPerQuestion = 10;
        private const int MaxVotesPerQuestion = 50;
        private const int MinVotesPerAnswer = 10;
        private const int MaxVotesPerAnswer = 50;
        private const int MinVotesPerComment = 10;
        private const int MaxVotesPerComment = 50;

        private static readonly Random random = new Random();

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(TitaniumForumDbContext context)
        {
            UserManager<User, int> userManager = new UserManager<User, int>(new UserStore(context));

            if (!context.Roles.Any())
            {
                this.SeedRoles(context);
            }

            if (!context.Users.Any())
            {
                this.SeedUsers(UsersCount, userManager, context);
            }

            if (!context.Users.Any(u => u.Roles.Any(r => r.Role.Name == CommonConstants.AdminRole)))
            {
                this.SeedUsers(AdminsCount, CommonConstants.AdminRole, userManager, context);
            }

            if (!context.Users.Any(u => u.Roles.Any(r => r.Role.Name == CommonConstants.ModeratorRole)))
            {
                this.SeedUsers(ModeratorsCount, CommonConstants.ModeratorRole, userManager, context);
            }

            if (!context.Categories.Any())
            {
                this.SeedCategories(CategoriesCount, context);
            }

            if (!context.SubCategories.Any())
            {
                this.SeedSubCategories(SubCategoriesCount, context);
            }

            if (!context.Tags.Any())
            {
                this.SeedTags(TagsCount, context);
            }

            if (!context.Questions.Any())
            {
                this.SeedQuestions(QuestionsCount, context);
            }

            if (!context.Answers.Any())
            {
                this.SeedAnswers(AnswersCount, context);
            }

            if (!context.Comments.Any())
            {
                this.SeedComments(CommentsCount, context);
            }
        }

        private void SeedRoles(TitaniumForumDbContext context)
        {
            context.Roles.Add(new Role { Name = CommonConstants.AdminRole });
            context.Roles.Add(new Role { Name = CommonConstants.ModeratorRole });

            context.SaveChanges();
        }

        private void SeedUsers(int usersCount, string role, UserManager<User, int> userManager, TitaniumForumDbContext context)
        {
            int roleId = context.Roles.FirstOrDefault(r => r.Name == role).Id;

            for (int i = 1; i <= usersCount; i++)
            {
                string username = $"{role}{i}";

                User user = new User
                {
                    UserName = username,
                    Email = $"{username}@{username}.com",
                    PasswordHash = userManager.PasswordHasher.HashPassword("123"),
                    ProfileImage = CommonConstants.defaultUserImage
                };

                context.Users.Add(user);
                user.Roles.Add(new UserRole
                {
                    RoleId = roleId
                });
            }

            context.SaveChanges();
        }

        private void SeedUsers(int usersCount, UserManager<User, int> userManager, TitaniumForumDbContext context)
        {
            for (int i = 1; i <= usersCount; i++)
            {
                string username = $"Username{i}";

                User user = new User
                {
                    UserName = username,
                    Email = $"{username}@{username}.com",
                    PasswordHash = userManager.PasswordHasher.HashPassword("123")
                };

                context.Users.Add(user);
            }

            context.SaveChanges();
        }

        private void SeedCategories(int categoriesCount, TitaniumForumDbContext context)
        {
            for (int i = 1; i <= categoriesCount; i++)
            {
                context.Categories.Add(new Category
                {
                    Name = $"Category Name {i}"
                });
            }

            context.SaveChanges();
        }

        private void SeedSubCategories(int subCategoriesCount, TitaniumForumDbContext context)
        {
            List<int> categoryIds = context.Categories.Select(c => c.Id).ToList();

            for (int i = 1; i <= subCategoriesCount; i++)
            {
                context.SubCategories.Add(new SubCategory
                {
                    Name = $"Sub Category Name {i}",
                    CategoryId = categoryIds[random.Next(0, categoryIds.Count)]
                });
            }

            context.SaveChanges();
        }

        private void SeedTags(int tagsCount, TitaniumForumDbContext context)
        {
            for (int i = 1; i <= TagsCount; i++)
            {
                context.Tags.Add(new Tag
                {
                    Name = $"TagName{i}"
                });
            }

            context.SaveChanges();
        }

        private void SeedQuestions(int questionsCount, TitaniumForumDbContext context)
        {
            List<int> subCategoryIds = context.SubCategories.Select(c => c.Id).ToList();
            List<int> userIds = context.Users.Select(u => u.Id).ToList();
            List<int> tagIds = context.Tags.Select(u => u.Id).ToList();

            for (int i = 1; i <= questionsCount; i++)
            {
                Question question = new Question
                {
                    Title = $"Question Title {i}",
                    Content = CommonConstants.lorem,
                    DateAdded = DateTime.UtcNow.AddDays(-i).AddHours(-i).AddMinutes(-i),
                    ViewCount = random.Next(MinViewsPerQuestion, MaxViewsPerQuestion),
                    AuthorId = userIds[random.Next(0, userIds.Count)],
                    SubCategoryId = subCategoryIds[random.Next(0, subCategoryIds.Count)]
                };

                int tagsPerQuestion = random.Next(MinTagsPerQuestion, MaxTagsPerQuestion);

                for (int j = 0; j < tagsPerQuestion; j++)
                {
                    int tagId = tagIds[random.Next(0, tagIds.Count)];

                    if (question.Tags.Any(t => t.TagId == tagId))
                    {
                        j--;
                        continue;
                    }

                    question.Tags.Add(new TagQuestion
                    {
                        TagId = tagId
                    });
                }

                int votesPerQuestion = random.Next(MinVotesPerQuestion, MaxVotesPerQuestion);

                for (int j = 0; j < votesPerQuestion; j++)
                {
                    int userId = userIds[random.Next(0, userIds.Count)];

                    if (question.Votes.Any(v => v.UserId == userId))
                    {
                        j--;
                        continue;
                    }

                    Direction direction = (Direction)random.Next(0, typeof(Direction).GetEnumValues().Length);

                    question.Votes.Add(new UserQuestionVote
                    {
                        UserId = userId,
                        Direction = direction
                    });

                    int likesCount = question.Votes.Count(v => v.Direction == Direction.Like);
                    int dislikesCount = question.Votes.Count(v => v.Direction == Direction.Dislike);

                    question.Rating = likesCount - dislikesCount;
                }

                context.Questions.Add(question);
            }

            context.SaveChanges();
        }

        private void SeedAnswers(int answersCount, TitaniumForumDbContext context)
        {
            List<int> userIds = context.Users.Select(u => u.Id).ToList();
            var questionInfo = context
                .Questions
                .Select(q => new
                {
                    q.Id,
                    q.DateAdded
                })
                .ToList();

            for (int i = 0; i < answersCount; i++)
            {
                var question = questionInfo[random.Next(0, questionInfo.Count)];

                Answer answer = new Answer
                {
                    Content = CommonConstants.lorem.Substring(0, CommonConstants.lorem.Length / 2),
                    AuthorId = userIds[random.Next(0, userIds.Count)],
                    DateAdded = question.DateAdded.AddHours(i).AddMinutes(i),
                    QuestionId = question.Id
                };

                int votesPerAnswer = random.Next(MinVotesPerAnswer, MaxVotesPerAnswer);

                for (int j = 0; j < votesPerAnswer; j++)
                {
                    int userId = userIds[random.Next(0, userIds.Count)];

                    if (answer.Votes.Any(v => v.UserId == userId))
                    {
                        j--;
                        continue;
                    }

                    Direction direction = (Direction)random.Next(0, typeof(Direction).GetEnumValues().Length);

                    answer.Votes.Add(new UserAnswerVote
                    {
                        UserId = userId,
                        Direction = direction
                    });

                    int likesCount = answer.Votes.Count(v => v.Direction == Direction.Like);
                    int dislikesCount = answer.Votes.Count(v => v.Direction == Direction.Dislike);

                    answer.Rating = likesCount - dislikesCount;
                }

                context.Answers.Add(answer);
            }

            context.SaveChanges();
        }

        private void SeedComments(int commentsCount, TitaniumForumDbContext context)
        {
            List<int> userIds = context.Users.Select(u => u.Id).ToList();
            var answerInfo = context
                .Answers
                .Select(q => new
                {
                    q.Id,
                    q.DateAdded
                })
                .ToList();

            for (int i = 0; i < commentsCount; i++)
            {
                var answer = answerInfo[random.Next(0, answerInfo.Count)];

                Comment comment = new Comment
                {
                    Content = CommonConstants.lorem.Substring(0, CommonConstants.lorem.Length / 4),
                    AuthorId = userIds[random.Next(0, userIds.Count)],
                    DateAdded = answer.DateAdded.AddHours(i).AddMinutes(i),
                    AnswerId = answer.Id
                };

                int votesPerComment = random.Next(MinVotesPerComment, MaxVotesPerComment);

                for (int j = 0; j < votesPerComment; j++)
                {
                    int userId = userIds[random.Next(0, userIds.Count)];

                    if (comment.Votes.Any(v => v.UserId == userId))
                    {
                        j--;
                        continue;
                    }

                    Direction direction = (Direction)random.Next(0, typeof(Direction).GetEnumValues().Length);

                    comment.Votes.Add(new UserCommentVote
                    {
                        UserId = userId,
                        Direction = direction
                    });

                    int likesCount = comment.Votes.Count(v => v.Direction == Direction.Like);
                    int dislikesCount = comment.Votes.Count(v => v.Direction == Direction.Dislike);

                    comment.Rating = likesCount - dislikesCount;
                }

                context.Comments.Add(comment);
            }

            context.SaveChanges();
        }
    }
}