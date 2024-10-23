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
        public List<DayOfAlienWeek> ActiveDays { get; set; } = new List<DayOfAlienWeek>();
        public string AlarmSound { get; set; } = "default.wav";
    }
}
