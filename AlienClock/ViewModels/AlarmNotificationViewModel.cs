using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlienClock.Services.Interfaces;
using AlienClock.ViewModels.Base;
using AlienClock.ViewModels.Commands;
using System.Windows.Input;
using AlienClock.Models;

namespace AlienClock.ViewModels
{
    public class AlarmNotificationViewModel : ViewModelBase
    {
        private readonly ISoundService _soundService;
        private readonly ISettingsService _settingsService;
        private readonly Action _closeWindow;

        public AlarmNotificationViewModel(
            ISoundService soundService,
            ISettingsService settingsService,
            Action closeWindow)
        {
            _soundService = soundService;
            _settingsService = settingsService;
            _closeWindow = closeWindow;

            DismissCommand = new RelayCommand(_ => ExecuteDismiss());
            SnoozeCommand = new RelayCommand(_ => ExecuteSnooze());

            // Start playing alarm sound
            _soundService.PlayAlarm();
        }

        public ICommand DismissCommand { get; }
        public ICommand SnoozeCommand { get; }

        private void ExecuteDismiss()
        {
            _soundService.StopAlarm();
            _closeWindow?.Invoke();
        }

        private void ExecuteSnooze()
        {
            _soundService.StopAlarm();

            // Get current settings and update next alarm time
            var settings = _settingsService.LoadAlarmSettings();
            // Add 5 alien minutes
            var currentTime = AlienDateTime.Now;
            var snoozeTime = currentTime.AddAlienSeconds(5 * 90); // 5 minutes * 90 seconds

            settings.Hour = snoozeTime.Hour;
            settings.Minute = snoozeTime.Minute;
            settings.Second = snoozeTime.Second;

            _settingsService.SaveAlarmSettings(settings);
            _closeWindow?.Invoke();
        }
    }
}
