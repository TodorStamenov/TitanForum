namespace TitaniumForum.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RemovedRatingColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Answers", "Rating");
            DropColumn("dbo.Comments", "Rating");
            DropColumn("dbo.Questions", "Rating");
        }

        public override void Down()
        {
            AddColumn("dbo.Questions", "Rating", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "Rating", c => c.Int(nullable: false));
            AddColumn("dbo.Answers", "Rating", c => c.Int(nullable: false));
        }
    }
}