﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlienClock.Models;
using AlienClock.Services.Interfaces;
using AlienClock.ViewModels.Base;
using AlienClock.ViewModels.Commands;
using System;
using System.Windows.Threading;
using System.Windows.Input;
using AlienClock.Views.Windows;
using System.Windows;

namespace AlienClock.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly DispatcherTimer _timer;
        private AlienDateTime _currentTime;
        private DateTime _earthTime;
        private AlarmSettings _alarmSettings;
        private bool _isEditingTime;
        private string _statusMessage;
        private readonly ISoundService _soundService;
        private bool _isAlarmTriggered;

        public MainViewModel(ISettingsService settingsService, ISoundService soundService)
        {
            _settingsService = settingsService;
            _soundService = soundService;
            _isAlarmTriggered = false;

            _alarmSettings = settingsService.LoadAlarmSettings();
            System.Diagnostics.Debug.WriteLine($"Loaded Alarm Settings:");
            System.Diagnostics.Debug.WriteLine($"Enabled: {_alarmSettings.IsEnabled}");
            System.Diagnostics.Debug.WriteLine($"Time: {_alarmSettings.Hour}:{_alarmSettings.Minute}:{_alarmSettings.Second}");
            System.Diagnostics.Debug.WriteLine($"Active Days: {string.Join(", ", _alarmSettings.ActiveDays)}");


            // Initialize timer for updates (0.5 earth seconds = 1 alien second)
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            UpdateTime();
        }

        #region Properties
        public AlienDateTime CurrentTime
        {
            get => _currentTime;
            private set => SetProperty(ref _currentTime, value);
        }

        public DateTime EarthTime
        {
            get => _earthTime;
            private set => SetProperty(ref _earthTime, value);
        }

        public AlarmSettings AlarmSettings
        {
            get => _alarmSettings;
            set
            {
                if (SetProperty(ref _alarmSettings, value))
                {
                    _settingsService.SaveAlarmSettings(value);
                }
            }
        }

        public bool IsEditingTime
        {
            get => _isEditingTime;
            set
            {
                if (SetProperty(ref _isEditingTime, value))
                {
                    // Optionally refresh commands that depend on this state
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // Editing properties
        private int _editYear;
        public int EditYear
        {
            get => _editYear;
            set => SetProperty(ref _editYear, value);
        }

        private int _editMonth;
        public int EditMonth
        {
            get => _editMonth;
            set => SetProperty(ref _editMonth, value);
        }

        private int _editDay;
        public int EditDay
        {
            get => _editDay;
            set => SetProperty(ref _editDay, value);
        }

        private int _editHour;
        public int EditHour
        {
            get => _editHour;
            set => SetProperty(ref _editHour, value);
        }

        private int _editMinute;
        public int EditMinute
        {
            get => _editMinute;
            set => SetProperty(ref _editMinute, value);
        }

        private int _editSecond;
        public int EditSecond
        {
            get => _editSecond;
            set => SetProperty(ref _editSecond, value);
        }
        #endregion

        #region Commands
        private ICommand _setTimeCommand;
        private ICommand _setAlarmCommand;
        private ICommand _saveTimeCommand;
        private ICommand _cancelEditCommand;

        public ICommand SetTimeCommand
        {
            get
            {
                return _setTimeCommand ??= new RelayCommand(_ =>
                {
                    // Populate form with current time values
                    EditYear = CurrentTime.Year;
                    EditMonth = CurrentTime.Month;
                    EditDay = CurrentTime.Day;
                    EditHour = CurrentTime.Hour;
                    EditMinute = CurrentTime.Minute;
                    EditSecond = CurrentTime.Second;

                    IsEditingTime = true;
                    StatusMessage = "Editing time...";
                    System.Diagnostics.Debug.WriteLine($"Current Time: {CurrentTime}");
                    System.Diagnostics.Debug.WriteLine($"Form populated with: Y:{EditYear} M:{EditMonth} D:{EditDay} H:{EditHour} M:{EditMinute} S:{EditSecond}");
                });
            }
        }

        public ICommand SaveTimeCommand
        {
            get
            {
                return _saveTimeCommand ??= new RelayCommand(_ =>
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Saving time with values: Y:{EditYear} M:{EditMonth} D:{EditDay} H:{EditHour} M:{EditMinute} S:{EditSecond}");

                        // Create new AlienDateTime
                        var newTime = new AlienDateTime(
                            EditYear,
                            EditMonth,
                            EditDay,
                            EditHour,
                            EditMinute,
                            EditSecond
                        );

                        // Update the timer
                        _timer.Stop();
                        CurrentTime = newTime;
                        _timer.Start();

                        IsEditingTime = false;
                        StatusMessage = "Time updated successfully";
                        System.Diagnostics.Debug.WriteLine($"New time set: {CurrentTime}");
                    }
                    catch (ArgumentException ex)
                    {
                        StatusMessage = $"Error: {ex.Message}";
                        System.Diagnostics.Debug.WriteLine($"Error setting time: {ex.Message}");
                    }
                });
            }
        }

        public ICommand CancelEditCommand
        {
            get
            {
                return _cancelEditCommand ??= new RelayCommand(_ =>
                {
                    IsEditingTime = false;
                    StatusMessage = "Time edit cancelled";
                });
            }
        }

        public ICommand SetAlarmCommand
        {
            get
            {
                return _setAlarmCommand ??= new RelayCommand(_ =>
                {
                    var settingsWindow = new AlarmSettingsWindow(_settingsService);
                    settingsWindow.Owner = Application.Current.MainWindow;
                    settingsWindow.ShowDialog();

                    // Reload alarm settings
                    AlarmSettings = _settingsService.LoadAlarmSettings();
                    System.Diagnostics.Debug.WriteLine($"New Alarm Settings:");
                    System.Diagnostics.Debug.WriteLine($"Enabled: {AlarmSettings.IsEnabled}");
                    System.Diagnostics.Debug.WriteLine($"Time: {AlarmSettings.Hour}:{AlarmSettings.Minute}:{AlarmSettings.Second}");
                    System.Diagnostics.Debug.WriteLine($"Active Days: {string.Join(", ", AlarmSettings.ActiveDays)}");

                    StatusMessage = "Alarm settings updated";
                });
            }
        }
        #endregion

        #region Private Methods
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!IsEditingTime)
            {
                UpdateTime();
                CheckAlarm();
            }
        }

        private void UpdateTime()
        {
            if (!IsEditingTime)
            {
                if (CurrentTime == null)
                {
                    CurrentTime = AlienDateTime.Now;
                }
                else
                {
                    // Add one alien second to current time
                    CurrentTime = CurrentTime.AddAlienSeconds(1);
                }
                EarthTime = CurrentTime.ToEarthDateTime();
            }
        }

        private void CheckAlarm()
        {
            if (!AlarmSettings.IsEnabled || _isAlarmTriggered)
            {
                return;
            }

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"Checking Alarm at: {CurrentTime}");
            System.Diagnostics.Debug.WriteLine($"Alarm set for: H:{AlarmSettings.Hour} M:{AlarmSettings.Minute} S:{AlarmSettings.Second}");
            System.Diagnostics.Debug.WriteLine($"Current time: H:{CurrentTime.Hour} M:{CurrentTime.Minute} S:{CurrentTime.Second}");
            System.Diagnostics.Debug.WriteLine($"Active Days: {string.Join(", ", AlarmSettings.ActiveDays)}");
            System.Diagnostics.Debug.WriteLine($"Current Day: {CurrentTime.DayOfWeek}");

            if (AlarmSettings.Hour == CurrentTime.Hour &&
                AlarmSettings.Minute == CurrentTime.Minute &&
                AlarmSettings.Second == CurrentTime.Second)
            {
                System.Diagnostics.Debug.WriteLine("Time matches!");

                // Check if alarm should trigger on current day
                var currentDayOfWeek = CurrentTime.DayOfWeek;
                if (AlarmSettings.ActiveDays.Contains(currentDayOfWeek))
                {
                    System.Diagnostics.Debug.WriteLine("Day matches! Triggering alarm...");
                    _isAlarmTriggered = true;
                    TriggerAlarm();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Day does not match, not triggering alarm.");
                }
            }
        }

        private void TriggerAlarm()
        {
            System.Diagnostics.Debug.WriteLine("TriggerAlarm method called");
            Application.Current.Dispatcher.Invoke(() =>
            {
                var notificationWindow = new AlarmNotificationWindow(_soundService, _settingsService);
                notificationWindow.Closed += (s, e) =>
                {
                    _isAlarmTriggered = false;
                    // Reload alarm settings in case they were modified (e.g., by snooze)
                    AlarmSettings = _settingsService.LoadAlarmSettings();
                };
                notificationWindow.Show();
            });
        }

        private void ExecuteSetTime()
        {
            IsEditingTime = true;
            EditYear = CurrentTime.Year;
            EditMonth = CurrentTime.Month;
            EditDay = CurrentTime.Day;
            EditHour = CurrentTime.Hour;
            EditMinute = CurrentTime.Minute;
            EditSecond = CurrentTime.Second;
            StatusMessage = "Editing time...";
        }

        private void ExecuteSetAlarm()
        {
            // This will be implemented when we create the alarm settings window
            StatusMessage = "Setting alarm...";
        }

        private void ExecuteSaveTime()
        {
            try
            {
                CurrentTime = new AlienDateTime(
                    EditYear, EditMonth, EditDay,
                    EditHour, EditMinute, EditSecond);

                IsEditingTime = false;
                StatusMessage = "Time updated successfully";
            }
            catch (ArgumentException ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private void ExecuteCancelEdit()
        {
            IsEditingTime = false;
            StatusMessage = "Time edit cancelled";
        }
        #endregion
    }
}
