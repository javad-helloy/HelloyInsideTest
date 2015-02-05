namespace InsideModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedToDatetime2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        AnalyticsTableId = c.String(),
                        ClientId = c.Int(nullable: false),
                        BrandName = c.String(),
                        ConsultantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Consultant", t => t.ConsultantId, cascadeDelete: true)
                .Index(t => t.ConsultantId);
            
            CreateTable(
                "dbo.Consultant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FacebookAdCampaign",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CampaignId = c.Long(nullable: false),
                        CampaignName = c.String(nullable: false),
                        Client_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.FacebookAdPerformance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Impressions = c.Int(nullable: false),
                        Clicks = c.Int(nullable: false),
                        CPC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Spent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FacebookAdCampaign_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FacebookAdCampaign", t => t.FacebookAdCampaign_Id, cascadeDelete: true)
                .Index(t => t.FacebookAdCampaign_Id);
            
            CreateTable(
                "dbo.FacebookPage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FacebookId = c.Long(nullable: false),
                        Name = c.String(nullable: false),
                        Client_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.InsideUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        MembershipProviderId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.FacebookAuthentication",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TokenString = c.String(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PhoneStatistic",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        From = c.String(),
                        To = c.String(),
                        LengthInTicks = c.Long(),
                        Date = c.DateTime(nullable: false),
                        Answered = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SerpRanking",
                c => new
                    {
                        Date = c.DateTime(nullable: false, storeType: "datetime2"),
                        Url = c.String(nullable: false, maxLength: 128),
                        Keyword = c.String(nullable: false, maxLength: 128),
                        PageRank = c.Int(nullable: false),
                        Region = c.String(),
                        Language = c.String(),
                        Start = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        ChangeDay = c.Int(),
                        ChangeWeek = c.Int(),
                        ChangeMonth = c.Int(),
                        RankingUrl = c.String(),
                        BackLinks = c.Int(),
                        SearchVolume = c.Int(),
                    })
                .PrimaryKey(t => new { t.Date, t.Url, t.Keyword });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InsideUser", "ClientId", "dbo.Client");
            DropForeignKey("dbo.FacebookPage", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.FacebookAdPerformance", "FacebookAdCampaign_Id", "dbo.FacebookAdCampaign");
            DropForeignKey("dbo.FacebookAdCampaign", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.Client", "ConsultantId", "dbo.Consultant");
            DropIndex("dbo.InsideUser", new[] { "ClientId" });
            DropIndex("dbo.FacebookPage", new[] { "Client_Id" });
            DropIndex("dbo.FacebookAdPerformance", new[] { "FacebookAdCampaign_Id" });
            DropIndex("dbo.FacebookAdCampaign", new[] { "Client_Id" });
            DropIndex("dbo.Client", new[] { "ConsultantId" });
            DropTable("dbo.SerpRanking");
            DropTable("dbo.PhoneStatistic");
            DropTable("dbo.FacebookAuthentication");
            DropTable("dbo.InsideUser");
            DropTable("dbo.FacebookPage");
            DropTable("dbo.FacebookAdPerformance");
            DropTable("dbo.FacebookAdCampaign");
            DropTable("dbo.Consultant");
            DropTable("dbo.Client");
        }
    }
}
