using DisplayRotation.Avalonia.ViewModels;
using EvilBaschdi.About.Avalonia;
using EvilBaschdi.About.Avalonia.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DisplayRotation.Avalonia.DependencyInjection;

/// <summary />
public static class ConfigureWindowsAndViewModels
{
    /// <summary />
    public static void AddWindowsAndViewModels(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IAboutViewModelExtended, AboutViewModelExtended>();
        services.AddTransient<AboutWindow>();

        services.AddSingleton<ITopLevel, MainWindowTopLevel>();
        services.AddSingleton<MainWindowViewModel>();
    }
}