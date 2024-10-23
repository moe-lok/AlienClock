using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienClock.Models
{
    public class AlienDateTime : IEquatable<AlienDateTime>
    {
        #region Properties
        public int Year { get; private set; }
        public int Month { get; private set; }  // 1-18
        public int Day { get; private set; }    // Varies by month
        public int Hour { get; private set; }   // 0-35
        public int Minute { get; private set; } // 0-89
        public int Second { get; private set; } // 0-89

        // Alien calendar constants
        private static readonly int[] DaysInMonth = new int[]
        {
            44, 42, 48, 40, 48, 44, 40, 44, 42,  // Months 1-9
            40, 40, 42, 44, 48, 42, 40, 44, 38   // Months 10-18
        };

        // Update the reference point (epoch) definitions
        private static readonly DateTime EarthEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly AlienDateTime AlienEpoch = new AlienDateTime(2804, 18, 31, 2, 2, 88);
        #endregion

        #region Constructors
        public AlienDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            ValidateAndSetTime(year, month, day, hour, minute, second);
        }

        // Copy constructor
        public AlienDateTime(AlienDateTime other)
        {
            Year = other.Year;
            Month = other.Month;
            Day = other.Day;
            Hour = other.Hour;
            Minute = other.Minute;
            Second = other.Second;
        }
        #endregion

        #region Public Methods
        public static AlienDateTime Now
        {
            get
            {
                return FromEarthDateTime(DateTime.Now);
            }
        }

        public DateTime ToEarthDateTime()
        {
            // Calculate total alien seconds difference from the epoch
            long epochTotalSeconds = CalculateTotalAlienSeconds(
                AlienEpoch.Year, AlienEpoch.Month, AlienEpoch.Day,
                AlienEpoch.Hour, AlienEpoch.Minute, AlienEpoch.Second);

            long currentTotalSeconds = CalculateTotalAlienSeconds(
                Year, Month, Day, Hour, Minute, Second);

            // Get the difference in alien seconds
            long diffAlienSeconds = currentTotalSeconds - epochTotalSeconds;

            // Convert to Earth seconds (1 alien second = 0.5 earth seconds)
            double earthSeconds = diffAlienSeconds * 0.5;

            return EarthEpoch.AddSeconds(earthSeconds);
        }

        public static AlienDateTime FromEarthDateTime(DateTime earthDateTime)
        {
            if (earthDateTime < EarthEpoch)
            {
                throw new ArgumentException("Cannot convert Earth time before epoch (1970-1-1)");
            }

            // Calculate difference in Earth seconds
            TimeSpan earthDiff = earthDateTime - EarthEpoch;

            // Convert to alien seconds (multiply by 2 because 1 earth second = 0.5 alien seconds)
            double alienSeconds = earthDiff.TotalSeconds * 2;

            return AlienEpoch.AddAlienSeconds((long)alienSeconds);
        }

        public AlienDateTime AddAlienSeconds(long seconds)
        {
            // Convert current time to total seconds
            long totalSeconds = CalculateTotalAlienSeconds(Year, Month, Day, Hour, Minute, Second);

            // Add the seconds
            totalSeconds += seconds;

            // Convert back to date/time components
            return CreateFromTotalSeconds(totalSeconds);
        }

        public DayOfAlienWeek DayOfWeek => (DayOfAlienWeek)((Day - 1) % 12);

        public int DaysInCurrentMonth => DaysInMonth[Month - 1];
        #endregion

        #region Private Methods
        private void ValidateAndSetTime(int year, int month, int day, int hour, int minute, int second)
        {
            // Validate all components
            if (month < 1 || month > 18)
                throw new ArgumentException($"Month must be between 1 and 18. Received: {month}");

            if (day < 1 || day > DaysInMonth[month - 1])
                throw new ArgumentException($"Day must be between 1 and {DaysInMonth[month - 1]} for month {month}. Received: {day}");

            if (hour < 0 || hour >= 36)
                throw new ArgumentException($"Hour must be between 0 and 35. Received: {hour}");

            if (minute < 0 || minute >= 90)
                throw new ArgumentException($"Minute must be between 0 and 89. Received: {minute}");

            if (second < 0 || second >= 90)
                throw new ArgumentException($"Second must be between 0 and 89. Received: {second}");

            // Set values if all validations pass
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        private static AlienDateTime CreateFromTotalSeconds(long totalSeconds)
        {
            int year = 2804;
            long remainingSeconds = totalSeconds;

            // Calculate years
            while (true)
            {
                long secondsInYear = 0;
                for (int m = 1; m <= 18; m++)
                {
                    secondsInYear += DaysInMonth[m - 1] * 36L * 90 * 90;
                }

                if (remainingSeconds < secondsInYear)
                    break;

                remainingSeconds -= secondsInYear;
                year++;
            }

            // Calculate months
            int month = 1;
            while (month <= 18)
            {
                long secondsInMonth = DaysInMonth[month - 1] * 36L * 90 * 90;
                if (remainingSeconds < secondsInMonth)
                    break;

                remainingSeconds -= secondsInMonth;
                month++;
            }

            // Calculate days
            long secondsInDay = 36L * 90 * 90;
            int day = 1 + (int)(remainingSeconds / secondsInDay);
            remainingSeconds %= secondsInDay;

            // Calculate hours
            long secondsInHour = 90L * 90;
            int hour = (int)(remainingSeconds / secondsInHour);
            remainingSeconds %= secondsInHour;

            // Calculate minutes
            int minute = (int)(remainingSeconds / 90);
            remainingSeconds %= 90;

            // Remaining seconds
            int second = (int)remainingSeconds;

            return new AlienDateTime(year, month, day, hour, minute, second);
        }

        private long CalculateTotalAlienSeconds(int year, int month, int day, int hour, int minute, int second)
        {
            long totalSeconds = 0;

            // Add years
            int baseYear = 2804; // Use this as a base year for all calculations
            for (int y = baseYear; y < year; y++)
            {
                for (int m = 1; m <= 18; m++)
                {
                    totalSeconds += DaysInMonth[m - 1] * 36L * 90 * 90;
                }
            }

            // Add months
            for (int m = 1; m < month; m++)
            {
                totalSeconds += DaysInMonth[m - 1] * 36L * 90 * 90;
            }

            // Add remaining components
            totalSeconds += (day - 1) * 36L * 90 * 90;
            totalSeconds += hour * 90L * 90;
            totalSeconds += minute * 90L;
            totalSeconds += second;

            return totalSeconds;
        }

        private long CalculateTotalAlienSecondsFromEpoch()
        {
            long totalSeconds = 0;

            // Add complete years
            for (int y = AlienEpoch.Year; y < Year; y++)
            {
                for (int m = 1; m <= 18; m++)
                {
                    totalSeconds += DaysInMonth[m - 1] * 36L * 90 * 90;
                }
            }

            // Add months in current year
            for (int m = 1; m < Month; m++)
            {
                totalSeconds += DaysInMonth[m - 1] * 36L * 90 * 90;
            }

            // Add remaining components
            totalSeconds += (Day - 1) * 36L * 90 * 90;
            totalSeconds += Hour * 90L * 90;
            totalSeconds += Minute * 90L;
            totalSeconds += Second;

            return totalSeconds;
        }

        private long CalculateTotalAlienSecondsFromYear2804()
        {
            long totalSeconds = 0;

            // Add complete years from 2804
            for (int y = 2804; y < Year; y++)
            {
                for (int m = 1; m <= 18; m++)
                {
                    totalSeconds += DaysInMonth[m - 1] * 36L * 90 * 90;
                }
            }

            // Add months in current year
            for (int m = 1; m < Month; m++)
            {
                totalSeconds += DaysInMonth[m - 1] * 36L * 90 * 90;
            }

            // Add days
            totalSeconds += (Day - 1) * 36L * 90 * 90;

            // Add hours
            totalSeconds += Hour * 90L * 90;

            // Add minutes
            totalSeconds += Minute * 90L;

            // Add seconds
            totalSeconds += Second;

            return totalSeconds;
        }

        private static AlienDateTime CalculateTimeFromTotalSeconds(long totalSeconds)
        {
            long secondsInDay = 36L * 90 * 90;
            long secondsInHour = 90L * 90;
            long secondsInMinute = 90L;

            int year = AlienEpoch.Year;
            int month = 1;
            int day = 1;

            // Calculate years and remaining seconds
            while (true)
            {
                long secondsInYear = 0;
                for (int m = 1; m <= 18; m++)
                {
                    secondsInYear += DaysInMonth[m - 1] * secondsInDay;
                }

                if (totalSeconds < secondsInYear)
                    break;

                totalSeconds -= secondsInYear;
                year++;
            }

            // Calculate months
            while (totalSeconds >= DaysInMonth[month - 1] * secondsInDay)
            {
                totalSeconds -= DaysInMonth[month - 1] * secondsInDay;
                month++;
            }

            // Calculate days
            day += (int)(totalSeconds / secondsInDay);
            totalSeconds %= secondsInDay;

            // Calculate hours
            int hour = (int)(totalSeconds / secondsInHour);
            totalSeconds %= secondsInHour;

            // Calculate minutes
            int minute = (int)(totalSeconds / secondsInMinute);
            totalSeconds %= secondsInMinute;

            // Remaining seconds
            int second = (int)totalSeconds;

            return new AlienDateTime(year, month, day, hour, minute, second);
        }
        #endregion

        #region Overrides and Interface Implementations
        public override string ToString()
        {
            return $"Year {Year}, Month {Month}, Day {Day}, {Hour:D2}:{Minute:D2}:{Second:D2}";
        }

        public string ToShortTimeString()
        {
            return $"{Hour:D2}:{Minute:D2}:{Second:D2}";
        }

        public string ToShortDateString()
        {
            return $"{Year}-{Month:D2}-{Day:D2}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AlienDateTime);
        }

        public bool Equals(AlienDateTime other)
        {
            return other != null &&
                   Year == other.Year &&
                   Month == other.Month &&
                   Day == other.Day &&
                   Hour == other.Hour &&
                   Minute == other.Minute &&
                   Second == other.Second;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Month, Day, Hour, Minute, Second);
        }
        #endregion
    }
}
