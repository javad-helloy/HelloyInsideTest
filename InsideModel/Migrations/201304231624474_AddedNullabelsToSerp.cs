namespace InsideModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNullabelsToSerp : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SerpRanking", "PageRank", c => c.Int());
            AlterColumn("dbo.SerpRanking", "Start", c => c.Int());
            AlterColumn("dbo.SerpRanking", "Rank", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SerpRanking", "Rank", c => c.Int(nullable: false));
            AlterColumn("dbo.SerpRanking", "Start", c => c.Int(nullable: false));
            AlterColumn("dbo.SerpRanking", "PageRank", c => c.Int(nullable: false));
        }
    }
}
