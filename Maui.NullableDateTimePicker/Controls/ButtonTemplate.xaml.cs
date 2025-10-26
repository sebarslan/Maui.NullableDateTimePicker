using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;

namespace Maui.NullableDateTimePicker.Controls;

internal partial class ButtonTemplate : ContentView
{
    public event EventHandler Clicked;

    private Button _innerButton;

    public ButtonTemplate()
    {
        InitializeComponent();
        BackgroundColor = Colors.Transparent;
        TapCommand = new Command(OnTapped);
        BindingContext = this;
        UpdateVisualState();

    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(ButtonTemplate), default(string));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty SelectedProperty =
        BindableProperty.Create(nameof(Selected), typeof(bool), typeof(ButtonTemplate), false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((ButtonTemplate)bindable).UpdateVisualState();
            });

    public bool Selected
    {
        get => (bool)GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }

    public static readonly BindableProperty StrokeShapeProperty =
       BindableProperty.Create(nameof(StrokeShape), typeof(Shape), typeof(ButtonTemplate), default(Shape));

    public Shape StrokeShape
    {
        get => (Shape)GetValue(StrokeShapeProperty);
        set => SetValue(StrokeShapeProperty, value);
    }


    public ICommand TapCommand { get; }

    private void OnTapped()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateVisualState()
    {
        if (Selected)
        {
            OuterBorder.BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.DimGray : Colors.Blue;
            InnerLabel.TextColor = Colors.White;
        }
        else
        {
            OuterBorder.BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.DarkGray : Colors.LightGray;
            InnerLabel.TextColor = Colors.Black;
        }
    }
}