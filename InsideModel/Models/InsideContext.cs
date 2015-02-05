using System.Data.Entity;
using InsideModel.Models.Identity;
using InsideModel.Models.Mapping;

namespace InsideModel.Models
{
    public partial class InsideContext : DbContext
    {
        static InsideContext()
        {
            Database.SetInitializer<InsideContext>(null);
        }

        public InsideContext()
            : base("Name=nassaContext")

        {
            var t = 2;
            var t2 = t - 2;
        }

        public DbSet<Client> Clients { get; set; }
        /*public DbSet<Admin> Admin { get; set; }*/
        public DbSet<InsideUser> InsideUser { get; set; }
        public DbSet<SerpRanking> SerpRanking { get; set; }
        public DbSet<Task> Task { get; set; }
        public DbSet<Contact> Lead { get; set; }
        public DbSet<ContactInteraction> LeadInteraction { get; set; }
        public DbSet<ContactProperty> LeadProperty { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<Label> Label { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<Token> Token { get; set; }
        public DbSet<UserClaims> UserClaims { get; set; }
        public DbSet<ExternalToken> ExternalToken { get; set; }
        public DbSet<UserLogins> UserLogins { get; set; }
        public DbSet<InsideRole> Role { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new ClientMap());
            /*modelBuilder.Configurations.Add(new AdminMap());*/
            modelBuilder.Configurations.Add(new InsideUserMap());
            modelBuilder.Configurations.Add(new SerpRankingMap());
            modelBuilder.Configurations.Add(new TaskMap());
            modelBuilder.Configurations.Add(new ContactMap());
            modelBuilder.Configurations.Add(new ContactInteractionMap());
            modelBuilder.Configurations.Add(new ContactPropertyMap());
            modelBuilder.Configurations.Add(new BudgetMap());
            modelBuilder.Configurations.Add(new LabelMap());
            modelBuilder.Configurations.Add(new TagMap());
            modelBuilder.Configurations.Add(new TokenMap());
            modelBuilder.Configurations.Add(new UserLoginsMap());
            modelBuilder.Configurations.Add(new InsideRoleMap());
            modelBuilder.Configurations.Add(new UserClaimsMap());
            modelBuilder.Configurations.Add(new ExternalTokenMap());
            
        
        }
    }
}
