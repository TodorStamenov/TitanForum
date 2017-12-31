namespace TitaniumForum.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class LogsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    User = c.String(),
                    TableName = c.String(),
                    LogType = c.Int(nullable: false),
                    TimeStamp = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.Logs");
        }
    }
}