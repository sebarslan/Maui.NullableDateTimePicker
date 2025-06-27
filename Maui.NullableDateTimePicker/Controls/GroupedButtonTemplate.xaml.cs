using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;

namespace Maui.NullableDateTimePicker.Controls;

public partial class GroupedButtonTemplate : ContentView
{
    public event EventHandler Clicked;

    private Button _innerButton;

    public GroupedButtonTemplate()
    {
        InitializeComponent();
        ButtonCommand = new Command(OnButtonCommandExecuted);
        this.Loaded += OnLoaded;
        BindingContext = this;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        _innerButton = this.FindByName<Button>("InnerButton");
        if (!string.IsNullOrEmpty(VisualState) && _innerButton != null)
        {
            VisualStateManager.GoToState(_innerButton, VisualState);
        }
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(GroupedButtonTemplate), default(string));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty StrokeShapeProperty =
        BindableProperty.Create(nameof(StrokeShape), typeof(Shape), typeof(GroupedButtonTemplate), default(Shape));

    public Shape StrokeShape
    {
        get => (Shape)GetValue(StrokeShapeProperty);
        set => SetValue(StrokeShapeProperty, value);
    }

    public ICommand ButtonCommand { get; }

    private void OnButtonCommandExecuted()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }

    public static readonly BindableProperty VisualStateProperty =
        BindableProperty.Create(
            nameof(VisualState),
            typeof(string),
            typeof(GroupedButtonTemplate),
            "Normal",
            propertyChanged: OnVisualStateChanged);

    public string VisualState
    {
        get => (string)GetValue(VisualStateProperty);
        set => SetValue(VisualStateProperty, value);
    }

    private static void OnVisualStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        
        var control = (GroupedButtonTemplate)bindable;
        var newState = newValue as string;

        if (control.InnerButton != null && !string.IsNullOrEmpty(newState))
            VisualStateManager.GoToState(control.InnerButton, newState);
    }
}