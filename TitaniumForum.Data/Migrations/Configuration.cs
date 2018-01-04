namespace TitaniumForum.Data.Migrations
{
    using Common;
    using IdentityModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    internal sealed class Configuration : DbMigrationsConfiguration<TitaniumForumDbContext>
    {
        private const int AdminsCount = 1;
        private const int ModeratorsCount = 3;
        private const int UsersCount = 50;
        private const int CategoriesCount = 3;
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
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(TitaniumForumDbContext context)
        {
            var roleStore = new RoleStore<Role, int, UserRole>(context);
            var roleManager = new RoleManager<Role, int>(roleStore);

            var userStore = new UserStore<User, Role, int, UserLogin, UserRole, UserClaim>(context);
            var userManager = new UserManager<User, int>(userStore);

            userManager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 3,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            Assembly assembly = CommonConstants.loadWebAssembly;

            Task.Run(async () =>
            {
                await SeedRolesAsync(roleManager, context);
                await SeedUsersAsync(userManager, UsersCount, context);
                await SeedUsersAsync(userManager, roleManager, AdminsCount, CommonConstants.AdminRole, context);
                await SeedUsersAsync(userManager, roleManager, ModeratorsCount, CommonConstants.ModeratorRole, context);
                await SeedCategoriesAsync(CategoriesCount, context);
                await SeedSubCategoriesAsync(SubCategoriesCount, context);
                await SeedTagsAsync(TagsCount, context);
                await SeedQuestionsAsync(QuestionsCount, context);
                await SeedAnswersAsync(AnswersCount, context);
                await SeedCommentsAsync(CommentsCount, context);
            })
            .GetAwaiter()
            .GetResult();
        }

        private static async Task SeedRolesAsync(RoleManager<Role, int> roleManager, TitaniumForumDbContext context)
        {
            if (await context.Roles.AnyAsync())
            {
                return;
            }

            await roleManager.CreateAsync(new Role { Name = CommonConstants.AdminRole });
            await roleManager.CreateAsync(new Role { Name = CommonConstants.ModeratorRole });

            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(UserManager<User, int> userManager, int usersCount, TitaniumForumDbContext context)
        {
            if (await context.Users.AnyAsync(u => !u.Roles.Any()))
            {
                return;
            }

            for (int i = 1; i <= usersCount; i++)
            {
                string username = $"Username{i}";

                User user = new User
                {
                    UserName = username,
                    Email = $"{username}@{username}.com",
                    ProfileImage = CommonConstants.defaultUserImage
                };

                await userManager.CreateAsync(user, "123");
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUsersAsync(UserManager<User, int> userManager, RoleManager<Role, int> roleManager, int usersCount, string role, TitaniumForumDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Roles.Any(r => r.Role.Name == role)))
            {
                return;
            }

            for (int i = 1; i <= usersCount; i++)
            {
                string username = $"{role}{i}";

                User user = new User
                {
                    UserName = username,
                    Email = $"{username}@{username}.com",
                    ProfileImage = CommonConstants.defaultUserImage
                };

                await userManager.CreateAsync(user, "123");
                await userManager.AddToRoleAsync(user.Id, role);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(int categoriesCount, TitaniumForumDbContext context)
        {
            if (await context.Categories.AnyAsync())
            {
                return;
            }

            for (int i = 1; i <= categoriesCount; i++)
            {
                context.Categories.Add(new Category
                {
                    Name = $"Category Name {i}"
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedSubCategoriesAsync(int subCategoriesCount, TitaniumForumDbContext context)
        {
            if (await context.SubCategories.AnyAsync())
            {
                return;
            }

            List<int> categoryIds = await context.Categories.Select(c => c.Id).ToListAsync();

            for (int i = 1; i <= subCategoriesCount; i++)
            {
                context.SubCategories.Add(new SubCategory
                {
                    Name = $"Sub Category Name {i}",
                    CategoryId = categoryIds[random.Next(0, categoryIds.Count)]
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedTagsAsync(int tagsCount, TitaniumForumDbContext context)
        {
            if (await context.Tags.AnyAsync())
            {
                return;
            }

            for (int i = 1; i <= TagsCount; i++)
            {
                context.Tags.Add(new Tag
                {
                    Name = $"TagName{i}"
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedQuestionsAsync(int questionsCount, TitaniumForumDbContext context)
        {
            if (await context.Questions.AnyAsync())
            {
                return;
            }

            List<int> subCategoryIds = await context.SubCategories.Select(c => c.Id).ToListAsync();
            List<int> userIds = await context.Users.Select(u => u.Id).ToListAsync();
            List<int> tagIds = await context.Tags.Select(u => u.Id).ToListAsync();

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
                }

                context.Questions.Add(question);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedAnswersAsync(int answersCount, TitaniumForumDbContext context)
        {
            if (await context.Answers.AnyAsync())
            {
                return;
            }

            List<int> userIds = await context.Users.Select(u => u.Id).ToListAsync();
            var questionInfo = await context
                .Questions

                .Select(q => new
                {
                    q.Id,
                    q.DateAdded
                })
                .ToListAsync();

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
                }

                context.Answers.Add(answer);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedCommentsAsync(int commentsCount, TitaniumForumDbContext context)
        {
            if (await context.Comments.AnyAsync())
            {
                return;
            }

            List<int> userIds = await context.Users.Select(u => u.Id).ToListAsync();
            var answerInfo = await context
                .Answers

                .Select(q => new
                {
                    q.Id,
                    q.DateAdded
                })
                .ToListAsync();

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
                }

                context.Comments.Add(comment);
            }

            await context.SaveChangesAsync();
        }
    }
}