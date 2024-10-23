using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienClock.Models
{
    public class AlarmSettings
    {
        public bool IsEnabled { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public List<DayOfAlienWeek> ActiveDays { get; set; }
        public string AlarmSound { get; set; }

        public AlarmSettings()
        {
            // Initialize with default values
            IsEnabled = false;
            Hour = 0;
            Minute = 0;
            Second = 0;
            ActiveDays = new List<DayOfAlienWeek>();
            AlarmSound = "alarm.wav";
        }

        // Optional: Add a copy constructor if needed
        public AlarmSettings(AlarmSettings other)
        {
            IsEnabled = other.IsEnabled;
            Hour = other.Hour;
            Minute = other.Minute;
            Second = other.Second;
            ActiveDays = new List<DayOfAlienWeek>(other.ActiveDays);
            AlarmSound = other.AlarmSound;
        }
    }
}
