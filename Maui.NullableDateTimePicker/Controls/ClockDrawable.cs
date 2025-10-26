namespace Maui.NullableDateTimePicker.Controls
{
    internal class ClockDrawable : IDrawable
    {
        private int _hour12;
        private int _minute;
        private int _second;

        public int Hour => _hour12;
        public int Minute => _minute;
        public int Second => _second;

        private const float ClockRadius = 90;
        //private readonly List<(float X, float Y, string Group, int Value)> clickablePoints = new();
        public bool Is12HourFormat { get; set; }
        public bool IsAmMode { get; set; }
        public bool Is24Mode => !Is12HourFormat && !IsAmMode;

        public bool IsHourMode { get; set; } = true;

        //public TappedGroup LastTappedGroup { get; private set; } = TappedGroup.None;
        //public int LastTappedValue { get; private set; }

        public ClockDrawable()
        {
        }


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Save the initial canvas state (important for Windows)
            canvas.SaveState();

            //clickablePoints.Clear(); // Reset and recalculate the points

            float centerX = dirtyRect.Center.X;
            float centerY = dirtyRect.Center.Y;

            // Translation and scaling
            canvas.Translate(centerX, centerY);
            float scale = Math.Min(dirtyRect.Width / 200f, dirtyRect.Height / 200f);
            canvas.Scale(scale, scale);

            // Set the background of the clock circle to gray
            canvas.FillColor = Colors.LightGray; // Background color (gray)
            canvas.FillCircle(0, 0, ClockRadius + 10); // Background of the clock circle (slightly larger than the circle of dots)

            if (IsHourMode)
            {
                // Place the _hour12 numbers and dots
                for (int i = 1; i <= 12; i++)
                {
                    // Calculate the angle of the _hour12 (12-_hour12 circle)
                    double angle = (i - 3) * 30 * Math.PI / 180; // Hour angles (0° corresponds to 3 o'clock)

                    // Calculate the position of the _hour12 dot
                    float x = (float)(ClockRadius * Math.Cos(angle));
                    float y = (float)(ClockRadius * Math.Sin(angle));

                    //float scaledX = (x - centerX) / scale;
                    //float scaledY = (y - centerY) / scale;
                    //clickablePoints.Add((x, y, "Hour", i));

                    // Draw the large dot
                    canvas.FontSize = 11;
                    canvas.FillColor = Color.FromArgb("#4f20b0");
                    canvas.FillCircle(x, y, 3);

                    // Place the _hour12 number on the inner side of the circle
                    canvas.FontColor = Colors.Black;

                    float textX = (float)((ClockRadius - 10) * Math.Cos(angle));
                    float textY = (float)((ClockRadius - 10) * Math.Sin(angle));

                    string hourText = Is12HourFormat
                                    ? i.ToString()
                                    : Is24Mode
                                        ? (i % 12 + 12).ToString("00") // noon: 13–23, 12 → 12
                                        : (i % 12 == 0 ? "00" : (i % 12).ToString("00")); // night: 1–11, 12 → 00

                    // Create a Rect for aligning the numbers
                    RectF textRect = new RectF(textX - 10, textY - 10, 20, 20); // Text rectangle (dimensions can be adjusted)
                    canvas.DrawString(hourText, textRect, HorizontalAlignment.Center, VerticalAlignment.Center);
                }
            }
            else
            {
                for (int i = 0; i < 60; i++)
                {
                    //Calculate the angle (0° represents 3)
                    double angle = (i - 15) * 6 * Math.PI / 180;

                    // Calculate the location of the marking points
                    float x = (float)(ClockRadius * Math.Cos(angle));
                    float y = (float)(ClockRadius * Math.Sin(angle));

                    // Add clickable point
                    //clickablePoints.Add((x, y, "Minute", i));

                    // Set color and size (large in 5 minutes)
                    canvas.FontSize = 11;
                    canvas.FillColor = Color.FromArgb("#4f20b0");
                    float radius = i % 5 == 0 ? 3 : 1;
                    canvas.FillCircle(x, y, radius);

                    // Draw a number every 5 minutes
                    if (i % 5 == 0)
                    {
                        float textX = (float)((ClockRadius - 10) * Math.Cos(angle));
                        float textY = (float)((ClockRadius - 10) * Math.Sin(angle));
                        string minuteText = i == 0 ? "00" : i.ToString("D2");

                        RectF textRect = new RectF(textX - 10, textY - 10, 20, 20);
                        canvas.DrawString(minuteText, textRect, HorizontalAlignment.Center, VerticalAlignment.Center);
                    }
                }
            }

           

            // Draw the _hour12 hand
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 3;
            canvas.SaveState();
            canvas.Rotate(30 * _hour12 + _minute / 2f);
            canvas.DrawLine(0, 0, 0, -50);
            canvas.RestoreState();

            // Draw the _minute hand
            canvas.StrokeSize = 2;
            canvas.SaveState();
            canvas.Rotate(6 * _minute + _second / 10f);
            canvas.DrawLine(0, 0, 0, -70);
            canvas.RestoreState(); 
            
            // Small circle at the center
            canvas.FillColor = Colors.Black;
            canvas.FillCircle(0, 0, 4);

            canvas.RestoreState();
        }

        //public void HandleMinuteTap(float touchX, float touchY, float centerX, float centerY, float scale)
        //{
        //    //Scale touch point relative to center
        //    float scaledX = (touchX - centerX) / scale;
        //    float scaledY = (touchY - centerY) / scale;

        //    // Calculate angle (Atan2 uses y, x)
        //    double angle = Math.Atan2(scaledY, scaledX);

        //    // Shift -90° (π/2) to align 0° to 3 o'clock
        //    double normalizedAngle = (angle + Math.PI / 2) % (2 * Math.PI);

        //    // Scale angle to 60 minutes
        //    int _minute = (int)((normalizedAngle / (2 * Math.PI)) * 60);

        //    // Correct negative values
        //    _minute = (_minute + 60) % 60;

        //    // Sonucu ata
        //    LastTappedGroup = "Minute";
        //    LastTappedValue = _minute;

        //    Console.WriteLine($"Tapped Minute: {_minute}");
        //}

        public void HandleTap(float touchX, float touchY, float centerX, float centerY, float scale)
        {
            // Scale touch point relative to center
            float scaledX = (touchX - centerX) / scale;
            float scaledY = (touchY - centerY) / scale;

            // Calculate angle (Atan2 uses y, x)
            double angle = Math.Atan2(scaledY, scaledX);

            // Shift -90° (π/2) to align 0° to 3 o'clock
            double normalizedAngle = (angle + 2 * Math.PI + Math.PI / 2) % (2 * Math.PI);

            if (IsHourMode)
            {
                // **Hour Mode (In the 12th slice)**
                int hour = (int)((normalizedAngle / (2 * Math.PI)) * 12 + 0.5); // Align to center by adding 0.5
                hour = (hour + 12) % 12;
                hour = hour == 0 ? 12 : hour; // Correct 0 to 12
                _hour12 = hour;
                //LastTappedGroup = TappedGroup.Hour;
                //LastTappedValue = hour;
                Console.WriteLine($"Tapped Hour: {hour}");
            }
            else
            {
                // **Minute Mode (in 60s)**
                int minute = (int)((normalizedAngle / (2 * Math.PI)) * 60 + 0.5); // Align to center by adding 0.5
                minute = (minute + 60) % 60;
                _minute = minute;
                //LastTappedGroup = TappedGroup.Minute;
                //LastTappedValue = minute;
                Console.WriteLine($"Tapped Minute: {minute}");
            }
            IsHourMode = !IsHourMode;
        }

        public void SetClockValueFromTime(TimeOnly timeOnly)
        {
            int hour12 = timeOnly.Hour % 12;
            int minute = timeOnly.Minute;
            int second = timeOnly.Second;

            if (hour12 == 0)
                hour12 = 12;

            if (_hour12 != hour12 || _minute != minute || _second != second)
            {
                _hour12 = hour12;
                _minute = minute;
                _second = second;
            }
        }


        //private int FindNearestMinute(float touchX, float touchY)
        //{
        //    int nearestMinute = -1;
        //    float minDistance = float.MaxValue;

        //    foreach (var (x, y, type, _minute) in clickablePoints)
        //    {
        //        float distance = (float)Math.Sqrt(Math.Pow(x - touchX, 2) + Math.Pow(y - touchY, 2));

        //        if (distance < minDistance)
        //        {
        //            minDistance = distance;
        //            nearestMinute = _minute;
        //        }
        //    }

        //    // If the distance is greater than a certain threshold, it is not a valid click
        //    return minDistance < 15 ? nearestMinute : -1;  // 15 pixel threshold
        //}


        //private bool IsPointTapped(float touchX, float touchY, float pointX, float pointY, float radius)
        //{
        //    float distance = (float)Math.Sqrt(Math.Pow(touchX - pointX, 2) + Math.Pow(touchY - pointY, 2));
        //    return distance <= radius;
        //}

        //public void ToggleAmPmMode(bool isAm)
        //{
        //    IsAmMode = isAm;
        //}
    }

    //internal enum TappedGroup
    //{
    //    None,
    //    Hour,
    //    Minute
    //}
}
