using Microsoft.Extensions.DependencyInjection;
using BlazorApp.Server.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Server.DbContext
{
    public static class DbContextSetup
    {
        public static void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = ConfigurationInfo.GetSqlConnectionString();
            services.AddDbContext<BlazorAppDbContext>(options => options.UseSqlServer(connectionString, ob =>
            {
                ob.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                ob.MaxBatchSize(1);
            }));

            var factory = new BlazorAppDbContextFactory(connectionString);
            factory.CreateDbContext().Database.Migrate();

        }
    }
}
