using Microsoft.EntityFrameworkCore;
using KKLProject.Models;
using System.Collections.Generic;

namespace KKLProject.DBContext
{
    public class KKLDbContext : DbContext
    {
        public KKLDbContext(DbContextOptions<KKLDbContext> options)
        : base(options)
        {
        }

        public DbSet<JsonData> JsonData { get; set; }
        public DbSet<CompressedData> CompressedData { get; set; }
    }
}
