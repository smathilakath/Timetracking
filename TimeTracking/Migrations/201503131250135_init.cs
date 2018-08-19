namespace TimeTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OAuthTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        realmid = c.Int(nullable: false),
                        access_token = c.String(),
                        access_secret = c.String(),
                        datasource = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OAuthTokens");
        }
    }
}
