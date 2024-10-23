using AlienClock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienClock.Services.Interfaces
{
    public interface ISettingsService
    {
        void SaveAlarmSettings(AlarmSettings settings);
        AlarmSettings LoadAlarmSettings();
    }
}
