using Maui.NullableDateTimePicker;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Xamarin.UITest.Shared.Screenshots;

namespace Maui.NullableDateTimePicker.Tests
{
    [TestFixture]
    public class UnitTests
    {
        private NullableDateTimePickerOptions options;
        private DateTime? testDate = DateTime.Now;
        private NullableDateTimePickerContent content;
        private NullableDateTimePicker nullableDateTimePicker;

        [SetUp]
        public void SetUp()
        {
            options = new()
            {
                InitDateTimeValue = testDate,
                PickerMode = PickerMode.Date
            };
            content = new NullableDateTimePickerContent(options);
            nullableDateTimePicker = new NullableDateTimePicker();

        }

        [Test]
        public void NullableDateTimePickerContent_SelectedDate_Test()
        {
            // Arrange
            content.InitCalendar();

            // Act

            // Assert
            Assert.AreEqual(testDate, content.SelectedDate);
        }

        [Test]
        public void NullableDateTimePickerContent_SetDate_Test()
        {
            // Arrange
            var content = new NullableDateTimePickerContent(options);
            content.InitCalendar();

            // Act
            var newDate = new DateTime(2000, 1, 1);

            content.SetCurrentDateAndRebuildCalendar(newDate.Year, newDate.Month, newDate.Day);

            content.OnOkButtonClicked(this, null);

            // Asserts
            Assert.AreEqual(newDate.Date, content.SelectedDate?.Date);

        }

        [Test]
        [TestCase("t", PickerMode.Time)]
        [TestCase("g", PickerMode.DateTime)]
        [TestCase("d", PickerMode.Time)]
        [TestCase("", PickerMode.Time)]
        [TestCase("", PickerMode.DateTime)]
        [TestCase("", PickerMode.Time)]
        public void NullableDateTimePickerContent_DateFormat_Test(string format, PickerMode pickerMode)
        {
            // Arrange
            var date = DateTime.Now;

            // Act
            nullableDateTimePicker.Format = format;
            nullableDateTimePicker.Mode = pickerMode;
            nullableDateTimePicker.NullableDateTime = date;

            if (string.IsNullOrEmpty(format))
                format = nullableDateTimePicker.Format;

            // Assert
            Assert.AreEqual(nullableDateTimePicker.NullableDateTime?.ToString(format), nullableDateTimePicker._dateTimePickerEntry.Text);
        }

        [Test]
        public void NullableDateTimePickerContent_InitNullDateTime_Test()
        {
            NullableDateTimePickerOptions options = new()
            {
                InitDateTimeValue = null,
                PickerMode = PickerMode.DateTime
            };

            // Arrange
            var content = new NullableDateTimePickerContent(options);


            content.InitCalendar();

            // Act

            // Assert
            Assert.IsNull(content.SelectedDate);
        }


        [Test]
        public void NullableDateTimePickerContent_SetYear_Test()
        {
            // Arrange
            var startDate = DateTime.Now.AddYears(-10);
            var endDate = DateTime.Now.AddYears(10);
            var options = new NullableDateTimePickerOptions()
            {
                InitDateTimeValue = DateTime.Now,
                PickerMode = PickerMode.Date,
                MinDate = startDate,
                MaxDate = endDate
            };
            var content = new NullableDateTimePickerContent(options);

            content.InitCalendar();

            Random rnd = new Random();
            int randomYear = rnd.Next(startDate.Year, endDate.Year);

            // Act
            content.SetYear(randomYear);
            content.OnOkButtonClicked(this, null);

            // Assert
            Assert.AreEqual(randomYear, content.SelectedDate?.Year);
        }
    }
}