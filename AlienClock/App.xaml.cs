using System.Configuration;
using System.Data;
using System.Windows;
using AlienClock.Services;
using AlienClock.Services.Interfaces;
using AlienClock.Views.Windows;

namespace AlienClock
{
    public partial class App : Application
    {
        private ISettingsService _settingsService;
        private ISoundService _soundService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize services
            _settingsService = new SettingsService();
            _soundService = new SoundService();

            // Create and show main window
            var mainWindow = new MainWindow(_settingsService, _soundService);
            mainWindow.Show();
        }
    }
}
