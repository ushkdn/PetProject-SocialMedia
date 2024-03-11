namespace SocialNetwork.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MetaData> MetaDatas { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Groups)
                .WithMany(f => f.Followers)
                .UsingEntity(j => j.ToTable("UserGroups")); // Таблица, представляющая отношение многие ко многим

            modelBuilder.Entity<User>()
                .HasMany(u => u.SentGroupJoinRequests)
                .WithMany(j => j.JoinRequests)
                .UsingEntity(j => j.ToTable("UserJoinRequests"));
        }
    }
}