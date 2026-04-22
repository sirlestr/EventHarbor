using System.IO;
using System.Windows;
using EventHarbor.Common;
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

        DispatcherUnhandledException += (_, args) =>
        {
            DumpCrash("Dispatcher", args.Exception);
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            DumpCrash("AppDomain", args.ExceptionObject as Exception);
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            DumpCrash("Task", args.Exception);
            args.SetObserved();
        };

        Directory.CreateDirectory(AppPaths.LocalAppFolder);
        Directory.CreateDirectory(AppPaths.LogFolder);

        TextBoxSelectAllBehavior.Register();

        try
        {

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddDbContextFactory<EventHarborDbContext>(options =>
                    options.UseSqlite($"Data Source={AppPaths.DatabasePath}"));

                services.AddSingleton<SessionState>();
                services.AddSingleton<SettingsStore>();
                services.AddSingleton<ThemeManager>();
                services.AddSingleton<RememberMeService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<ICultureActionService, CultureActionService>();

                services.AddTransient<LoginViewModel>();
                services.AddTransient<RegisterViewModel>();
                services.AddTransient<ForgotViewModel>();
                services.AddSingleton<AuthShellViewModel>(sp =>
                {
                    AuthShellViewModel? shell = null;
                    Func<LoginViewModel> loginFactory = () =>
                    {
                        var vm = sp.GetRequiredService<LoginViewModel>();
                        vm.LoginSucceeded += (_, _) => shell!.RaiseAuthSucceeded();
                        vm.NavigateToRegister += (_, _) => shell!.GoTo(AuthScreen.Register);
                        vm.NavigateToForgot += (_, _) => shell!.GoTo(AuthScreen.Forgot);
                        return vm;
                    };
                    Func<RegisterViewModel> registerFactory = () =>
                    {
                        var vm = sp.GetRequiredService<RegisterViewModel>();
                        vm.NavigateToLogin += (_, _) => shell!.GoTo(AuthScreen.Login);
                        vm.RegisterSucceeded += (_, _) => shell!.GoTo(AuthScreen.Login);
                        return vm;
                    };
                    Func<ForgotViewModel> forgotFactory = () =>
                    {
                        var vm = sp.GetRequiredService<ForgotViewModel>();
                        vm.NavigateToLogin += (_, _) => shell!.GoTo(AuthScreen.Login);
                        vm.ResetCompleted += (_, _) => shell!.GoTo(AuthScreen.Login);
                        return vm;
                    };
                    shell = new AuthShellViewModel(loginFactory, registerFactory, forgotFactory);
                    return shell;
                });

                services.AddTransient<ListViewModel>();
                services.AddTransient<FormViewModel>();
                services.AddTransient<StatsViewModel>();
                services.AddSingleton<MainShellViewModel>(sp =>
                {
                    MainShellViewModel? shell = null;
                    Func<ListViewModel> listFactory = () =>
                    {
                        var vm = sp.GetRequiredService<ListViewModel>();
                        vm.NewRequested += (_, _) => shell!.StartNewEvent();
                        vm.EditRequested += (_, a) => shell!.StartEditEvent(a);
                        return vm;
                    };
                    Func<FormViewModel> formFactory = () =>
                    {
                        var vm = sp.GetRequiredService<FormViewModel>();
                        vm.Saved += (_, _) => shell!.ReturnToList();
                        vm.Cancelled += (_, _) => shell!.ReturnToList();
                        return vm;
                    };
                    shell = new MainShellViewModel(
                        listFactory,
                        formFactory,
                        sp.GetRequiredService<StatsViewModel>,
                        sp.GetRequiredService<SessionState>(),
                        sp.GetRequiredService<ThemeManager>());
                    return shell;
                });

                services.AddTransient<LoginWindow>();
                services.AddTransient<MainWindow>();
            })
            .UseSerilog((_, cfg) => cfg
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting", Serilog.Events.LogEventLevel.Warning)
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

        AppHost.Services.GetRequiredService<ThemeManager>().ApplyCurrent();

        // Auto-login if a remembered session exists for this Windows user.
        var rememberMe = AppHost.Services.GetRequiredService<RememberMeService>();
        var rememberedId = rememberMe.TryLoad();
        if (rememberedId is int id)
        {
            using var scope = AppHost.Services.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<EventHarborDbContext>>();
            await using var db = await factory.CreateDbContextAsync();
            var user = await db.Users.FindAsync(id);
            if (user is not null)
            {
                AppHost.Services.GetRequiredService<SessionState>().SetLoggedUser(user);
                var main = AppHost.Services.GetRequiredService<MainWindow>();
                main.Show();
                return;
            }
            // remembered id no longer exists in DB -> clear and fall through
            rememberMe.Clear();
        }

        var login = AppHost.Services.GetRequiredService<LoginWindow>();
        login.Show();
        }
        catch (Exception ex)
        {
            DumpCrash("OnStartup", ex);
            Shutdown(1);
        }
    }

    private static void DumpCrash(string source, Exception? ex)
    {
        try
        {
            var path = Path.Combine(AppPaths.LogFolder, "crash.log");
            Directory.CreateDirectory(AppPaths.LogFolder);
            File.AppendAllText(path,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {source}:{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}");
        }
        catch { /* last-chance swallow */ }

        try
        {
            MessageBox.Show(
                $"{source} exception:{Environment.NewLine}{Environment.NewLine}{ex?.Message}{Environment.NewLine}{Environment.NewLine}Detail: %LOCALAPPDATA%\\EventHarbor\\logs\\crash.log",
                "EventHarbor — Chyba",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch { }
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
