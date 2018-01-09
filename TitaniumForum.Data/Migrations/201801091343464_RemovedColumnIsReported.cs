namespace TitaniumForum.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RemovedColumnIsReported : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Answers", "IsReported");
            DropColumn("dbo.Comments", "IsReported");
        }

        public override void Down()
        {
            AddColumn("dbo.Comments", "IsReported", c => c.Boolean(nullable: false));
            AddColumn("dbo.Answers", "IsReported", c => c.Boolean(nullable: false));
        }
    }
}