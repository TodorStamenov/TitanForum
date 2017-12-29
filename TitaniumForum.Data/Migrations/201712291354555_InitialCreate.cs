namespace TitaniumForum.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Content = c.String(nullable: false),
                    Rating = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false),
                    QuestionId = c.Int(nullable: false),
                    AuthorId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId)
                .Index(t => t.AuthorId);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    PostsCount = c.Int(nullable: false),
                    Rating = c.Int(nullable: false),
                    ProfileImage = c.Binary(),
                    Email = c.String(),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.UserAnswerVotes",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    AnswerId = c.Int(nullable: false),
                    Direction = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserId, t.AnswerId })
                .ForeignKey("dbo.Answers", t => t.AnswerId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AnswerId);

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.Int(nullable: false),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Comments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Content = c.String(nullable: false),
                    Rating = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false),
                    AnswerId = c.Int(nullable: false),
                    AuthorId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .ForeignKey("dbo.Answers", t => t.AnswerId, cascadeDelete: true)
                .Index(t => t.AnswerId)
                .Index(t => t.AuthorId);

            CreateTable(
                "dbo.UserCommentVotes",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    CommentId = c.Int(nullable: false),
                    Direction = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserId, t.CommentId })
                .ForeignKey("dbo.Comments", t => t.CommentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.CommentId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Questions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false, maxLength: 150),
                    Content = c.String(nullable: false),
                    Rating = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false),
                    ViewCount = c.Int(nullable: false),
                    SubCategoryId = c.Int(nullable: false),
                    AuthorId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .Index(t => t.Title, unique: true)
                .Index(t => t.SubCategoryId)
                .Index(t => t.AuthorId);

            CreateTable(
                "dbo.SubCategories",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 20),
                    CategoryId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.Name, unique: true)
                .Index(t => t.CategoryId);

            CreateTable(
                "dbo.Categories",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 20),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.TagQuestions",
                c => new
                {
                    TagId = c.Int(nullable: false),
                    QuestionId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.TagId, t.QuestionId })
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.TagId)
                .Index(t => t.QuestionId);

            CreateTable(
                "dbo.Tags",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 10),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.UserQuestionVotes",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    QuestionId = c.Int(nullable: false),
                    Direction = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserId, t.QuestionId })
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.QuestionId);

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    RoleId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Comments", "AnswerId", "dbo.Answers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Questions", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.UserQuestionVotes", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserQuestionVotes", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.TagQuestions", "TagId", "dbo.Tags");
            DropForeignKey("dbo.TagQuestions", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Questions", "SubCategoryId", "dbo.SubCategories");
            DropForeignKey("dbo.SubCategories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.Users");
            DropForeignKey("dbo.Comments", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.UserCommentVotes", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserCommentVotes", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAnswerVotes", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAnswerVotes", "AnswerId", "dbo.Answers");
            DropForeignKey("dbo.Answers", "AuthorId", "dbo.Users");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.UserQuestionVotes", new[] { "QuestionId" });
            DropIndex("dbo.UserQuestionVotes", new[] { "UserId" });
            DropIndex("dbo.Tags", new[] { "Name" });
            DropIndex("dbo.TagQuestions", new[] { "QuestionId" });
            DropIndex("dbo.TagQuestions", new[] { "TagId" });
            DropIndex("dbo.Categories", new[] { "Name" });
            DropIndex("dbo.SubCategories", new[] { "CategoryId" });
            DropIndex("dbo.SubCategories", new[] { "Name" });
            DropIndex("dbo.Questions", new[] { "AuthorId" });
            DropIndex("dbo.Questions", new[] { "SubCategoryId" });
            DropIndex("dbo.Questions", new[] { "Title" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.UserCommentVotes", new[] { "CommentId" });
            DropIndex("dbo.UserCommentVotes", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "AuthorId" });
            DropIndex("dbo.Comments", new[] { "AnswerId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.UserAnswerVotes", new[] { "AnswerId" });
            DropIndex("dbo.UserAnswerVotes", new[] { "UserId" });
            DropIndex("dbo.Answers", new[] { "AuthorId" });
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.UserQuestionVotes");
            DropTable("dbo.Tags");
            DropTable("dbo.TagQuestions");
            DropTable("dbo.Categories");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Questions");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.UserCommentVotes");
            DropTable("dbo.Comments");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.UserAnswerVotes");
            DropTable("dbo.Users");
            DropTable("dbo.Answers");
        }
    }
}