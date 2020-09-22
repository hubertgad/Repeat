using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repeat.DataAccess.Data;
using Repeat.DataAccess.Services;
using Repeat.Domain.Interfaces;

namespace Repeat.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
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