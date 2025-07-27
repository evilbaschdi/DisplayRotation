using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using EvilBaschdi.About.Avalonia;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace DisplayRotation.Avalonia.ViewModels;

/// <summary>
///     The main window view model.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private readonly ITopLevel _topLevel;

    /// <summary>
    ///     Gets or Sets the about window command.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AboutWindowCommand { get; set; }

    private async Task AboutWindowCommandAction()
    {
        var aboutWindow = App.ServiceProvider.GetRequiredService<AboutWindow>();
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
        if (mainWindow != null)
        {
            await aboutWindow.ShowDialog(mainWindow);
        }
    }

    private static string FullPathOrName(IStorageItem item) => item is null ? "(null)" : item.Path.LocalPath;

    /// <summary>
    ///     Constructor
    /// </summary>
    public MainWindowViewModel([NotNull] ITopLevel topLevel
    )
    {
        _topLevel = topLevel ?? throw new ArgumentNullException(nameof(topLevel));
        AboutWindowCommand = ReactiveCommand.CreateFromTask(AboutWindowCommandAction);
    }
}