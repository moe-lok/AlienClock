using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AlienClock.ViewModels;
using AlienClock.Services.Interfaces;

namespace AlienClock.Views.Windows
{
    public partial class AlarmNotificationWindow : Window
    {
        public AlarmNotificationWindow(ISoundService soundService, ISettingsService settingsService)
        {
            InitializeComponent();
            DataContext = new AlarmNotificationViewModel(soundService, settingsService, Close);
        }
    }
}
