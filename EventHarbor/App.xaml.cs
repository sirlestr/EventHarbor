using System.IO;
using System.Windows;
using EventHarbor.Data;
using EventHarbor.Services;
using EventHarbor.ViewModels;
using EventHarbor.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EventHarbor;

public partial class App : Application
{
    public static IHost AppHost { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Directory.CreateDirectory(AppPaths.LocalAppFolder);
        Directory.CreateDirectory(AppPaths.LogFolder);

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddDbContextFactory<EventHarborDbContext>(options =>
                    options.UseSqlite($"Data Source={AppPaths.DatabasePath}"));

                services.AddSingleton<SessionState>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<ICultureActionService, CultureActionService>();

                services.AddTransient<LoginViewModel>();
                services.AddTransient<RegisterViewModel>();
                services.AddTransient<ForgotViewModel>();
                services.AddSingleton<AuthShellViewModel>(sp => new AuthShellViewModel(
                    sp.GetRequiredService<LoginViewModel>,
                    sp.GetRequiredService<RegisterViewModel>,
                    sp.GetRequiredService<ForgotViewModel>));

                services.AddTransient<ListViewModel>();
                services.AddTransient<FormViewModel>();
                services.AddTransient<StatsViewModel>();
                services.AddSingleton<MainShellViewModel>(sp => new MainShellViewModel(
                    sp.GetRequiredService<ListViewModel>,
                    sp.GetRequiredService<FormViewModel>,
                    sp.GetRequiredService<StatsViewModel>,
                    sp.GetRequiredService<SessionState>()));

                services.AddTransient<LoginWindow>();
                services.AddTransient<MainWindow>();
            })
            .UseSerilog((_, cfg) => cfg
                .MinimumLevel.Information()
                .WriteTo.File(
                    Path.Combine(AppPaths.LogFolder, "eh-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14))
            .Build();

        await AppHost.StartAsync();

        using (var scope = AppHost.Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<EventHarborDbContext>>();
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.MigrateAsync();
        }

        var login = AppHost.Services.GetRequiredService<LoginWindow>();
        login.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (AppHost is not null)
        {
            await AppHost.StopAsync();
            AppHost.Dispose();
        }
        base.OnExit(e);
    }
}
