using Maui.NullableDateTimePicker.Models;
using System.Globalization;

namespace Maui.NullableDateTimePicker;

internal class NullableDateTimePickerContent : ContentView
{
    internal event EventHandler<EventArgs> OkButtonClicked;
    internal event EventHandler<EventArgs> ClearButtonClicked;
    internal event EventHandler<EventArgs> CancelButtonClicked;
    private DateTime? _selectedDate;
    private DateTime _currentDate;
    readonly DateTime _minDate;
    readonly DateTime _maxDate;
    readonly INullableDateTimePickerOptions _options;
    private Grid _calendarGrid;
    private Button _okButton;
    private Button _cancelButton;
    private Button _clearButton;
    private Grid _daysGrid;
    private Picker _yearsPicker;
    private Label _selectedDateLabel;
    private Label _monthYearLabel;
    private Button _previousMonthButton;
    private Button _nextMonthButton;
    private Style _selectedDayStyle;
    private Style _dayStyle;
    private Style _disabledDayStyle;
    private Style _otherMonthDayStyle;
    private Style _dayNamesStyle;
    private Style _toolButtonsStyle;
    private Style _weekNumberStyle;
    private Picker _hoursPicker;
    private Picker _minutesPicker;
    private Picker _amPmPicker;
    private StackLayout _timeStackLayout;
    private List<Button> _dayButtons;
    private Grid _monthListGrid;
    private Grid _mainGrid;
    private ActivityIndicator _activityIndicator;
    private ScrollView _scrollView;
    List<PickerItem> _hours = null;
    List<PickerItem> _minutes = null;


