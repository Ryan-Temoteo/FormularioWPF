using Microsoft.EntityFrameworkCore;

namespace FormularioWPF.Models
{
    public class ConnDbContext : DbContext
    {

        public DbSet<Endereco> clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=7274");
        }
    }
}
