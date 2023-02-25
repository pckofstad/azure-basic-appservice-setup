using BlazorApp.Server.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace BlazorApp.Server.DbContext
{
    public class BlazorAppDbContextFactory : IDesignTimeDbContextFactory<BlazorAppDbContext>
    {
        private readonly DbContextOptions<BlazorAppDbContext> _options;
        private readonly string _connectionString;
        private const string ConnectionStringKey = "SQL_CONNECTION_STRING";

#if DEBUG
        public BlazorAppDbContextFactory()
        {
            _connectionString = "only for migration";
            _options = CreateOptions();
        }
#endif
        public BlazorAppDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
            _options = CreateOptions();
        }

        public BlazorAppDbContextFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection(ConnectionStringKey)?.Value;
            _options = CreateOptions();
        }

        private DbContextOptions<BlazorAppDbContext> CreateOptions()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ApplicationException($"Connection string {ConnectionStringKey} not set.");
            }

            return new DbContextOptionsBuilder<BlazorAppDbContext>()
                .UseSqlServer(_connectionString, optionsBuilder =>
                {
                    optionsBuilder.CommandTimeout(10000);
                    optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    optionsBuilder.MaxBatchSize(1);
                })
                .ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                .Options;
        }



        public BlazorAppDbContext CreateDbContext(params string[] args) =>
            new BlazorAppDbContext(_options);


    }
}
