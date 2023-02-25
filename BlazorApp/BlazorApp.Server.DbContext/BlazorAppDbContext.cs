using BlazorApp.Server.DbContext.Models;
using Microsoft.EntityFrameworkCore;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BlazorApp.Server.DbContext
{
    public class BlazorAppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {

        public BlazorAppDbContext() { }

        public BlazorAppDbContext(DbContextOptions<BlazorAppDbContext> options) : base(options) { }

        
        public DbSet<ExampleTextData> ExampleTextData { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
