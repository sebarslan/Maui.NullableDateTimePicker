using Maui.NullableDateTimePicker.Controls;
using Maui.NullableDateTimePicker.Helpers;
using Maui.NullableDateTimePicker.Models;
using System.Globalization;

namespace Maui.NullableDateTimePicker;

internal class NullableDateTimePickerContent : ContentView
{
    internal event EventHandler<EventArgs> OkButtonClicked;
    internal event EventHandler<EventArgs> ClearButtonClicked;
    internal event EventHandler<EventArgs> CancelButtonClicked;
    private DateTime? _selectedDateTime;
    private DateTime _currentDate;
    readonly DateTime _minDate;
    readonly DateTime _maxDate;
    readonly INullableDateTimePickerOptions _options;
    private ActivityIndicator _activityIndicator;
    private Grid _popupContentGrid;
    private Grid _mainContentArea;
    private Button _okButton;
    private Button _cancelButton;
    private Button _clearButton;
    private List<Button> _dayButtons;
    private Grid _daysGrid;
    Grid _preNextButtonsGrid;
    private SelectList _yearsSelectList;
    private SelectList _monthsSelectList;
    private Label _selectedDateLabel;
    private Label _selectedYearLabel;
    private Label _monthYearLabel;
    private Button _previousMonthButton;
    private Button _nextMonthButton;
    private Style _dayStyle;
    private Style _selectedDayStyle;
    private Style _disabledDayStyle;
    private Style _otherMonthDayStyle;
    private Style _dayNamesStyle;
    private Style _toolButtonsStyle;
    private Style _weekNumberStyle;
    private Picker _hoursPicker;
    private Picker _minutesPicker;
    private Picker _amPmPicker;
    private StackLayout _timeStackLayout;
    private ScrollView _scrollView;
    private List<YearModel> _years = null;
    private List<PickerItem> _hours = null;
    private List<PickerItem> _minutes = null;
    private List<string> _amPmList = null;
    private ClockView _nullableDateTimePickerClockView;
    private bool isClockDisplaying;

    internal NullableDateTimePickerContent(INullableDateTimePickerOptions options)
    {
        base.Padding = 0;
        base.Margin = 0;
        options ??= new NullableDateTimePickerOptions();

        _options = options;
        _selectedDateTime = options.SelectedDateTime;
        _currentDate = options.SelectedDateTime ?? DateTime.Now;
        _minDate = options.MinDate ?? new DateTime(1900, 1, 1);
        _maxDate = options.MaxDate ?? new DateTime(DateTime.Now.Year + 100, 12, 31);

        _popupContentGrid = new Grid
        {
            AutomationId = options.AutomationId + "_PopupContentGrid",
            Margin = new Thickness(0),
            Padding = new Thickness(0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.Transparent
        };

        _activityIndicator = new ActivityIndicator
        {
            IsVisible = false,
            IsRunning = false,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        if (_options.ActivityIndicatorColor != null)
        {
            _activityIndicator.Color = _options.ActivityIndicatorColor;
        }
        else if (_options.ActivityIndicatorThemeColor != null)
        {
            _activityIndicator.SetBinding(ActivityIndicator.ColorProperty, _options.ActivityIndicatorThemeColor.GetBinding());
        }

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
                    _popupContentGrid,
                    _activityIndicator
                }
            }
        };

