using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlienClock.Models;
using AlienClock.Services.Interfaces;
using System.IO;
using System.Text.Json;

namespace AlienClock.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsPath;

        public SettingsService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appData, "AlienClock");
            _settingsPath = Path.Combine(appFolder, "settings.json");

            // Ensure directory exists
            Directory.CreateDirectory(appFolder);
        }

        public AlarmSettings LoadAlarmSettings()
        {
            if (!File.Exists(_settingsPath))
            {
                return new AlarmSettings
                {
                    IsEnabled = false,
                    Hour = 0,
                    Minute = 0,
                    Second = 0,
                    AlarmSound = "default.wav"
                };
            }

            string json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AlarmSettings>(json);
        }

        public void SaveAlarmSettings(AlarmSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_settingsPath, json);
        }
    }
}
