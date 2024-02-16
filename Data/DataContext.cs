﻿namespace SocialNetwork.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<MetaData> MetaDatas { get; set; }
    }
}
