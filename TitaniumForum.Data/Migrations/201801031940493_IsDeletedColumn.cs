namespace TitaniumForum.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class IsDeletedColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Questions", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.SubCategories", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Categories", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Logs", "Username", c => c.String());
            DropColumn("dbo.Logs", "User");
        }

        public override void Down()
        {
            AddColumn("dbo.Logs", "User", c => c.String());
            DropColumn("dbo.Logs", "Username");
            DropColumn("dbo.Categories", "IsDeleted");
            DropColumn("dbo.SubCategories", "IsDeleted");
            DropColumn("dbo.Questions", "IsDeleted");
            DropColumn("dbo.Comments", "IsDeleted");
            DropColumn("dbo.Answers", "IsDeleted");
        }
    }
}