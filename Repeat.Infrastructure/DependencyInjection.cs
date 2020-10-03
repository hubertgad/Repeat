using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repeat.Domain.Interfaces;
using Repeat.Infrastructure.Data;
using Repeat.Infrastructure.Services;

namespace Repeat.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("RepeatContext")));

            services.AddScoped<IApplicationDbContext>
                (provider => provider.GetService<ApplicationDbContext>());

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<ISetService, SetService>();

            services.AddScoped<IQuestionService, QuestionService>();

            services.AddScoped<ITestService, TestService>();

            return services;
        }
    }
}