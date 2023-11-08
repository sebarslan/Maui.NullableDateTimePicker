using System.Globalization;

namespace Maui.NullableDateTimePicker;

public partial class NullableDateTimePickerContent : ContentView
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
    private Style _otherMonthDayStyle;
    private Style _dayNamesStyle;
    private Style _toolButtonsStyle;
    private Style _weekNumberStyle;
    private Picker _hoursPicker;
    private Picker _minutesPicker;
    private StackLayout _timeStackLayout;
    private List<Button> _dayButtons;
    private Grid _monthListGrid;


    internal NullableDateTimePickerContent(INullableDateTimePickerOptions options)
    {
        options ??= new NullableDateTimePickerOptions();

        _options = options;
        _selectedDate = options.NullableDateTime;
        _currentDate = options.NullableDateTime ?? DateTime.Now;
        _minDate = options.MinDate ?? new DateTime(1900, 1, 1);
        _maxDate = options.MaxDate ?? new DateTime(DateTime.Now.Year + 100, 12, 31);

        InitializeComponent();

        if (_options.ActivityIndicatorColor != null)
            CalendarActivityIndicator.Color = _options.ActivityIndicatorColor;
    }

    internal void NullableDateTimePickerPopupOpened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        InitCalendar();
    }

    internal void InitCalendar()
    {
        Task.Run(async () =>
        {
            try
            {
                await InitContent();

                PopulatePickers();

                BuildCalendar();
            }
            catch (Exception ex)
            {
                Console.Write($"BuildCalendar-Error: {ex}");
            }
            finally
            {

            }
        });
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

    private void OnPreviousMonthButtonClicked(object sender, EventArgs e)
    {
        SetPreviousMonth();
    }

    internal void SetPreviousMonth()
    {
        var previousMonth = _currentDate.Month - 1;
        var currentYear = _currentDate.Year;
        if (previousMonth < 1)
        {
            previousMonth = 12;
            currentYear--;
        }
        SetCurrentDateAndRebuildCalendar(currentYear, previousMonth, _currentDate.Day);
    }

    private void OnNextMonthButtonClicked(object sender, EventArgs e)
    {
        SetNextMonth();
    }

    internal void SetNextMonth()
    {
        var nextMonth = _currentDate.Month + 1;
        var currentYear = _currentDate.Year;
        if (nextMonth > 12)
        {
            nextMonth = 1;
            currentYear++;
        }

        SetCurrentDateAndRebuildCalendar(currentYear, nextMonth, _currentDate.Day);
    }

    private void OnDayButtonTapped(object sender, EventArgs e)
    {
        if (_options.Mode == PickerModes.Time) //Click skipping in time mode 
            return;

        Button dayButton = sender as Button;
        UpdateCurrentDateAndControls(new DateTime(_currentDate.Year, _currentDate.Month, Convert.ToInt32(dayButton.Text), _currentDate.Hour, _currentDate.Minute, _currentDate.Second));
    }

    private void OnLastMonthDayButtonTapped(object sender, EventArgs e)
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
        SetCurrentDateAndRebuildCalendar(year, month, Convert.ToInt32(dayButton.Text));
    }

    private void OnNextMonthDayButtonTapped(object sender, EventArgs e)
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
        SetCurrentDateAndRebuildCalendar(year, month, Convert.ToInt32(dayButton.Text));
    }



    internal void SetCurrentDateAndRebuildCalendar(int year, int month, int day)
    {
        FixAndSetCurrentDate(year, month, day);
        BuildCalendar();
    }

    private void FixAndSetCurrentDate(int year, int month, int day)
    {
        if (month > 12)
            month = 12;

        if (month < 1)
            month = 1;

        if (month > _maxDate.Month)
            month = _maxDate.Month;

        if (month < _minDate.Month)
            month = _minDate.Month;


        var daysInMonth = DateTime.DaysInMonth(year, month);
        if (day > daysInMonth)
            day = daysInMonth;
        if (day < 1)
            day = 1;

        var currentDate = new DateTime(year, month, day, _currentDate.Hour, _currentDate.Minute, _currentDate.Second);
        if (currentDate < _minDate)
            _currentDate = _minDate;
        else
            _currentDate = currentDate;
    }

    private void BuildCalendar()
    {
        Task.Run(() =>
        {
            MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
       {
           CalendarActivityIndicator.IsVisible = true;
           CalendarActivityIndicator.IsRunning = true;

           if (!_options.ShowWeekNumbers)
               _daysGrid.ColumnDefinitions[0].Width = 0;

           if (_currentDate > _minDate)
               _previousMonthButton.IsVisible = true;
           else
               _previousMonthButton.IsVisible = false;

           if (_currentDate <= _maxDate)
               _nextMonthButton.IsVisible = true;
           else
               _nextMonthButton.IsVisible = false;
       });

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

            MainThreadHelper.SafeBeginInvokeOnMainThread(async () =>
            {
                _daysGrid.Clear();
                _daysGrid.Children?.Clear();


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
                    dayButton.IsEnabled = true;

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
                    var lastMonthDate = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddMonths(-1);
                    int daysInLastMonth = DateTime.DaysInMonth(lastMonthDate.Year, lastMonthDate.Month);
                    // Fill in days for the last month
                    int daysNeededFromLastMonth = ((int)firstDayOfMonth - firstDayOfWekkIndex + 7) % 7;
                    for (int i = daysNeededFromLastMonth - 1; i >= 0; i--)
                    {
                        Button lastMonthDayButton = new()
                        {
                            Text = (daysInLastMonth - i).ToString(),
                            Style = _otherMonthDayStyle
                        };
                        lastMonthDayButton.Clicked += OnLastMonthDayButtonTapped;

                        _daysGrid.Add(lastMonthDayButton, daysNeededFromLastMonth - i, 1); //the first column of the grid is for the week numbers
                    }


                    DateTime firstDayOfNextMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddMonths(1);
                    int daysInNextMonth = DateTime.DaysInMonth(firstDayOfNextMonth.Year, firstDayOfNextMonth.Month);
                    int lastWeekLastDayIndex = ((int)firstDayOfNextMonth.DayOfWeek - 1 + 7) % 7;

                    for (int i = 0; i < 6 * 7 - daysInMonth - daysNeededFromLastMonth; i++)
                    {
                        Button nextMonthDayButton = new()
                        {
                            Text = (i + 1).ToString(),
                            Style = _otherMonthDayStyle
                        };
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

                CalendarActivityIndicator.IsVisible = false;
                CalendarActivityIndicator.IsRunning = false;
            });
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
        RemoveMonthListView();

        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
       {
           _yearsPicker.SelectedItem = _currentDate.Year;
           if (_options.Mode != PickerModes.Date)
           {
               _hoursPicker.SelectedItem = string.Format("{0:D2}", _currentDate.Hour);
               _minutesPicker.SelectedItem = string.Format("{0:D2}", _currentDate.Minute);
           }
           _monthYearLabel.Text = _currentDate.ToString("MMMM yyyy");
           _selectedDateLabel.Text = date.HasValue ? date.Value.ToString("ddd, MMM d") : "No Date Selected";
           SetCurrentDayStyle(_currentDate.Day.ToString());
       });
    }

    Button lastClickedDayButton = null;
    private void SetCurrentDayStyle(string day)
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
        {
            if (lastClickedDayButton != null)
                lastClickedDayButton.Style = _dayStyle as Style;

            foreach (var child in _dayButtons)
            {
                if (child is Button button)
                {
                    if (button.Text == day)
                    {
                        button.Style = _selectedDayStyle as Style;
                        lastClickedDayButton = button;
                        break;
                    }
                }
            }
        });
    }

    private void PopulatePickers()
    {
        List<int> years = new();
        int minYear = _minDate.Year;
        int maxYear = _maxDate.Year;
        for (int y = minYear; y <= maxYear; y++)
        {
            years.Add(y);
        }

        List<string> hours = null;
        List<string> minutes = null;
        if (_options.Mode != PickerModes.Date)
        {
            hours = new();

            for (int h = 1; h < 24; h++)
            {
                hours.Add(string.Format("{0:D2}", h));
            }
            hours.Add("00");


            minutes = new();
            for (int m = 1; m < 60; m++)
            {
                minutes.Add(string.Format("{0:D2}", m));
            }
            minutes.Add("00");
        }

        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
        {
            _yearsPicker.ItemsSource = years;
            if (_hoursPicker != null)
                _hoursPicker.ItemsSource = hours;
            if (_minutesPicker != null)
                _minutesPicker.ItemsSource = minutes;
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

    private void OnYearsPickerIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = ((Picker)sender).SelectedItem;
        if (selectedItem == null)
            return;

        SetYear((int)selectedItem);
    }

    internal void SetYear(int year)
    {
        if (year == _currentDate.Year)
            return;

        if (year < _minDate.Year)
            year = _minDate.Year;
        if (year > _maxDate.Year)
            year = _maxDate.Year;

        SetCurrentDateAndRebuildCalendar(year, _currentDate.Month, _currentDate.Day);
    }

    private void OnHoursPickerIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = ((Picker)sender).SelectedItem;
        if (selectedItem == null)
            return;

        if (int.TryParse((string)selectedItem, out int selectedHour))
            SetHour(selectedHour);
    }

    internal void SetHour(int hour)
    {
        if (hour < 0 || hour > 23 || _currentDate.Minute == hour)
            return;

        UpdateCurrentDateAndControls(new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, hour, _currentDate.Minute, _currentDate.Second));
    }

    private void OnMinutesPickerIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = ((Picker)sender).SelectedItem;
        if (selectedItem == null)
            return;

        if (int.TryParse((string)selectedItem, out int selectedMinute))
            SetMinute(selectedMinute);
    }

    internal void SetMinute(int minute)
    {
        if (minute < 0 || minute > 59 || _currentDate.Minute == minute)
            return;

        UpdateCurrentDateAndControls(new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, _currentDate.Hour, minute, _currentDate.Second));
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
                    Style = _dayStyle
                };
                button.Clicked += OnDayButtonTapped;
                _dayButtons.Add(button);
            }
        }

        _calendarGrid = new Grid
        {
            BackgroundColor = _options.BackgroundColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Color.FromRgba("#434343") : Colors.White),
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
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
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
                TextColor = _options.ForeColor ?? _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            _minutesPicker = new Picker
            {
                BackgroundColor = Colors.Transparent,
                TextColor = _options.ForeColor ?? _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                TitleColor = _options.ForeColor ?? _options.ForeColor ?? (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                FontSize = 14,
                HeightRequest = 40,
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
            this.MainGrid.Add((IView)this._calendarGrid);
        }));
    }

    private void OnMonthYearLabelClicked(object s, TappedEventArgs e)
    {
        CreateMonthListGrid();
        AddMonthListView();
    }

    private Grid CreateMonthListGrid()
    {
        if (_monthListGrid != null)
            return _monthListGrid;

        string[] months = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames;

        _monthListGrid = new Grid
        {
            BackgroundColor = Colors.Transparent,
            RowSpacing = 1,
            ColumnSpacing = 1,
            Margin = 5,
            Padding = 0,
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
        return _monthListGrid;
    }

    private void AddMonthListView()
    {
        RemoveMonthListView();
        MainThreadHelper.SafeBeginInvokeOnMainThread((Action)(() =>
          {
              if (_daysGrid != null)
                  _daysGrid.IsVisible = false;

              if (_monthListGrid != null)
                  _monthListGrid.IsVisible = true;

              if (_calendarGrid != null)
                  _calendarGrid.Add((IView)_monthListGrid, 0, 2);
          }));
    }

    private void RemoveMonthListView()
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread((Action)(() =>
      {
          if (_monthListGrid != null)
              _monthListGrid.IsVisible = false;

          if (_calendarGrid != null)
              _calendarGrid.Remove((IView)_monthListGrid);

          if (_daysGrid != null)
              _daysGrid.IsVisible = true;
      }));
    }
    private void OnMonthNameLabelClicked(object s, TappedEventArgs e)
    {
        SetCurrentDateAndRebuildCalendar(_currentDate.Year, Convert.ToInt16((s as Label).Text.Substring(0, 2)), _currentDate.Day);
    }
}