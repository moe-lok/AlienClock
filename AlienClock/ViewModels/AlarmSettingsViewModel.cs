using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlienClock.Models;
using AlienClock.Services.Interfaces;
using AlienClock.ViewModels.Base;
using AlienClock.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AlienClock.ViewModels
{
    public class DaySelection : ViewModelBase
    {
        private bool _isSelected;
        public DayOfAlienWeek Day { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public class AlarmSettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly Action _closeWindow;

        private bool _isEnabled;
        private int _hour;
        private int _minute;
        private int _second;
        private ObservableCollection<DaySelection> _daySelections;

        public AlarmSettingsViewModel(ISettingsService settingsService, Action closeWindow)
        {
            _settingsService = settingsService;
            _closeWindow = closeWindow;

            // Initialize commands
            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());

            // Initialize day selections
            InitializeDaySelections();

            // Load existing settings
            LoadSettings();
        }

        private void InitializeDaySelections()
        {
            DaySelections = new ObservableCollection<DaySelection>(
                Enum.GetValues(typeof(DayOfAlienWeek))
                    .Cast<DayOfAlienWeek>()
                    .Select(day => new DaySelection { Day = day, IsSelected = false })
            );
        }

        private void LoadSettings()
        {
            var settings = _settingsService.LoadAlarmSettings();
            IsEnabled = settings.IsEnabled;
            Hour = settings.Hour;
            Minute = settings.Minute;
            Second = settings.Second;

            // Update day selections based on saved settings
            foreach (var daySelection in DaySelections)
            {
                daySelection.IsSelected = settings.ActiveDays.Contains(daySelection.Day);
            }
        }

        #region Properties
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public int Hour
        {
            get => _hour;
            set
            {
                if (value >= 0 && value < 36)
                    SetProperty(ref _hour, value);
            }
        }

        public int Minute
        {
            get => _minute;
            set
            {
                if (value >= 0 && value < 90)
                    SetProperty(ref _minute, value);
            }
        }

        public int Second
        {
            get => _second;
            set
            {
                if (value >= 0 && value < 90)
                    SetProperty(ref _second, value);
            }
        }

        public ObservableCollection<DaySelection> DaySelections
        {
            get => _daySelections;
            set => SetProperty(ref _daySelections, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region Command Methods
        private void ExecuteSave()
        {
            var settings = new AlarmSettings
            {
                IsEnabled = IsEnabled,
                Hour = Hour,
                Minute = Minute,
                Second = Second,
                ActiveDays = DaySelections.Where(x => x.IsSelected)
                                        .Select(x => x.Day)
                                        .ToList()
            };

            _settingsService.SaveAlarmSettings(settings);
            _closeWindow?.Invoke();
        }

        private void ExecuteCancel()
        {
            _closeWindow?.Invoke();
        }
        #endregion
    }
}
