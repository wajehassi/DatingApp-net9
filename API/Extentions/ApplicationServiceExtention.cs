namespace API.Extentions;
using API.Services;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using API.Data;

public static class ApplicationServiceExtention
{
    public static IServiceCollection AddApplicationSyrvices(this IServiceCollection services,
     IConfiguration config)
    {

        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
         {
             opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
         });

        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        
        return services;
    }

}
