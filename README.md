# Alien Clock

A WPF application that implements an alien time system with Earth time conversion and alarm functionality.
![AlienClock Screenshot](https://github.com/moe-lok/AlienClock/blob/master/AlienClockScreenshot.png?raw=true)
## Features

### Time Display
- Real-time alien clock with both analog and digital display
- Shows current alien time:
  - 36 hours per day
  - 90 minutes per hour
  - 90 seconds per minute
  - 18 months per year with varying days per month
  - 12-day week system
- Displays corresponding Earth time
- Updates every alien second (0.5 Earth seconds)

### Time Conversion
- Accurate conversion between Earth and Alien time systems
- Reference point: 
  - Earth time: 1970-1-1 12:00:00am 
  - Alien time: Year 2804, Month 18, Day 31, Hour 2, Minute 2, Second 88

### Time Setting
- Manual time setting with validation
- Input validation for:
  - Hours (0-35)
  - Minutes (0-89)
  - Seconds (0-89)
  - Months (1-18)
  - Days (varies by month)

### Alarm System
- Set alarms with specific alien time
- Select active days for the alarm
- Alarm notification with sound
- Snooze and dismiss options

## Technical Details

### Built With
- C# WPF (.NET 6.0)
- MVVM Architecture
- Custom Controls for Clock Display

### Project Structure
- Models: Core data structures and time conversion logic
- ViewModels: Application logic and state management
- Views: User interface components
- Services: Settings persistence and sound management

## Installation

1. Download the latest release
2. Extract all files to a directory
3. Run AlienClock.exe

## Usage

### Setting Time
1. Click "Set Time" button
2. Enter desired time values
3. Click "Save" to apply or "Cancel" to discard changes

### Setting Alarm
1. Click "Set Alarm" button
2. Enable alarm using checkbox
3. Set desired alarm time
4. Select active days
5. Click "Save" to apply settings

## Development Setup

1. Clone the repository
2. Open AlienClock.sln in Visual Studio
3. Restore NuGet packages
4. Build and run the solution

## License

This project was created as part of a technical assessment and is for demonstration purposes.
