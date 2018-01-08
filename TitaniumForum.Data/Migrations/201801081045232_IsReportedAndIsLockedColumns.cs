namespace TitaniumForum.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class IsReportedAndIsLockedColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answers", "IsReported", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "IsReported", c => c.Boolean(nullable: false));
            AddColumn("dbo.Questions", "IsReported", c => c.Boolean(nullable: false));
            AddColumn("dbo.Questions", "IsLocked", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Questions", "IsLocked");
            DropColumn("dbo.Questions", "IsReported");
            DropColumn("dbo.Comments", "IsReported");
            DropColumn("dbo.Answers", "IsReported");
        }
    }
}