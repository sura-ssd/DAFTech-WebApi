using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admin { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbUser = Env.GetString("DB_USER");
            string dbPassword = Env.GetString("DB_PASSWORD");

            optionsBuilder.UseSqlServer($"Data Source=.; Initial Catalog=Admin; User Id={dbUser}; password={dbPassword}; TrustServerCertificate=True");
        }
    }
}
