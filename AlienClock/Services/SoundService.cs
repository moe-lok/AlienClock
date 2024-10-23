using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlienClock.Services.Interfaces;
using System.Media;
using System.IO;

namespace AlienClock.Services
{
    public class SoundService : ISoundService
    {
        private SoundPlayer _soundPlayer;
        private bool _isPlaying;

        public SoundService()
        {
            try
            {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "alarm.wav");
                if (File.Exists(soundPath))
                {
                    _soundPlayer = new SoundPlayer(soundPath);
                    _soundPlayer.LoadAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Alarm sound file not found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing sound service: {ex.Message}");
            }
        }

        public void PlayAlarm()
        {
            if (_soundPlayer != null && !_isPlaying)
            {
                try
                {
                    _soundPlayer.PlayLooping();
                    _isPlaying = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error playing alarm: {ex.Message}");
                }
            }
        }

        public void StopAlarm()
        {
            if (_soundPlayer != null && _isPlaying)
            {
                try
                {
                    _soundPlayer.Stop();
                    _isPlaying = false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error stopping alarm: {ex.Message}");
                }
            }
        }
    }
}
