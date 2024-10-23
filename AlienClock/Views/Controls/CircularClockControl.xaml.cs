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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AlienClock.Models;
using System;

namespace AlienClock.Views.Controls
{
    public partial class CircularClockControl : UserControl
    {
        private const double HourMarkLength = 15;
        private const double MinuteMarkLength = 10;
        private const double SecondMarkLength = 5;

        // Dependency Properties
        public static readonly DependencyProperty AlienTimeProperty =
            DependencyProperty.Register(
                "AlienTime",
                typeof(AlienDateTime),
                typeof(CircularClockControl),
                new PropertyMetadata(null, OnAlienTimeChanged));

        public AlienDateTime AlienTime
        {
            get => (AlienDateTime)GetValue(AlienTimeProperty);
            set => SetValue(AlienTimeProperty, value);
        }

        public CircularClockControl()
        {
            InitializeComponent();
            SizeChanged += CircularClockControl_SizeChanged;
        }

        private void CircularClockControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateClockSize();
            DrawClockFace();
            UpdateClockHands();
        }

        private static void OnAlienTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CircularClockControl clock)
            {
                clock.UpdateClockHands();
                clock.UpdateDigitalDisplay();
            }
        }

        private void UpdateClockSize()
        {
            double size = Math.Min(ActualWidth, ActualHeight);

            // Set the canvas size
            ClockCanvas.Width = size;
            ClockCanvas.Height = size;

            // Position the canvas in the center
            double left = (ActualWidth - size) / 2;
            double top = (ActualHeight - size) / 2;
            Canvas.SetLeft(ClockCanvas, left);
            Canvas.SetTop(ClockCanvas, top);

            // Update circle sizes
            OuterCircle.Width = size;
            OuterCircle.Height = size;

            double hourRingSize = size * 0.8;
            HourRing.Width = hourRingSize;
            HourRing.Height = hourRingSize;
            Canvas.SetLeft(HourRing, (size - hourRingSize) / 2);
            Canvas.SetTop(HourRing, (size - hourRingSize) / 2);

            double minuteRingSize = size * 0.6;
            MinuteRing.Width = minuteRingSize;
            MinuteRing.Height = minuteRingSize;
            Canvas.SetLeft(MinuteRing, (size - minuteRingSize) / 2);
            Canvas.SetTop(MinuteRing, (size - minuteRingSize) / 2);

            double secondRingSize = size * 0.4;
            SecondRing.Width = secondRingSize;
            SecondRing.Height = secondRingSize;
            Canvas.SetLeft(SecondRing, (size - secondRingSize) / 2);
            Canvas.SetTop(SecondRing, (size - secondRingSize) / 2);

            // Update center point
            Canvas.SetLeft(CenterPoint, size / 2 - 5);
            Canvas.SetTop(CenterPoint, size / 2 - 5);

            // Update digital display
            Canvas.SetLeft(DigitalDisplay, size / 2 - 60);
            Canvas.SetTop(DigitalDisplay, size * 0.7);
        }

        private void DrawClockFace()
        {
            ClockCanvas.Children.Clear();
            double size = ClockCanvas.Width;
            double center = size / 2;

            // Re-add the basic elements
            ClockCanvas.Children.Add(OuterCircle);
            ClockCanvas.Children.Add(HourRing);
            ClockCanvas.Children.Add(MinuteRing);
            ClockCanvas.Children.Add(SecondRing);

            // Draw hour markers (36 hours)
            for (int i = 0; i < 36; i++)
            {
                double angle = i * (360.0 / 36);
                DrawMarker(center, size * 0.4, angle, HourMarkLength, "#00FF00", 2);
            }

            // Draw minute markers (90 minutes)
            for (int i = 0; i < 90; i++)
            {
                double angle = i * (360.0 / 90);
                DrawMarker(center, size * 0.3, angle, MinuteMarkLength, "#00FF00", 1);
            }

            // Draw second markers (90 seconds)
            for (int i = 0; i < 90; i++)
            {
                double angle = i * (360.0 / 90);
                DrawMarker(center, size * 0.2, angle, SecondMarkLength, "#00FF00", 1);
            }

            // Add hands and center point back
            ClockCanvas.Children.Add(HourHand);
            ClockCanvas.Children.Add(MinuteHand);
            ClockCanvas.Children.Add(SecondHand);
            ClockCanvas.Children.Add(CenterPoint);
            ClockCanvas.Children.Add(DigitalDisplay);
        }

        private void DrawMarker(double center, double radius, double angle, double length, string color, double thickness)
        {
            double radians = angle * Math.PI / 180;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double x1 = center + (radius - length) * cos;
            double y1 = center + (radius - length) * sin;
            double x2 = center + radius * cos;
            double y2 = center + radius * sin;

            var marker = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = (Brush)new BrushConverter().ConvertFrom(color),
                StrokeThickness = thickness
            };

            ClockCanvas.Children.Add(marker);
        }

        private void UpdateClockHands()
        {
            if (AlienTime == null) return;

            double size = ClockCanvas.Width;
            double center = size / 2;

            // Calculate angles
            double hourAngle = (AlienTime.Hour * 360.0 / 36) + (AlienTime.Minute * 360.0 / (36 * 90));
            double minuteAngle = (AlienTime.Minute * 360.0 / 90) + (AlienTime.Second * 360.0 / (90 * 90));
            double secondAngle = AlienTime.Second * 360.0 / 90;

            // Update hour hand
            UpdateClockHand(HourHand, center, size * 0.3, hourAngle);

            // Update minute hand
            UpdateClockHand(MinuteHand, center, size * 0.25, minuteAngle);

            // Update second hand
            UpdateClockHand(SecondHand, center, size * 0.2, secondAngle);
        }

        private void UpdateClockHand(Line hand, double center, double length, double angle)
        {
            double radians = (angle - 90) * Math.PI / 180;

            hand.X1 = center;
            hand.Y1 = center;
            hand.X2 = center + length * Math.Cos(radians);
            hand.Y2 = center + length * Math.Sin(radians);
        }

        private void UpdateDigitalDisplay()
        {
            if (AlienTime == null) return;

            DateText.Text = AlienTime.ToShortDateString();
            TimeText.Text = AlienTime.ToShortTimeString();
        }
    }
}