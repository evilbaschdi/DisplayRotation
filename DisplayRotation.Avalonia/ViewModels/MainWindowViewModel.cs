using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using EvilBaschdi.About.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace DisplayRotation.Avalonia.ViewModels;

/// <summary>
///     The main window view model.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
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

    /// <summary>
    ///     Constructor
    /// </summary>
    public MainWindowViewModel()
    {
        AboutWindowCommand = ReactiveCommand.CreateFromTask(AboutWindowCommandAction);
    }
}