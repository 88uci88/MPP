using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MPP.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<PicketValue> PicketValues { get; set; }
        public DbSet<Picket> Pickets { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<AreaPoint> AreaPoints { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<ProfilePoint> ProfilePoints { get; set; }

        public static string ConnectionString { get; set; }

        private static ApplicationContext instance;
        public static ApplicationContext getInstance()
        {
            if (instance == null)
            {
                instance = new ApplicationContext(ConnectionString.ToString());
                
                instance.Customers.Load();
                instance.Projects.Load();
                instance.AreaPoints.Load();
                instance.Areas.Load();
                instance.Operators.Load();
                instance.ProfilePoints.Load();
                instance.Profiles.Load();
                instance.Pickets.Load();
                instance.PicketValues.Load();                
                instance.SaveChanges();
            }
            return instance;
        }

        public ApplicationContext() { }

        public ApplicationContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($@"Server=DBSRV\GLO2023;Database={ConnectionString};Trusted_Connection=True;TrustServerCertificate=True");
        }

        
    }
}