        Content = _scrollView;
    }


    internal async void NullableDateTimePickerPopupOpened(object sender, EventArgs e)
    {
        try
        {
            await InitPopupContent();
        }
        catch (Exception ex)
        {
            Console.Write($"BuildCalendar-Error: {ex}");
        }
    }


    internal DateTime? SelectedDateTime
    {
        get { return _selectedDateTime; }
        private set
        {
            var date = value;
            _selectedDateTime = date;
        }
    }

    private async Task InitPopupContent()
    {
        _mainContentArea = new Grid
        {
            AutomationId = $"{_options.AutomationId}_BodyGrid",
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            Padding = _options.PopupPadding,
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

        if (_options.BodyBackgroundColor != null)
        {
            _mainContentArea.BackgroundColor = _options.BodyBackgroundColor;
        }
        else if (_options.BodyBackgroundThemeColor != null)
        {
            _mainContentArea.SetBinding(Grid.BackgroundColorProperty, _options.BodyBackgroundThemeColor.GetBinding());
        }

        #region header

        Grid headerGrid = new()
        {
            AutomationId = $"{_options.AutomationId}_HeaderGrid",
            Padding = new Thickness(10, 0, 10, 0),
            Margin = new Thickness(-_options.PopupPadding.Left, -_options.PopupPadding.Top, -_options.PopupPadding.Right, 0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            }
        };

        if (_options.HeaderBackgroundColor != null)
        {
            headerGrid.BackgroundColor = _options.HeaderBackgroundColor;
        }
        else if (_options.HeaderBackgroundThemeColor != null)
        {
            headerGrid.SetBinding(Grid.BackgroundColorProperty, _options.HeaderBackgroundThemeColor.GetBinding());
        }

        _selectedYearLabel = new Label
        {
            AutomationId = _options.AutomationId + "_CalendarSelectedYearLabel",
            HeightRequest = 35,
            Padding = 0,
            FontSize = 25,
            BackgroundColor = Colors.Transparent,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Start,
            HorizontalOptions = LayoutOptions.Start,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.TailTruncation,
            IsEnabled = _options.Mode != PickerModes.Time,  //Click skipping in time mode
            IsVisible = _options.Mode != PickerModes.Time
        };

        if (_options.HeaderForeColor != null)
        {
            _selectedYearLabel.TextColor = _options.HeaderForeColor;
        }
        else if (_options.HeaderForeThemeColor != null)
        {
            _selectedYearLabel.SetBinding(Label.TextColorProperty, _options.HeaderForeThemeColor.GetBinding());
        }

        var tapGestureRecognizerForYearLabel = new TapGestureRecognizer();
        tapGestureRecognizerForYearLabel.Tapped += OnYearLabelClicked;

        _selectedYearLabel.GestureRecognizers.Add(tapGestureRecognizerForYearLabel);

        _selectedDateLabel = new Label
        {
            AutomationId = _options.AutomationId + "_CalendarSelectedDateLabel",
            HeightRequest = 35,
            Padding = 0,
            FontSize = 20,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Start,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Start,
            LineBreakMode = LineBreakMode.TailTruncation
        };

        if (_options.HeaderForeColor != null)
        {
            _selectedDateLabel.TextColor = _options.HeaderForeColor;
        }
        else if (_options.HeaderForeThemeColor != null)
        {
            _selectedDateLabel.SetBinding(Label.TextColorProperty, _options.HeaderForeThemeColor.GetBinding());
        }

        headerGrid.Add(_selectedYearLabel);
        headerGrid.SetRow(_selectedYearLabel, 0);
        headerGrid.Add(_selectedDateLabel);
        headerGrid.SetRow(_selectedDateLabel, 1);
        _mainContentArea.Add(headerGrid);

        #endregion //header

        #region Time row
        if (_options.Mode != PickerModes.Date)
        {
            await InitTimePickerBlock();
        }
        #endregion // Time row

        #region Calendar row
        if (_options.Mode == PickerModes.Time)
        {
            await InitClockBlock();
        }
        else
        {
            InitCalendarBlock();
            await SetCurrentDateAndRebuildCalendar(_currentDate.Year, _currentDate.Month, _currentDate.Day);
        }
        #endregion //Calendar row


        #region ToolButtons row
        InitToolButtonsBlock();
        #endregion //Tool buttons row

        await MainThreadHelper.SafeInvokeOnMainThreadAsync(() => this._popupContentGrid.Add(_mainContentArea));
    }

    private void InitCalendarBlock()
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
        #endregion // Styles end

        if (_dayButtons == null)
        {
            _dayButtons = new List<Button>();
            for (int day = 1; day <= 31; day++)
            {
                Button button = new()
                {
                    AutomationId = $"{_options.AutomationId}_CalendarCurrentMonthDayButton_{day}",
                    Text = day.ToString(),
                    Style = _dayStyle,
                    IsEnabled = true
                };
                button.Clicked += OnDayButtonTapped;
                _dayButtons.Add(button);
            }
        }

        _preNextButtonsGrid = new()
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
            AutomationId = _options.AutomationId + "_CalendarPreviousMonthButton",
            Text = "<",
            BackgroundColor = Colors.Transparent,
            Background = Colors.Transparent,
            BorderColor = Colors.Transparent,
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
            WidthRequest = 50,
            Margin = 0,
            IsEnabled = _options.Mode != PickerModes.Time  //Click skipping in time mode 
        };

        if (_options.ForeColor != null)
        {
            _previousMonthButton.TextColor = _options.ForeColor;
        }
        else if (_options.ForeColorThemeColor != null)
        {
            _previousMonthButton.SetBinding(Button.TextColorProperty, _options.ForeColorThemeColor.GetBinding());
        }

        _previousMonthButton.Clicked += OnPreviousMonthButtonClicked;
        _preNextButtonsGrid.Add(_previousMonthButton, 0, 0);

        _monthYearLabel = new Label
        {
            AutomationId = _options.AutomationId + "_CalendarMontYearLabel",
            BackgroundColor = Colors.Transparent,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        if (_options.ForeColor != null)
        {
            _monthYearLabel.TextColor = _options.ForeColor;
        }
        else if (_options.ForeColorThemeColor != null)
        {
            _monthYearLabel.SetBinding(Button.TextColorProperty, _options.ForeColorThemeColor.GetBinding());
        }

        TapGestureRecognizer tapGestureRecognizer = new();
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            OnMonthYearLabelClicked(s, e);
        };
        _monthYearLabel.GestureRecognizers.Add(tapGestureRecognizer);

        _preNextButtonsGrid.Add(_monthYearLabel, 1, 0);

        _nextMonthButton = new Button
        {
            AutomationId = _options.AutomationId + "_CalendarNextMonthButton",
            Text = ">",
            BackgroundColor = Colors.Transparent,
            Background = Colors.Transparent,
            BorderColor = Colors.Transparent,
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
            WidthRequest = 50,
            Margin = 0,
            IsEnabled = _options.Mode != PickerModes.Time  //Click skipping in time mode 
        };


        if (_options.ForeColor != null)
        {
            _nextMonthButton.TextColor = _options.ForeColor;
        }
        else if (_options.ForeColorThemeColor != null)
        {
            _nextMonthButton.SetBinding(Button.TextColorProperty, _options.ForeColorThemeColor.GetBinding());
        }

        _nextMonthButton.Clicked += OnNextMonthButtonClicked;
        _preNextButtonsGrid.Add(_nextMonthButton, 2, 0);

        _mainContentArea.Add(_preNextButtonsGrid, 0, 1);


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

        _mainContentArea.Add(_daysGrid, 0, 2);

        #endregion // days
    }

    private async Task InitClockBlock()
    {
        await MainThreadHelper.SafeInvokeOnMainThreadAsync(() =>
        {
            try
            {
                _activityIndicator.IsVisible = true;
                _activityIndicator.IsRunning = true;

                _nullableDateTimePickerClockView = new ClockView(_options);
                _nullableDateTimePickerClockView.SelectedTime = TimeOnly.FromDateTime(_currentDate);
                _nullableDateTimePickerClockView.TimeChanged += (s, e) =>
                {
                    UpdateCurrentDateAndControls(SelectedDateTime?.Date.AddHours(e.NewTime.Hour).AddMinutes(e.NewTime.Minute).AddSeconds(e.NewTime.Second));
                };

                _mainContentArea.Add(_nullableDateTimePickerClockView, 0, 1);
                _mainContentArea.SetRowSpan(_nullableDateTimePickerClockView, 2);

                UpdateCurrentDateAndControls(_currentDate);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _activityIndicator.IsVisible = false;
                _activityIndicator.IsRunning = false;
            }
        });
    }

    private async Task InitTimePickerBlock()
    {
        _hoursPicker = new Picker
        {
            AutomationId = _options.AutomationId + "_CalendarHoursPicker",
            BackgroundColor = Colors.Transparent,
            FontSize = 14,
            HeightRequest = 40,
            ItemDisplayBinding = new Binding("Text"),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center
        };

        if (_options.ForeColor != null)
        {
            _hoursPicker.TextColor = _options.ForeColor;
            _hoursPicker.TitleColor = _options.ForeColor;
        }
        else if (_options.ForeColorThemeColor != null)
        {
            _hoursPicker.SetBinding(Picker.TextColorProperty, _options.ForeColorThemeColor.GetBinding());
            _hoursPicker.SetBinding(Picker.TitleColorProperty, _options.ForeColorThemeColor.GetBinding());
        }

        _hoursPicker.SelectedIndexChanged += OnHoursPickerIndexChanged;

        var hoursMinutesSeparatorLabel = new Label
        {
            Text = ":",
            BackgroundColor = Colors.Transparent,
            FontSize = 14,
            Margin = new Thickness(5, 0),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        if (_options.ForeColor != null)
        {
            hoursMinutesSeparatorLabel.TextColor = _options.ForeColor;
        }
        else if (_options.ForeColorThemeColor != null)
        {
            hoursMinutesSeparatorLabel.SetBinding(Picker.TextColorProperty, _options.ForeColorThemeColor.GetBinding());
        }

        _minutesPicker = new Picker
        {
            AutomationId = _options.AutomationId + "_CalendarMinutesPicker",
            BackgroundColor = Colors.Transparent,
            FontSize = 14,
            HeightRequest = 40,
            ItemDisplayBinding = new Binding("Text"),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center
        };

        if (_options.ForeColor != null)
        {
            _minutesPicker.TextColor = _options.ForeColor;
            _minutesPicker.TitleColor = _options.ForeColor;
        }
        else if (_options.ForeColorThemeColor != null)
        {
            _minutesPicker.SetBinding(Picker.TextColorProperty, _options.ForeColorThemeColor.GetBinding());
            _minutesPicker.SetBinding(Picker.TitleColorProperty, _options.ForeColorThemeColor.GetBinding());
        }

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
                AutomationId = _options.AutomationId + "_CalendarAmPmPicker",
                BackgroundColor = Colors.Transparent,
                FontSize = 14,
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            if (_options.ForeColor != null)
            {
                _amPmPicker.TextColor = _options.ForeColor;
                _amPmPicker.TitleColor = _options.ForeColor;
            }
            else if (_options.ForeColorThemeColor != null)
            {
                _amPmPicker.SetBinding(Picker.TextColorProperty, _options.ForeColorThemeColor.GetBinding());
                _amPmPicker.SetBinding(Picker.TitleColorProperty, _options.ForeColorThemeColor.GetBinding());
            }

            _amPmPicker.SelectedIndexChanged += OnAmPmPickerIndexChanged;
            _timeStackLayout.Add(_amPmPicker);
        }

        if (_options.Mode == PickerModes.DateTime)
        {
            var toggleCalendarClockImageButton = new ImageButton
            {
                Source = Utilities.GetImageSource("toggle_calendar_clock.png"),
                Margin = new Thickness(10, 0, 0, 0),
                Padding = 0,
                Aspect = Aspect.AspectFit,
                BackgroundColor = Colors.Transparent,
                WidthRequest = 14,
                HeightRequest = 14,
                MaximumHeightRequest = 14,
                MaximumWidthRequest = 14,
                MinimumHeightRequest = 14,
                MinimumWidthRequest = 14
            };
            toggleCalendarClockImageButton.Clicked += (s, e) =>
            {
                ToggleCalendarClock();
            };

            _timeStackLayout.Add(toggleCalendarClockImageButton);
        }



        // Hours and Minutes Picker

        _hours = new();

        int startHour = _options.Is12HourFormat ? 1 : 0;
        int maxHour = _options.Is12HourFormat ? 12 : 23;
        for (int h = startHour; h <= maxHour; h++)
        {
            string hourText = h.ToString("00");

            _hours.Add(new PickerItem { Text = hourText, Value = h });
        }

        _minutes = new();
        for (int m = 0; m < 60; m++)
        {
            _minutes.Add(new PickerItem { Text = m.ToString("00"), Value = m });
        }

        if (_options.Is12HourFormat)
        {
            _amPmList = new()
                {
                    "AM",
                    "PM"
                };
        }

        await MainThreadHelper.SafeInvokeOnMainThreadAsync(() =>
        {
            // _hours
            if (_hoursPicker != null)
            {
                _hoursPicker.ItemsSource = _hours;
            }

            // _minutes
            if (_minutesPicker != null)
            {
                _minutesPicker.ItemsSource = _minutes;
            }

            // am/pm
            if (_options.Is12HourFormat && _amPmPicker != null)
            {
                _amPmPicker.ItemsSource = _amPmList;
            }

            _mainContentArea.Add(_timeStackLayout, 0, 3);
        });

    }

    private void InitToolButtonsBlock()
    {
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

        Grid toolButtonsGrid = new()
        {
            AutomationId = _options.AutomationId + "_FooterGrid",
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
                AutomationId = _options.AutomationId + "_ClearButton",
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
            AutomationId = _options.AutomationId + "_CancelButton",
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
            AutomationId = _options.AutomationId + "_OkButton",
            Text = !string.IsNullOrEmpty(_options.OkButtonText) ? _options.OkButtonText : "Ok",
            Style = _toolButtonsStyle,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = 0
        };
        _okButton.Clicked += OnOkButtonClicked;
        toolButtonsGrid.Add(_okButton, 2);

        _mainContentArea.Add(toolButtonsGrid, 0, 4);
    }

    private async void OnPreviousMonthButtonClicked(object sender, EventArgs e)
    {
        await SetPreviousMonth();
    }

    internal async Task SetPreviousMonth()
    {
        var previousMonth = _currentDate.Month;
        var currentYear = _currentDate.Year;

        if (_monthsSelectList?.IsVisible == true)
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
        if (_monthsSelectList?.IsVisible == true)
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
        if (_options.Mode == PickerModes.Time)
            return;

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
                        Style = _options.DayNamesStyle ?? _dayNamesStyle
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
                            AutomationId = $"{_options.AutomationId}_CalendarLastMonthDayButton_{lastMonthDate.Year}_{lastMonthDate.Month}_{dayInLastMonth}",
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
                            AutomationId = $"{_options.AutomationId}_CalendarNextMonthDayButton_{nextMonthDate.Year}_{nextMonthDate.Month}_{dayInNextMonth}",
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _activityIndicator.IsVisible = false;
                _activityIndicator.IsRunning = false;
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
        //Console.Write($"UpdateCurrentDate: {_currentDate}");

        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
        {
            if (_yearsSelectList != null)
                _yearsSelectList.SelectedIndex = _years?.FindIndex(y => y.Year == _currentDate.Year) ?? -1;

            if (_monthsSelectList != null)
                _monthsSelectList.SelectedIndex = _currentDate.Month - 1;

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
                    _amPmPicker.SelectedItem = _amPmList?.FirstOrDefault(x => x == _currentDate.ToString("tt", CultureInfo.InvariantCulture));
            }

            if (_monthYearLabel != null)
                _monthYearLabel.Text = _currentDate.ToString("MMMM yyyy");

            string selectedDateText = string.Empty;
            //if (_currentDate != null)
            //{
            if (_options.Mode == PickerModes.DateTime)
            {
                selectedDateText = _currentDate.ToString($"ddd, MMM d, {(_options.Is12HourFormat ? CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern : "HH:mm")}");
            }
            else if (_options.Mode == PickerModes.Time)
            {
                selectedDateText = _currentDate.ToString(_options.Is12HourFormat ? CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern : "HH:mm");
            }
            else
            {
                selectedDateText = _currentDate.ToString("ddd, MMM d");
            }
            //}
            //else
            //{
            //    selectedDateText = "No Date Selected";
            //}

            _selectedDateLabel.Text = selectedDateText;
            _selectedYearLabel.Text = _currentDate.Year.ToString() ?? "";

            SetCurrentDayStyle(_currentDate.Day.ToString());
        });
    }

    Button lastClickedDayButton = null;
    private void SetCurrentDayStyle(string day)
    {
        if (_options.Mode == PickerModes.Time)
            return;

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



    internal void OnClearButtonClicked(object sender, EventArgs e)
    {
        SelectedDateTime = null;
        ClearButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    internal void OnCancelButtonClicked(object sender, EventArgs e)
    {
        CancelButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    internal void OnOkButtonClicked(object sender, EventArgs e)
    {
        SelectedDateTime = _currentDate;
        OkButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    private async void OnYearsPickerIndexChanged(object sender, EventArgs e)
    {
        HideYearsSelectList();

        var yearSelectList = (SelectList)sender;
        if (yearSelectList.SelectedItem is not YearModel selectedYear)
            return;

        await SetYear(selectedYear.Year);
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
        var currentDate = new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, hour, _currentDate.Minute, _currentDate.Second);
        if (_nullableDateTimePickerClockView != null)
            _nullableDateTimePickerClockView.SelectedTime = TimeOnly.FromDateTime(currentDate);
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
        var currentDate = new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, _currentDate.Hour, minute, _currentDate.Second);
        if (_nullableDateTimePickerClockView != null)
            _nullableDateTimePickerClockView.SelectedTime = TimeOnly.FromDateTime(currentDate);
        UpdateCurrentDateAndControls(currentDate);
    }

    private void OnAmPmPickerIndexChanged(object sender, EventArgs e)
    {
        var selectedAmPmItem = ((Picker)sender).SelectedItem as string;
        if (string.IsNullOrEmpty(selectedAmPmItem))
            return;

        SetHour(UpdateHourByAmPmOption(_currentDate.Hour, selectedAmPmItem));
    }

    private void OnYearLabelClicked(object sender, EventArgs e)
    {
        if (_options.Mode == PickerModes.Time)
            return;

        if (!_mainContentArea.Children.Contains(_yearsSelectList) || !_yearsSelectList.IsVisible)
            ShowYearsSelectList();
        else
            HideYearsSelectList();
    }

    private void OnMonthYearLabelClicked(object s, TappedEventArgs e)
    {
        if (_options.Mode == PickerModes.Time)
            return;

        if (_monthsSelectList == null || !_mainContentArea.Children.Contains(_monthsSelectList) || !_monthsSelectList.IsVisible)
            ShowMonthListView();
        else
            HideMonthListView();
    }

    private void CreateYearsSelectList()
    {
        if (_yearsSelectList != null)
            return;

        // Years picker
        _years = new();
        int minYear = _minDate.Year;
        int maxYear = _maxDate.Year;
        for (int y = minYear; y <= maxYear; y++)
        {
            _years.Add(new YearModel(y));
        }
        var itemTextColor = Colors.Black;
        var dayTextColorSetter = _dayStyle.Setters.FirstOrDefault(s => s.Property == Button.TextColorProperty);

        if (dayTextColorSetter != null)
        {
            itemTextColor = (Color)dayTextColorSetter.Value;
        }
        var itemBackgroundColor = Colors.White;
        var itemBackgroundColorSetter = _dayStyle.Setters.FirstOrDefault(s => s.Property == Button.BackgroundColorProperty);

        if (itemBackgroundColorSetter != null)
        {
            itemBackgroundColor = (Color)itemBackgroundColorSetter.Value; // Colors.Transparent döner
        }
        var selectedItemBackgroundColor = Colors.White;
        var selectedItemBackgroundColorSetter = _selectedDayStyle.Setters.FirstOrDefault(s => s.Property == Button.BackgroundColorProperty);
        if (selectedItemBackgroundColorSetter != null)
        {
            selectedItemBackgroundColor = (Color)itemBackgroundColorSetter.Value; // Colors.Transparent döner
        }
        _yearsSelectList = new SelectList
        {
            AutomationId = _options.AutomationId + "_CalendarYearsSelectList",
            Margin = new Thickness(5, 0),
            ItemTextColor = GetColorFromStyle(_dayStyle, Button.TextColorProperty, Colors.Black),
            SelectedItemTextColor = GetColorFromStyle(_selectedDayStyle, Button.TextColorProperty, Colors.Black),
            ItemBackgroundColor = GetColorFromStyle(_dayStyle, Button.BackgroundColorProperty, Colors.White),
            SelectedItemBackgroundColor = GetColorFromStyle(_selectedDayStyle, Button.BackgroundColorProperty, Colors.LightBlue),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ItemsSource = _years,
            SelectedIndex = _years?.FindIndex(y => y.Year == _currentDate.Year) ?? -1,
            IsVisible = false,
            ItemDisplayBinding = "Year"
        };


        if (_options.BodyBackgroundColor != null)
        {
            _yearsSelectList.BackgroundColor = _options.BodyBackgroundColor;
        }
        else if (_options.BodyBackgroundThemeColor != null)
        {
            _yearsSelectList.SetBinding(SelectList.BackgroundColorProperty, _options.BodyBackgroundThemeColor.GetBinding());
        }

        _yearsSelectList.SelectedIndexChanged += OnYearsPickerIndexChanged;
        _yearsSelectList.Closed += (s, e) =>
        {
            HideYearsSelectList();
        };
    }

    private void CreateMonthsSelectList()
    {
        if (_monthsSelectList != null)
            return;

        string[] months = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames;

        _monthsSelectList = new SelectList
        {
            AutomationId = _options.AutomationId + "_CalendarMonthsSelectList",
            ItemTextColor = GetColorFromStyle(_dayStyle, Button.TextColorProperty, Colors.Black),
            SelectedItemTextColor = GetColorFromStyle(_selectedDayStyle, Button.TextColorProperty, Colors.Black),
            ItemBackgroundColor = GetColorFromStyle(_dayStyle, Button.BackgroundColorProperty, Colors.White),
            SelectedItemBackgroundColor = GetColorFromStyle(_selectedDayStyle, Button.BackgroundColorProperty, Colors.LightBlue),
            Margin = 5,
            Padding = 0,
            IsVisible = false,
            ItemsSource = Enumerable.Range(0, 12).Select(m => new MonthModel { Name = months[m], Number = m + 1 }).ToList(),
            ItemDisplayBinding = "Name",
            SelectedIndex = _currentDate.Month - 1
        };

        if (_options.BodyBackgroundColor != null)
        {
            _monthsSelectList.BackgroundColor = _options.BodyBackgroundColor;
        }
        else if (_options.BodyBackgroundThemeColor != null)
        {
            _monthsSelectList.SetBinding(SelectList.BackgroundColorProperty, _options.BodyBackgroundThemeColor.GetBinding());
        }

        _monthsSelectList.SelectedIndexChanged += OnMonthsPickerIndexChanged;
        _monthsSelectList.Closed += (s, e) =>
        {
            HideMonthListView();
        };
    }

    private void ToggleCalendarClock()
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread(async () =>
        {
            HideYearsSelectList();
            HideMonthListView();
            isClockDisplaying = !isClockDisplaying;
            if (_nullableDateTimePickerClockView == null)
                await InitClockBlock();

            if (isClockDisplaying)
            {
                _preNextButtonsGrid.IsVisible = false;
                _daysGrid.IsVisible = false;
                _nullableDateTimePickerClockView.IsVisible = true;
            }
            else
            {
                _nullableDateTimePickerClockView.IsVisible = false;
                _preNextButtonsGrid.IsVisible = true;
                _daysGrid.IsVisible = true;
            }
        });
    }

    private void ShowYearsSelectList()
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
        {
            CreateYearsSelectList();
            if (_daysGrid != null)
                _daysGrid.IsVisible = false;

            if (_daysGrid != null)
                _daysGrid.IsVisible = false;

            if (_monthsSelectList != null)
                _monthsSelectList.IsVisible = false;

            if (_mainContentArea != null && _yearsSelectList != null)
            {
                if (!_mainContentArea.Children.Contains(_yearsSelectList))
                {
                    _mainContentArea.Add(_yearsSelectList, 0, 1);
                    _mainContentArea.SetRowSpan(_yearsSelectList, 4);
                }
                _yearsSelectList.IsVisible = true;
            }
        });
    }

    private void HideYearsSelectList()
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
        {
            if (_yearsSelectList != null)
            {
                _yearsSelectList.IsVisible = false;

                if (_daysGrid != null)
                    _daysGrid.IsVisible = true;
            }
        });
    }

    private void ShowMonthListView()
    {
        CreateMonthsSelectList();
        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
        {
            if (_daysGrid != null)
                _daysGrid.IsVisible = false;

            if (_yearsSelectList != null)
                _yearsSelectList.IsVisible = false;

            if (_nullableDateTimePickerClockView != null)
                _nullableDateTimePickerClockView.IsVisible = false;

            if (_mainContentArea != null && _monthsSelectList != null)
            {
                _monthsSelectList.IsVisible = true;
                if (!_mainContentArea.Children.Contains(_monthsSelectList))
                {
                    _mainContentArea.Add(_monthsSelectList, 0, 2);
                    _mainContentArea.SetRowSpan(_monthsSelectList, 2);
                }
            }
        });
    }

    private void HideMonthListView()
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread(() =>
        {
            if (_monthsSelectList != null)
            {
                _monthsSelectList.IsVisible = false;

                if (_daysGrid != null)
                    _daysGrid.IsVisible = true;
            }
        });
    }

    private async void OnMonthsPickerIndexChanged(object sender, EventArgs e)
    {
        HideMonthListView();

        var monthsSelectList = (SelectList)sender;
        if (monthsSelectList.SelectedItem is not MonthModel selectedMonth)
            return;

        await SetCurrentDateAndRebuildCalendar(_currentDate.Year, selectedMonth.Number, _currentDate.Day);
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
        string amPmDesignator = currentDate.ToString("tt", CultureInfo.InvariantCulture);
        DateTime dateTime = DateTime.ParseExact($"{hour12Format} {amPmDesignator}", "h tt", CultureInfo.InvariantCulture);
        return dateTime.Hour;
    }

    private static int UpdateHourByAmPmOption(int currentHour, string amPmOption)
    {
        // Update the _hour12 value based on the AM/PM option

        if (amPmOption.ToUpper() == "AM")
        {
            if (currentHour > 12)
            {
                currentHour -= 12;
            }
            else if (currentHour == 12)
            {
                currentHour = 0; // 12 AM is represented as 0 in 24-_hour12 format
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
                currentHour = 12; // 12 PM is represented as 12 in 24-_hour12 format
            }
        }

        return currentHour;
    }

    private static Color GetColorFromStyle(Style style, BindableProperty bindableProperty, Color defaultColor)
    {
        try
        {
            var colorSetter = style.Setters.FirstOrDefault(s => s.Property == bindableProperty);

            if (colorSetter != null)
            {
                return (Color)colorSetter.Value;
            }
        }
        catch (Exception ex) { }

        return defaultColor;
    }
}