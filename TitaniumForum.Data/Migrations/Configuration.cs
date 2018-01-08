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
        private const int SubCategoriesCount = CategoriesCount * 3;
        private const int QuestionsCount = SubCategoriesCount * 20;
        private const int AnswersCount = QuestionsCount * 5;
        private const int CommentsCount = QuestionsCount;
        private const int TagsCount = 50;
        private const int MinTagsPerQuestion = 3;
        private const int MaxTagsPerQuestion = 10;
        private const int MinViewsPerQuestion = 0;
        private const int MaxViewsPerQuestion = 200;
        private const int MinVotesPerQuestion = 0;
        private const int MaxVotesPerQuestion = 11;
        private const int MinVotesPerAnswer = 0;
        private const int MaxVotesPerAnswer = 1;
        private const int MinVotesPerComment = 0;
        private const int MaxVotesPerComment = 11;

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

            Assembly assembly = CommonConstants.webAssembly;

            Task.Run(async () =>
            {
                await this.SeedRolesAsync(roleManager, context);
                await this.SeedUsersAsync(userManager, UsersCount, context);
                await this.SeedUsersAsync(userManager, roleManager, AdminsCount, CommonConstants.AdminRole, context);
                await this.SeedUsersAsync(userManager, roleManager, ModeratorsCount, CommonConstants.ModeratorRole, context);
                await this.SeedCategoriesAsync(CategoriesCount, context);
                await this.SeedSubCategoriesAsync(SubCategoriesCount, context);
                await this.SeedTagsAsync(TagsCount, context);
                await this.SeedQuestionsAsync(QuestionsCount, context);
                await this.SeedAnswersAsync(AnswersCount, context);
                await this.SeedCommentsAsync(CommentsCount, context);
            })
            .GetAwaiter()
            .GetResult();
        }

        private async Task SeedRolesAsync(RoleManager<Role, int> roleManager, TitaniumForumDbContext context)
        {
            if (await context.Roles.AnyAsync())
            {
                return;
            }

            await roleManager.CreateAsync(new Role { Name = CommonConstants.AdminRole });
            await roleManager.CreateAsync(new Role { Name = CommonConstants.ModeratorRole });

            await context.SaveChangesAsync();
        }

        private async Task SeedUsersAsync(UserManager<User, int> userManager, int usersCount, TitaniumForumDbContext context)
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

        private async Task SeedUsersAsync(UserManager<User, int> userManager, RoleManager<Role, int> roleManager, int usersCount, string role, TitaniumForumDbContext context)
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

        private async Task SeedCategoriesAsync(int categoriesCount, TitaniumForumDbContext context)
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

        private async Task SeedSubCategoriesAsync(int subCategoriesCount, TitaniumForumDbContext context)
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

        private async Task SeedTagsAsync(int tagsCount, TitaniumForumDbContext context)
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

        private async Task SeedQuestionsAsync(int questionsCount, TitaniumForumDbContext context)
        {
            if (await context.Questions.AnyAsync())
            {
                return;
            }

            List<int> subCategoryIds = await context.SubCategories.Select(c => c.Id).ToListAsync();
            List<User> users = await context.Users.ToListAsync();
            List<int> tagIds = await context.Tags.Select(u => u.Id).ToListAsync();

            for (int i = 1; i <= questionsCount; i++)
            {
                Question question = new Question
                {
                    Title = $"Question Title {i}",
                    Content = CommonConstants.lorem,
                    DateAdded = DateTime.UtcNow.AddDays(-i).AddHours(-i).AddMinutes(-i),
                    ViewCount = random.Next(MinViewsPerQuestion, MaxViewsPerQuestion),
                    IsReported = this.GetRandomBool(),
                    IsLocked = this.GetRandomBool(),
                    IsDeleted = this.GetRandomBool(),
                    AuthorId = users[random.Next(0, users.Count)].Id,
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
                    User user = users[random.Next(0, users.Count)];

                    if (question.Votes.Any(v => v.UserId == user.Id))
                    {
                        j--;
                        continue;
                    }

                    Direction direction = (Direction)random.Next(0, typeof(Direction).GetEnumValues().Length);

                    question.Votes.Add(new UserQuestionVote
                    {
                        UserId = user.Id,
                        Direction = direction
                    });

                    if (direction == Direction.Like)
                    {
                        user.Rating++;
                    }
                    else if (direction == Direction.Dislike)
                    {
                        user.Rating--;
                    }
                }

                context.Questions.Add(question);
            }

            await context.SaveChangesAsync();
        }

        private async Task SeedAnswersAsync(int answersCount, TitaniumForumDbContext context)
        {
            if (await context.Answers.AnyAsync())
            {
                return;
            }

            List<User> users = await context.Users.ToListAsync();
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
                    AuthorId = users[random.Next(0, users.Count)].Id,
                    DateAdded = question.DateAdded.AddHours(i).AddMinutes(i),
                    IsReported = this.GetRandomBool(),
                    IsDeleted = this.GetRandomBool(),
                    QuestionId = question.Id
                };

                int votesPerAnswer = random.Next(MinVotesPerAnswer, MaxVotesPerAnswer);

                for (int j = 0; j < votesPerAnswer; j++)
                {
                    User user = users[random.Next(0, users.Count)];

                    if (answer.Votes.Any(v => v.UserId == user.Id))
                    {
                        j--;
                        continue;
                    }

                    Direction direction = (Direction)random.Next(0, typeof(Direction).GetEnumValues().Length);

                    answer.Votes.Add(new UserAnswerVote
                    {
                        UserId = user.Id,
                        Direction = direction
                    });

                    if (direction == Direction.Like)
                    {
                        user.Rating++;
                    }
                    else if (direction == Direction.Dislike)
                    {
                        user.Rating--;
                    }
                }

                context.Answers.Add(answer);
            }

            await context.SaveChangesAsync();
        }

        private async Task SeedCommentsAsync(int commentsCount, TitaniumForumDbContext context)
        {
            if (await context.Comments.AnyAsync())
            {
                return;
            }

            List<User> users = await context.Users.ToListAsync();
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
                    AuthorId = users[random.Next(0, users.Count)].Id,
                    DateAdded = answer.DateAdded.AddHours(i).AddMinutes(i),
                    IsReported = this.GetRandomBool(),
                    IsDeleted = this.GetRandomBool(),
                    AnswerId = answer.Id
                };

                int votesPerComment = random.Next(MinVotesPerComment, MaxVotesPerComment);

                for (int j = 0; j < votesPerComment; j++)
                {
                    User user = users[random.Next(0, users.Count)];

                    if (comment.Votes.Any(v => v.UserId == user.Id))
                    {
                        j--;
                        continue;
                    }

                    Direction direction = (Direction)random.Next(0, typeof(Direction).GetEnumValues().Length);

                    comment.Votes.Add(new UserCommentVote
                    {
                        UserId = user.Id,
                        Direction = direction
                    });

                    if (direction == Direction.Like)
                    {
                        user.Rating++;
                    }
                    else if (direction == Direction.Dislike)
                    {
                        user.Rating--;
                    }
                }

                context.Comments.Add(comment);
            }

            await context.SaveChangesAsync();
        }

        private bool GetRandomBool()
        {
            return random.Next(0, 2) == 0 ? false : true;
        }
    }
}