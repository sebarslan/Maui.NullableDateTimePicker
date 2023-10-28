# Maui Nullable and Clearable DateTimePicker
The Nullable DateTimePicker is a custom calendar control for selecting a nullable date and time value in a .NET MAUI application. It provides a consistent and platform-independent user interface for selecting dates, and allows the user to clear the value if needed.

This control uses the <a href="https://github.com/CommunityToolkit/Maui" target="_blank">CommunityToolkit.Maui</a> Popup.

[![NuGet](https://img.shields.io/badge/nuget-v1.0.2-blue.svg?style=plastic)](https://www.nuget.org/packages/Sebarslan.Maui.NullableDateTimePicker)


# Usage
To use the Nullable DateTimePicker control in your .NET MAUI application, follow these steps:

0- Add .ConfigureNullableDateTimePicker() to the MauiProgram.cs file in your project.

<pre>
<code>
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
    .UseMauiApp&lt;App&gt;()
    .ConfigureNullableDateTimePicker()
    //.UseMauiCommunityToolkit() //For versions 1.0.2 and earlier
    ....
</code>
</pre>

### Use direct calendar popup with your own entry and button

1- Add your entry or button for datetime in xaml page (eg. MainPage.xaml)
<pre>
<code>
&lt;HorizontalStackLayout HorizontalOptions="Fill"&gt;
    &lt;Entry Text="{Binding MyDateTime, StringFormat='{0:d} {0:t}'}" HeightRequest="40" HorizontalOptions="Fill" IsReadOnly="True"&gt;
        &lt;Entry.GestureRecognizers&gt;
            &lt;TapGestureRecognizer Tapped="DateTimePicker_Clicked" /&gt;
        &lt;/Entry.GestureRecognizers&gt;
    &lt;/Entry&gt;
    &lt;ImageButton Source="{Binding CalendarIcon}" Clicked="DateTimePicker_Clicked" WidthRequest="30" HeightRequest="30" /&gt;
&lt;/HorizontalStackLayout&gt;
 </code>
</pre>  

2- Then, when you click on the button or entry, define the options and call NullableDateTimePicker.OpenPopupAsync(options) to open the calendar.
<pre>
<code>
private async void DateTimePicker_Clicked(object sender, EventArgs e)
{
    INullableDateTimePickerOptions nullableDateTimePickerOptions = new NullableDateTimePickerOptions
    {
        InitDateTimeValue = MyDateTime,
        PickerMode = PickerMode.DateTime,
        ShowClearButton = true,
        ShowWeekNumbers = true
        // .. other options
    };

    var result = await NullableDateTimePicker.OpenPopupAsync(nullableDateTimePickerOptions);
    if (result is PopupResult popupResult && popupResult.ButtonResult != PopupButton.Cancel)
    {
        MyDateTime = popupResult.DateTimeValue;
    }
}
</code>
</pre>    

### or use as ContentView
1- Add the NullableDateTimePicker control to your XAML layout file:

xmlns:ndtp="clr-namespace:Maui.NullableDateTimePicker;assembly=Maui.NullableDateTimePicker"
<pre>
<code>
&lt;ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:ndtp="clr-namespace:Maui.NullableDateTimePicker;assembly=Maui.NullableDateTimePicker"
             x:Class="Maui.NullableDateTimePicker.Samples.MainPage"&gt;
</code>
</pre>

2- Bind the DateTime? value property to a property in your view model:
<pre>
<code>
&lt;ndtp:NullableDateTimePicker NullableDateTime="{Binding MyDateTime}" Mode="Date" /&gt;
</code>
</pre>

More examples, please see the samples project

# Options
| Option | Description | Default Value |
|--------|-------------|---------|
| NullableDateTime | Gets or sets the nullable date and time value of the control. | null |
| Mode | Specifies the mode of the control. Valid values are Date, DateTime, and Time. | Date |
| Format | Specifies the display format for the date or time. | for date: d, for datetime: g, for time: t |
| MinDate | Minimum selectable date of the control. | DateTime.MinValue |
| MaxDate |	Maximum selectable date of the control. | DateTime.MaxValue |
| OkButtonText | The text for the OK button. | OK |
| CancelButtonText | The text for the Cancel button. | Cancel |
| ClearButtonText | Gets or sets the text for the Clear button.	| Clear |
| ShowClearButton | Clear button can be hidden/shown. If true, the button is displayed.	| true |
| ForeColor | It is used for the color of texts that cannot be styled in the calendar. | Black |
| BackgroundColor | Background color of the calendar.	| White |
| HeaderForeColor | Gets or sets the foreground color of the control's header. | White |
| HeaderBackgroundColor | Background color of the control's header.	| #2b0b98 |
| ToolButtonsStyle | Style of the control's tool buttons. | null |
| DayStyle | Style of the days in the calendar. | null |
| SelectedDayStyle | Style of the selected day in the calendar. | null |
| DayNamesStyle | Style of the day names in the calendar. | null |
| OtherMonthDayStyle | Style of the other month days in the calendar. | null |
| WeekNumberStyle | Style of the week numbers in the calendar. | null |
| ShowWeekNumbers | Determines whether to display week numbers in the calendar.	| false |
| ShowOtherMonthDays | Determines whether to display other month days in the calendar.	| true |


## NullableDateTimeChanged Event (If NullableDateTimePicker is used as ContentView)
The NullableDateTimeChanged event is used to indicate when a NullableDateTime value has been changed. 
This event is commonly used in programming or software environments and is triggered when the NullableDateTime value is modified.

The event utilizes the DateTimeChangedEventArgs class as its argument. The DateTimeChangedEventArgs class contains additional information that is carried at the moment the event is triggered. It may include details about the date and time change, such as the old DateTime value and the new DateTime value.

Below is an example code snippet illustrating the usage of the "NullableDateTimeChanged" event and the "DateTimeChangedEventArgs" argument class:
<pre>
<code>
NullableDateTimePicker dateTimePicker = new NullableDateTimePicker();
dateTimePicker.NullableDateTimeChanged += OnNullableDateTimeChanged;

private static void OnNullableDateTimeChanged(object sender, DateTimeChangedEventArgs e)
{
    Console.WriteLine("DateTime changed!");
    Console.WriteLine("Old DateTime: " + e.OldDateTime);
    Console.WriteLine("New DateTime: " + e.NewDateTime);
}
</code>
</pre>


# License
The Nullable DateTimePicker control is licensed under the MIT License. See <a href="LICENSE.txt">LICENSE file</a> for more information.

# Contributing
Contributions are welcome!

# Screenshot
on ios, android, windows

![DateTimePicker](https://raw.githubusercontent.com/sebarslan/Maui.NullableDateTimePicker/main/screenshot.png)

# Changelog

### 1.1.0
- The calendar popup can be opened directly via the NullableDateTimePicker, so you can use your own entry and button.
- A Builder Extension has been added. In this way, Configure Nullable DateTimePicker can be used without adding CommunityToolKit to your own project.
- On some screens, week and day numbers were not displayed on the same line.

### 1.0.2
- The problem of displaying the default icon in default mode has been fixed.

- Various improvements.

### 1.0.1
- The problem of not setting the margin has been fixed.
