using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using EvilBaschdi.About.Avalonia;
using EvilBaschdi.Core.Avalonia.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace DisplayRotation.Avalonia.ViewModels;

/// <summary>
///     The main window view model.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    ///     Gets or Sets the about window command.
    /// </summary>
    public ReactiveCommand<RxVoid, RxVoid> AboutWindowCommand { get; set; }

    private async Task AboutWindowCommandAction()
    {
        var aboutWindow = ApplicationServices.GetRequiredService<AboutWindow>();
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