using FlightInformationApi.Database;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace FlightInformationApi;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.local.json", true);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}


public class Startup
{
    //private const string AllowOrigins = "_myAllowSpecificOrigins";

    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddCors(options =>
        //      {
        //          options.AddPolicy(name: AllowOrigins,
        //              policy =>
        //              {
        //                  policy.WithOrigins("http://localhost:8888", "http://localhost:8889", "https://flightinfo.andynet.se/");
        //              });
        //      });

        //services.AddCors(options =>
        //{
        //    options.AddDefaultPolicy(policy =>
        //    {
        //        policy.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(s => true).AllowCredentials()
        //    });
        //});

        services.AddCors();

        services.AddControllers();

        services.AddOptions<AppSettings>().Bind(Configuration.GetSection(AppSettings.ConfigAreaName));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        var settings = Configuration.GetSection(AppSettings.ConfigAreaName).Get<AppSettings>();

        services.AddDbContext<FlightDataContext>(dbContextOptions =>
        {
            dbContextOptions
                .UseMySql(settings.MysqlConnectionString, ServerVersion.Create(8, 3, 0, ServerType.MySql))
                .LogTo(Console.WriteLine, LogLevel.Warning)
                //.EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        //app.UseCors(AllowOrigins);

        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials());

        app.UseAuthorization();

        app.UseEndpoints(builder =>
        {
            builder.MapControllers();
        });
    }
}