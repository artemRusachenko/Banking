using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Infrastructure.Data;
using Banking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("BankingDatabase")));

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetService<ApplicationDbContext>()!);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }
    }
}
