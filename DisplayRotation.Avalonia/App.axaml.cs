using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DisplayRotation.Avalonia.DependencyInjection;
using DisplayRotation.Avalonia.ViewModels;
using EvilBaschdi.About.Avalonia.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DisplayRotation.Avalonia;

/// <inheritdoc />
public class App : Application
{
    /// <inheritdoc />
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    ///     ServiceProvider for DependencyInjection
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceProvider ServiceProvider { get; set; }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddAboutServices();
        serviceCollection.AddAvaloniaServices();
        serviceCollection.AddWindowsAndViewModels();

        ServiceProvider = serviceCollection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
                             {
                                 DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>()
                             };

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}