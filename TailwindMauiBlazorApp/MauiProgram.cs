using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Services;
using TailwindMauiBlazorApp.Shared.Helpers;
using TailwindMauiBlazorApp.Shared.Mappings;
using TailwindMauiBlazorApp.Shared.Services;

namespace TailwindMauiBlazorApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add device-specific services used by the TailwindMauiBlazorApp.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddScoped<IIItineraryService, IItineraryService>();
            builder.Services.AddScoped<IIItineraryAccomodationService, IItineraryAccomodationService>();
            builder.Services.AddScoped<IIItineraryPlaceService, IItineraryPlaceService>();
            builder.Services.AddScoped<IIItineraryReservationService, IItineraryReservationService>();
            builder.Services.AddScoped<IIPlaceService, IPlaceService>();

            builder.Services.AddScoped<JsInterop>();

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });


            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly("TailwindMauiBlazorApp.Core"));
            });

            builder.Services.AddBlazorBootstrap();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
