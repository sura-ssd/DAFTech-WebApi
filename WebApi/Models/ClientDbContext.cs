using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace WebApi.Models
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Client { get; set; }

        public DbSet<Timeframe> Timeframes { get; set; }

        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }

        public DbSet<AdminAnswer> AdminAnswers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            string dbUser = Env.GetString("DB_USER");
            string dbPassword = Env.GetString("DB_PASSWORD");

            optionsBuilder.UseSqlServer($"Data Source=.; Initial Catalog=Client; User Id={dbUser}; password={dbPassword}; TrustServerCertificate=True");
        }
    }
}