    internal NullableDateTimePickerContent(INullableDateTimePickerOptions options)
    {
        base.Padding = 0;
        base.Margin = 0;
        options ??= new NullableDateTimePickerOptions();

        _options = options;
        _selectedDate = options.NullableDateTime;
        _currentDate = options.NullableDateTime ?? DateTime.Now;
        _minDate = options.MinDate ?? new DateTime(1900, 1, 1);
        _maxDate = options.MaxDate ?? new DateTime(DateTime.Now.Year + 100, 12, 31);

        _mainGrid = new Grid
        {
            Margin = new Thickness(0),
            Padding = new Thickness(0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.Transparent
        };

        _activityIndicator = new ActivityIndicator
        {
            IsVisible = true,
            IsRunning = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        _scrollView = new ScrollView
        {
            Margin = new Thickness(0),
            Padding = new Thickness(0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Content = new Grid
            {
                Children =
                {
                    _mainGrid,
                    _activityIndicator
                }
            }
        };

        Content = _scrollView;

        if (_options.ActivityIndicatorColor != null)
            _activityIndicator.Color = _options.ActivityIndicatorColor;
    }

    internal async void NullableDateTimePickerPopupOpened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        await InitCalendar();
    }

    internal async Task InitCalendar()
    {
        try
        {
            await InitContent();

            await PopulatePickers();

            await SetCurrentDateAndRebuildCalendar(_currentDate.Year, _currentDate.Month, _currentDate.Day);
        }
        catch (Exception ex)
        {
            Console.Write($"BuildCalendar-Error: {ex}");
        }
    }

    internal DateTime? SelectedDate
    {
        get { return _selectedDate; }
        private set
        {
            var date = value;
            _selectedDate = date;
        }
    }

    private async void OnPreviousMonthButtonClicked(object sender, EventArgs e)
    {
        await SetPreviousMonth();
    }

    internal async Task SetPreviousMonth()
    {
        var previousMonth = _currentDate.Month;
        var currentYear = _currentDate.Year;

        if (_monthListGrid?.IsVisible == true)
            currentYear -= 1;
        else
            previousMonth -= 1;

        if (previousMonth < 1)
        {
            previousMonth = 12;
            currentYear--;
        }
        await SetCurrentDateAndRebuildCalendar(currentYear, previousMonth, _currentDate.Day);
    }

    private async void OnNextMonthButtonClicked(object sender, EventArgs e)
    {
        await SetNextMonthOrYear();
    }

    private async Task SetNextMonthOrYear()
    {
        var nextMonth = _currentDate.Month;
        var currentYear = _currentDate.Year;
        if (_monthListGrid?.IsVisible == true)
            currentYear += 1;
        else
            nextMonth += 1;

        if (nextMonth > 12)
        {
            nextMonth = 1;
            currentYear++;
        }

        await SetCurrentDateAndRebuildCalendar(currentYear, nextMonth, _currentDate.Day);
    }

    private void OnDayButtonTapped(object sender, EventArgs e)
    {
        if (_options.Mode == PickerModes.Time) //Click skipping in time mode 
            return;

        Button dayButton = sender as Button;
        var day = Convert.ToInt32(dayButton.Text);

        if (DayDisabled(_currentDate.Year, _currentDate.Month, day))
            return;

        UpdateCurrentDateAndControls(new DateTime(_currentDate.Year, _currentDate.Month, day, _currentDate.Hour, _currentDate.Minute, _currentDate.Second));
    }

    private async void OnLastMonthDayButtonTapped(object sender, EventArgs e)
    {
        if (_options.Mode == PickerModes.Time) //Click skipping in time mode 
            return;
        int year = _currentDate.Year;
        int month = _currentDate.Month;
        if (month == 1)
        {
            month = 12;
            year--;
        }
        else
            month--;

        Button dayButton = sender as Button;
        int day = Convert.ToInt32(dayButton.Text);
        if (DayDisabled(year, month, day))
            return;

        await SetCurrentDateAndRebuildCalendar(year, month, day);
    }

    private async void OnNextMonthDayButtonTapped(object sender, EventArgs e)
    {
        if (_options.Mode == PickerModes.Time) //Click skipping in time mode 
            return;
        int year = _currentDate.Year;
        int month = _currentDate.Month;
        if (month == 12)
        {
            month = 1;
            year++;
        }
        else
            month++;

        Button dayButton = sender as Button;
        int day = Convert.ToInt32(dayButton.Text);

        if (DayDisabled(year, month, day))
            return;

        await SetCurrentDateAndRebuildCalendar(year, month, day);
    }


    internal async Task SetCurrentDateAndRebuildCalendar(int year, int month, int day)
    {
        FixAndSetCurrentDate(year, month, day);
        await BuildCalendar();
    }

    private void FixAndSetCurrentDate(int year, int month, int day)
    {
        if (year > DateTime.MaxValue.Year)
            year = DateTime.MaxValue.Year;
        else if (year < DateTime.MinValue.Year)
            year = DateTime.MinValue.Year;

        if (month > 12)
            month = 12;
        else if (month < 1)
            month = 1;

        var daysInMonth = DateTime.DaysInMonth(year, month);
        if (day > daysInMonth)
            day = daysInMonth;
        else if (day < 1)
            day = 1;

        var currentDate = new DateTime(year, month, day, _currentDate.Hour, _currentDate.Minute, _currentDate.Second);
        if (currentDate < _minDate)
            _currentDate = _minDate;
        else if (currentDate > _maxDate)
            _currentDate = _maxDate;
        else
            _currentDate = currentDate;
    }

    private async Task BuildCalendar()
    {
        await MainThreadHelper.SafeInvokeOnMainThreadAsync(() =>
        {
            try
            {
                _activityIndicator.IsVisible = true;
                _activityIndicator.IsRunning = true;

                lastClickedDayButton = null;
                _daysGrid.Clear();
                _daysGrid.Children?.Clear();

                DateTime lastMonthDate = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddMonths(-1);
                DateTime nextMonthDate = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddMonths(1);
                int daysInLastMonth = DateTime.DaysInMonth(lastMonthDate.Year, lastMonthDate.Month);
                //int daysInNextMonth = DateTime.DaysInMonth(firstDayOfNextMonth.Year, firstDayOfNextMonth.Month);

                if (!_options.ShowWeekNumbers)
                    _daysGrid.ColumnDefinitions[0].Width = 0;

                _previousMonthButton.IsVisible = true;
                if (DayDisabled(lastMonthDate.Year, lastMonthDate.Month, daysInLastMonth))
                    _previousMonthButton.IsVisible = false;

                _nextMonthButton.IsVisible = true;
                if (DayDisabled(nextMonthDate.Year, nextMonthDate.Month, 1))
                    _nextMonthButton.IsVisible = false;

                // Get the first day of the month and the number of days in the month
                DayOfWeek firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1).DayOfWeek;
                int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);

                // Get the DateTimeFormatInfo for the current culture
                DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
                string[] dayNames = dateTimeFormatInfo.AbbreviatedDayNames;
                int firstDayOfWekkIndex = (int)dateTimeFormatInfo.FirstDayOfWeek;

                // Calculate the week number for this day


                // Rotate the array so that the first day of the week comes first
                string[] rotatedDayNames = dayNames.Skip(firstDayOfWekkIndex)
                                                           .Concat(dayNames.Take(firstDayOfWekkIndex))
                                                           .ToArray();

                var dayLabels = new List<Label>();
                // Add the day labels to the top row of the grid
                for (int i = 0; i < rotatedDayNames.Length; i++)
                {
                    string dayName = rotatedDayNames[i];

                    Label label = new()
                    {
                        Text = dayName,
                        Style = _options.DayNamesStyle ?? _dayNamesStyle as Style
                    };
                    dayLabels.Add(label);
                }

                for (int i = 0; i < dayLabels.Count; i++)
                {
                    _daysGrid.Add(dayLabels[i], i + 1, 0);
                }

                int row = 1;
                int col = ((int)firstDayOfMonth - firstDayOfWekkIndex + 7) % 7;

                // Fill the day grid with buttons for each day of the month
                for (int day = 1; day <= daysInMonth; day++)
                {
                    var dayButton = _dayButtons[day - 1];

                    if (dayButton.Style != _dayStyle)
                    {
                        dayButton.Style = _dayStyle;
                    }

                    if (DayDisabled(_currentDate.Year, _currentDate.Month, day))
                    {
                        dayButton.Style = _disabledDayStyle;
                    }

                    _daysGrid.Add(dayButton, col + 1, row);

                    col++;

                    if (col > 6)
                    {
                        col = 0;
                        row++;
                    }
                }

                // week numbers
                if (_options.ShowWeekNumbers)
                {
                    // Calculate the number of weeks in the current year
                    int weeksInYear = GetIsoWeekOfYear(new DateTime(_currentDate.Year, 12, 31)) == 53 ? 53 : 52;

                    // Calculate the week number for the first day of the current month
                    int weekNumber = GetIsoWeekOfYear(new DateTime(_currentDate.Year, _currentDate.Month, 1));

                    for (int weekRow = 1; weekRow <= 6; weekRow++)
                    {
                        // Add the week number label to the grid
                        _daysGrid.Add(new Label { Text = weekNumber.ToString(), Style = _weekNumberStyle }, 0, weekRow);

                        // Move to the next day
                        weekNumber++;

                        // If we have reached the end of the year, reset the week number to 1
                        if (weekNumber > weeksInYear)
                            weekNumber = 1;
                    }
                }

                // other month days
                if (_options.ShowOtherMonthDays)
                {
                    // Fill in days for the last month
                    int daysNeededFromLastMonth = ((int)firstDayOfMonth - firstDayOfWekkIndex + 7) % 7;
                    for (int i = daysNeededFromLastMonth - 1; i >= 0; i--)
                    {
                        int dayInLastMonth = daysInLastMonth - i;
                        Button lastMonthDayButton = new()
                        {
                            Text = dayInLastMonth.ToString(),
                            Style = _otherMonthDayStyle
                        };

                        if (DayDisabled(lastMonthDate.Year, lastMonthDate.Month, dayInLastMonth))
                            lastMonthDayButton.Style = _disabledDayStyle;

                        lastMonthDayButton.Clicked += OnLastMonthDayButtonTapped;

                        _daysGrid.Add(lastMonthDayButton, daysNeededFromLastMonth - i, 1); //the first column of the grid is for the week numbers
                    }

                    int lastWeekLastDayIndex = ((int)nextMonthDate.DayOfWeek - 1 + 7) % 7;

                    for (int i = 0; i < 6 * 7 - daysInMonth - daysNeededFromLastMonth; i++)
                    {
                        int dayInNextMonth = i + 1;
                        Button nextMonthDayButton = new()
                        {
                            Text = dayInNextMonth.ToString(),
                            Style = _otherMonthDayStyle
                        };

                        if (DayDisabled(nextMonthDate.Year, nextMonthDate.Month, dayInNextMonth))
                            nextMonthDayButton.Style = _disabledDayStyle;

                        nextMonthDayButton.Clicked += OnNextMonthDayButtonTapped;

                        int nextDayRow = (daysNeededFromLastMonth + daysInMonth + i) / 7 + 1; //Add 1 because the first row is for day names
                        int nextDayCol = (daysNeededFromLastMonth + daysInMonth + i) % 7 + 1; //Add 1 because the first column is for week numbers

                        _daysGrid.Add(nextMonthDayButton, nextDayCol, nextDayRow);
                    }
                }

                //If the days from the previous or next month are not being displayed and the last row is empty, it should not be shown.
                // Check if the last row is empty
                bool lastRowEmpty = true;
                for (int i = 1; i <= 7; i++)
                {
                    if (_daysGrid.Children.LastOrDefault(c => _daysGrid.GetRow(c) == 6 && _daysGrid.GetColumn(c) == i) is Button lastButton && !string.IsNullOrEmpty(lastButton.Text))
                    {
                        lastRowEmpty = false;
                        break;
                    }
                }

                // If the last row is empty, set its height to 0
                if (lastRowEmpty)
                {
                    _daysGrid.RowDefinitions[6].Height = new GridLength(0);
                }

                UpdateCurrentDateAndControls(_currentDate);

                _activityIndicator.IsVisible = false;
                _activityIndicator.IsRunning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        });
    }


    /// <summary>
    /// ISO 8601 week of year.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private static int GetIsoWeekOfYear(DateTime date)
    {
        return System.Globalization.ISOWeek.GetWeekOfYear(date);
    }


    private void UpdateCurrentDateAndControls(DateTime? date)
    {
        _currentDate = date ?? DateTime.Now;
        Console.Write($"UpdateCurrentDate: {_currentDate}");

        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
       {
           _yearsPicker.SelectedItem = _currentDate.Year;
           if (_options.Mode != PickerModes.Date)
           {
               int currentHour = _currentDate.Hour;
               if (_options.Is12HourFormat)
               {
                   currentHour = ConvertTo12HourFormat(currentHour);
               }
               _hoursPicker.SelectedItem = _hours?.FirstOrDefault(x => x.Value == currentHour);
               _minutesPicker.SelectedItem = _minutes?.FirstOrDefault(x => x.Value == _currentDate.Minute);

               if (_options.Is12HourFormat)
                   _amPmPicker.SelectedItem = _currentDate.ToString("tt");
           }
           _monthYearLabel.Text = _currentDate.ToString("MMMM yyyy");
           if (_options.Mode == PickerModes.Time)
           {
               _selectedDateLabel.Text = date.HasValue ? date.Value.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern) : "";
           }
           else
           {
               _selectedDateLabel.Text = date.HasValue ? date.Value.ToString("ddd, MMM d") : "No Date Selected";
           }
           SetCurrentDayStyle(_currentDate.Day.ToString());
       });
    }

    Button lastClickedDayButton = null;
    private void SetCurrentDayStyle(string day)
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
        {
            if (lastClickedDayButton != null)
                lastClickedDayButton.Style = _dayStyle;

            var currentButton = _dayButtons.FirstOrDefault(b => b.Text == day);
            if (currentButton != null)
            {
                currentButton.Style = _selectedDayStyle;
                lastClickedDayButton = currentButton;
            }
        });
    }

    private async Task PopulatePickers()
    {
        // Years picker
        List<int> years = new();
        int minYear = _minDate.Year;
        int maxYear = _maxDate.Year;
        for (int y = minYear; y <= maxYear; y++)
        {
            years.Add(y);
        }


        // Hours and Minutes Picker
        if (_options.Mode != PickerModes.Date)
        {
            _hours = new();

            int maxHour = _options.Is12HourFormat ? 12 : 24;
            for (int h = 1; h <= maxHour; h++)
            {
                string hourText = h.ToString("00");

                _hours.Add(new PickerItem { Text = hourText, Value = h == 24 ? 0 : h });
            }


            _minutes = new();
            for (int m = 0; m < 60; m++)
            {
                _minutes.Add(new PickerItem { Text = m.ToString("00"), Value = m });
            }
        }

        await MainThreadHelper.SafeInvokeOnMainThreadAsync(() =>
        {
            //years
            _yearsPicker.ItemsSource = years;

            // _hours
            if (_hoursPicker != null)
                _hoursPicker.ItemsSource = _hours;

            // _minutes
            if (_minutesPicker != null)
                _minutesPicker.ItemsSource = _minutes;

            // am/pm
            if (_options.Is12HourFormat && _amPmPicker != null)
            {
                _amPmPicker.Items.Add("AM");
                _amPmPicker.Items.Add("PM");
            }
        });
    }

    internal void OnClearButtonClicked(object sender, EventArgs e)
    {
        SelectedDate = null;
        UpdateCurrentDateAndControls(null);
        ClearButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    internal void OnCancelButtonClicked(object sender, EventArgs e)
    {
        CancelButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    internal void OnOkButtonClicked(object sender, EventArgs e)
    {
        SelectedDate = _currentDate;
        OkButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    private async void OnYearsPickerIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = ((Picker)sender).SelectedItem;
        if (selectedItem == null)
            return;

        await SetYear((int)selectedItem);
    }

    internal async Task SetYear(int year)
    {
        if (year == _currentDate.Year)
            return;

        if (year < _minDate.Year)
            year = _minDate.Year;
        if (year > _maxDate.Year)
            year = _maxDate.Year;

        await SetCurrentDateAndRebuildCalendar(year, _currentDate.Month, _currentDate.Day);
    }

    private void OnHoursPickerIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = ((Picker)sender).SelectedItem as PickerItem;
        if (selectedItem == null)
            return;

        int selectedHour = selectedItem.Value;
        if (_options.Is12HourFormat)
        {
            selectedHour = ConvertTo24HourFormat(selectedHour, _currentDate);
        }
        SetHour(selectedHour);
    }

    internal void SetHour(int hour)
    {
        if (hour < 0 || hour > 23 || _currentDate.Hour == hour)
            return;

        UpdateCurrentDateAndControls(new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, hour, _currentDate.Minute, _currentDate.Second));
    }

    private void OnMinutesPickerIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = ((Picker)sender).SelectedItem as PickerItem;
        if (selectedItem == null)
            return;

        SetMinute(selectedItem.Value);
    }

    internal void SetMinute(int minute)
    {
        if (minute < 0 || minute > 59 || _currentDate.Minute == minute)
            return;

        UpdateCurrentDateAndControls(new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, _currentDate.Hour, minute, _currentDate.Second));
    }

    private void OnAmPmPickerIndexChanged(object sender, EventArgs e)
    {
        var selectedAmPmItem = ((Picker)sender).SelectedItem as string;
        if (string.IsNullOrEmpty(selectedAmPmItem))
            return;

        SetHour(UpdateHourByAmPmOption(_currentDate.Hour, selectedAmPmItem));
    }

    private async Task InitContent()
    {
        #region Styles
        // DayStyle
        _dayStyle = new Style(targetType: typeof(Button));

        if (_options.DayStyle != null)
        {
            _options.DayStyle.BasedOn = DefaultStyles.DayStyle;
            _dayStyle.BasedOn = _options.DayStyle;
        }
        else
        {
            _dayStyle.BasedOn = DefaultStyles.DayStyle;
        }

        _disabledDayStyle = new Style(targetType: typeof(Button));

        if (_options.DisabledDayStyle != null)
        {
            _options.DisabledDayStyle.BasedOn = DefaultStyles.DisabledDayStyle;
            _disabledDayStyle.BasedOn = _options.DisabledDayStyle;
        }
        else
        {
            _disabledDayStyle.BasedOn = DefaultStyles.DisabledDayStyle;
        }


        // OtherMonthDayStyle
        _otherMonthDayStyle = new Style(targetType: typeof(Button));
        if (_options.OtherMonthDayStyle != null)
        {
            _options.OtherMonthDayStyle.BasedOn = DefaultStyles.OtherMonthDayStyle;
            _otherMonthDayStyle.BasedOn = _options.OtherMonthDayStyle;
        }
        else
        {
            _otherMonthDayStyle.BasedOn = DefaultStyles.OtherMonthDayStyle;
        }

        // SelectedDayStyle
        _selectedDayStyle = new Style(targetType: typeof(Button));
        if (_options.SelectedDayStyle != null)
        {
            _options.SelectedDayStyle.BasedOn = DefaultStyles.SelectedDayStyle;
            _selectedDayStyle.BasedOn = _options.SelectedDayStyle;
        }
        else
        {
            _selectedDayStyle.BasedOn = DefaultStyles.SelectedDayStyle;
        }

        // WeekNumberStyle
        _weekNumberStyle = new Style(targetType: typeof(Label));

        if (_options.WeekNumberStyle != null)
        {
            _options.WeekNumberStyle.BasedOn = DefaultStyles.WeekNumberStyle;
            _weekNumberStyle.BasedOn = _options.WeekNumberStyle;
        }
        else
        {
            _weekNumberStyle.BasedOn = DefaultStyles.WeekNumberStyle;
        }

        // DayNamesStyle
        _dayNamesStyle = new Style(targetType: typeof(Label));
        if (_options.DayNamesStyle != null)
        {
            _options.DayNamesStyle.BasedOn = DefaultStyles.DayNamesStyle;
            _dayNamesStyle.BasedOn = _options.DayNamesStyle;
        }
        else
        {
            _dayNamesStyle.BasedOn = DefaultStyles.DayNamesStyle;
        }

        // ToolButtonsStyle
        _toolButtonsStyle = new Style(targetType: typeof(Button));

        if (_options.ToolButtonsStyle != null)
        {
            _options.ToolButtonsStyle.BasedOn = DefaultStyles.ToolButtonsStyle;
            _toolButtonsStyle.BasedOn = _options.ToolButtonsStyle;
        }
        else
        {
            _toolButtonsStyle.BasedOn = DefaultStyles.ToolButtonsStyle;
        }

        #endregion // Styles end

        if (_dayButtons == null)
        {
            _dayButtons = new List<Button>();
            for (int day = 1; day <= 31; day++)
            {
                Button button = new()
                {
                    Text = day.ToString(),
                    Style = _dayStyle,
                    IsEnabled = true
                };
                button.Clicked += OnDayButtonTapped;
                _dayButtons.Add(button);
            }
        }

        _calendarGrid = new Grid
        {
            BackgroundColor = _options.BodyBackgroundColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Color.FromRgba("#434343") : Colors.White),
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            ColumnSpacing = 0,
            RowSpacing = 0,
            MaximumWidthRequest = 300,
            MaximumHeightRequest = 450,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            },
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(90, GridUnitType.Absolute) }, // header
                new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) }, // pre/next button
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, // mnonth days
                new RowDefinition { Height = _options.Mode == PickerModes.Date // time
                ? new GridLength(0, GridUnitType.Absolute)
                :  new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) }, // buttons
            }
        };

        #region header

        Grid headerGrid = new()
        {
            BackgroundColor = _options.HeaderBackgroundColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Color.FromRgba("#252626") : Color.FromRgba("#2b0b98")),
            Padding = new Thickness(10, 0, 10, 0),
            Margin = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            }
        };

        _yearsPicker = new Picker
        {
            HeightRequest = 40,
            FontSize = 16,
            Margin = new Thickness(0),
            TextColor = _options.HeaderForeColor ?? Colors.White,
            TitleColor = _options.HeaderForeColor ?? Colors.White,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Start,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            IsEnabled = _options.Mode != PickerModes.Time  //Click skipping in time mode 
        };
        _yearsPicker.SelectedIndexChanged += OnYearsPickerIndexChanged;

        _selectedDateLabel = new Label
        {
            HeightRequest = 30,
            FontSize = 25,
            TextColor = _options.HeaderForeColor ?? Colors.White,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Start,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center
        };

        headerGrid.Add(_yearsPicker);
        headerGrid.SetRow(_yearsPicker, 0);
        headerGrid.Add(_selectedDateLabel);
        headerGrid.SetRow(_selectedDateLabel, 1);
        _calendarGrid.Add(headerGrid);

        #endregion //header

        Grid preNextButtonsGrid = new()
        {
            Padding = new Thickness(5, 0),
            Margin = 0,
            BackgroundColor = Colors.Transparent,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
            }
        };

        _previousMonthButton = new Button
        {
            Text = "<",
            TextColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
            BackgroundColor = Colors.Transparent,
            Background = Colors.Transparent,
            BorderColor = Colors.Transparent,
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
            WidthRequest = 50,
            Margin = 0,
            IsEnabled = _options.Mode != PickerModes.Time  //Click skipping in time mode 
        };
        _previousMonthButton.Clicked += OnPreviousMonthButtonClicked;
        preNextButtonsGrid.Add(_previousMonthButton, 0, 0);

        _monthYearLabel = new Label
        {
            BackgroundColor = Colors.Transparent,
            FontSize = 14,
            TextColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        TapGestureRecognizer tapGestureRecognizer = new();
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            OnMonthYearLabelClicked(s, e);
        };
        _monthYearLabel.GestureRecognizers.Add(tapGestureRecognizer);

        preNextButtonsGrid.Add(_monthYearLabel, 1, 0);

        _nextMonthButton = new Button
        {
            Text = ">",
            TextColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
            BackgroundColor = Colors.Transparent,
            Background = Colors.Transparent,
            BorderColor = Colors.Transparent,
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
            WidthRequest = 50,
            Margin = 0,
            IsEnabled = _options.Mode != PickerModes.Time  //Click skipping in time mode 
        };
        _nextMonthButton.Clicked += OnNextMonthButtonClicked;
        preNextButtonsGrid.Add(_nextMonthButton, 2, 0);

        _calendarGrid.Add(preNextButtonsGrid, 0, 1);


        #region days
        _daysGrid = new Grid
        {
            BackgroundColor = Colors.Transparent,
            ColumnSpacing = 0,
            RowSpacing = 0,
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        for (int col = 0; col <= 7; col++)
        {
            _daysGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
        for (int row = 0; row < 7; row++)
        {
            _daysGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        _calendarGrid.Add(_daysGrid, 0, 2);

        #endregion // days


        #region Time row
        if (_options.Mode != PickerModes.Date)
        {
            _hoursPicker = new Picker
            {
                BackgroundColor = Colors.Transparent,
                TextColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                TitleColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                FontSize = 14,
                HeightRequest = 40,
                ItemDisplayBinding = new Binding("Text"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };
            _hoursPicker.SelectedIndexChanged += OnHoursPickerIndexChanged;

            var hoursMinutesSeparatorLabel = new Label
            {
                Text = ":",
                BackgroundColor = Colors.Transparent,
                TextColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                FontSize = 14,
                Margin = new Thickness(5, 0),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            _minutesPicker = new Picker
            {
                BackgroundColor = Colors.Transparent,
                TextColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                TitleColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                FontSize = 14,
                HeightRequest = 40,
                ItemDisplayBinding = new Binding("Text"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };
            _minutesPicker.SelectedIndexChanged += OnMinutesPickerIndexChanged;

            _timeStackLayout = new StackLayout
            {
                Background = Colors.Transparent,
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                IsVisible = _options.Mode != PickerModes.Date
            };
            _timeStackLayout.Add(_hoursPicker);
            _timeStackLayout.Add(hoursMinutesSeparatorLabel);
            _timeStackLayout.Add(_minutesPicker);

            // am/pm picker
            if (_options.Is12HourFormat)
            {
                _amPmPicker = new Picker
                {
                    BackgroundColor = Colors.Transparent,
                    TextColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                    TitleColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                    FontSize = 14,
                    HeightRequest = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                _amPmPicker.SelectedIndexChanged += OnAmPmPickerIndexChanged;
                _timeStackLayout.Add(_amPmPicker);
            }

            _calendarGrid.Add(_timeStackLayout, 0, 3);
        }
        #endregion // Time row


        #region ToolButtons row
        Grid toolButtonsGrid = new()
        {
            Margin = 0,
            Padding = new Thickness(5, 0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            }
        };

        if (_options.ShowClearButton)
        {
            _clearButton = new Button
            {
                Text = !string.IsNullOrEmpty(_options.ClearButtonText) ? _options.ClearButtonText : "Clear",
                Style = _toolButtonsStyle,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = 0
            };
            _clearButton.Clicked += OnClearButtonClicked;
            toolButtonsGrid.Add(_clearButton, 0);
        }

        _cancelButton = new Button
        {
            Text = !string.IsNullOrEmpty(_options.CancelButtonText) ? _options.CancelButtonText : "Cancel",
            Style = _toolButtonsStyle,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = 0
        };
        _cancelButton.Clicked += OnCancelButtonClicked;
        toolButtonsGrid.Add(_cancelButton, 1);

        _okButton = new Button
        {
            Text = !string.IsNullOrEmpty(_options.OkButtonText) ? _options.OkButtonText : "Ok",
            Style = _toolButtonsStyle,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = 0
        };
        _okButton.Clicked += OnOkButtonClicked;
        toolButtonsGrid.Add(_okButton, 2);

        _calendarGrid.Add(toolButtonsGrid, 0, 4);
        #endregion //Tool buttons row

        await MainThreadHelper.SafeInvokeOnMainThreadAsync((Action)(() =>
        {
            this._mainGrid.Add((IView)this._calendarGrid);
        }));
    }

    private void OnMonthYearLabelClicked(object s, TappedEventArgs e)
    {
        if (_options.Mode == PickerModes.Time)
            return;

        if (_monthListGrid == null || !_calendarGrid.Children.Contains(_monthListGrid) || !_monthListGrid.IsVisible)
            ShowMonthListView();
        else
            HideMonthListView();
    }

    private void CreateMonthListGrid()
    {
        if (_monthListGrid != null)
            return;

        string[] months = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames;

        _monthListGrid = new Grid
        {
            BackgroundColor = Colors.Transparent,
            RowSpacing = 1,
            ColumnSpacing = 1,
            Margin = 5,
            Padding = 0,
            IsVisible = false,
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star }
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
            }
        };

        int monthNumber = 1;
        int row = 0;
        int col = 0;
        TapGestureRecognizer tapGestureRecognizer = new();
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            OnMonthNameLabelClicked(s, e);
        };

        foreach (string month in months)
        {
            if (string.IsNullOrEmpty(month))
                continue;

            Label monthLabel = new Label
            {
                Text = monthNumber.ToString("D2") + " " + month,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.HeadTruncation,
                TextColor = _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black)
            };
            monthLabel.GestureRecognizers.Add(tapGestureRecognizer);
            _monthListGrid.Add(monthLabel, col, row);
            monthNumber++;
            col++;
            if (col % 3 == 0)
            {
                col = 0;
                row++;
            }
        }
    }

    private void ShowMonthListView()
    {
        CreateMonthListGrid();
        MainThreadHelper.SafeBeginInvokeOnMainThread((Action)(() =>
          {
              if (_daysGrid != null)
                  _daysGrid.IsVisible = false;

              if (_monthListGrid != null)
                  _monthListGrid.IsVisible = true;

              if (_calendarGrid != null && !_calendarGrid.Children.Contains(_monthListGrid))
                  _calendarGrid.Add((IView)_monthListGrid, 0, 2);
          }));
    }

    private void HideMonthListView()
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread((Action)(() =>
      {
          if (_monthListGrid != null)
          {
              _monthListGrid.IsVisible = false;

              //if (_calendarGrid != null)
              //   _calendarGrid.Remove((IView)_monthListGrid);

              if (_daysGrid != null)
                  _daysGrid.IsVisible = true;
          }
      }));
    }
    private async void OnMonthNameLabelClicked(object s, TappedEventArgs e)
    {
        HideMonthListView();
        await SetCurrentDateAndRebuildCalendar(_currentDate.Year, Convert.ToInt16((s as Label).Text[..2]), _currentDate.Day);
    }
    private bool DayDisabled(int year, int month, int day)
    {
        return year < _minDate.Year
            || (year == _minDate.Year && month < _minDate.Month)
            || (year == _minDate.Year && month == _minDate.Month && day < _minDate.Day)
            || year > _maxDate.Year
            || (year == _maxDate.Year && month > _maxDate.Month)
            || (year == _maxDate.Year && month == _maxDate.Month && day > _maxDate.Day);
    }

    /// <summary>
    /// 0-23
    /// </summary>
    /// <param name="hour24Format"></param>
    /// <returns></returns>
    private static int ConvertTo12HourFormat(int hour24Format)
    {
        int hour12Format = hour24Format % 12;
        return (hour12Format == 0) ? 12 : hour12Format;
    }

    /// <summary>
    /// 1-12 am/pm
    /// </summary>
    /// <param name="hour12Format"></param>
    /// <returns></returns>
    private static int ConvertTo24HourFormat(int hour12Format, DateTime currentDate)
    {
        DateTime dateTime = DateTime.ParseExact($"{hour12Format} {currentDate:tt}", "h tt", CultureInfo.InvariantCulture);
        return dateTime.Hour;
    }

    private static int UpdateHourByAmPmOption(int currentHour, string amPmOption)
    {
        // Update the hour value based on the AM/PM option

        if (amPmOption.ToUpper() == "AM")
        {
            if (currentHour > 12)
            {
                currentHour -= 12;
            }
            else if (currentHour == 12)
            {
                currentHour = 0; // 12 AM is represented as 0 in 24-hour format
            }
        }

        if (amPmOption.ToUpper() == "PM")
        {

            if (currentHour <= 12)
            {
                currentHour += 12;
            }

            if (currentHour == 24)
            {
                currentHour = 12; // 12 PM is represented as 12 in 24-hour format
            }
        }

        return currentHour;
    }
}