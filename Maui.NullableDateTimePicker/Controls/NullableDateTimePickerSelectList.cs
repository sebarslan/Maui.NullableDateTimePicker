using Microsoft.Maui.Controls.Shapes;
using System.Collections;

namespace Maui.NullableDateTimePicker;

internal class NullableDateTimePickerSelectList : ContentView
{
    public event EventHandler SelectedIndexChanged;
    public event EventHandler Closed;

    // ItemsSource BindableProperty
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(NullableDateTimePickerSelectList), null, propertyChanged: OnItemsSourceChanged);

    public IList ItemsSource
    {
        get => (IList)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    // SelectedIndex BindableProperty
    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(NullableDateTimePickerSelectList), -1, BindingMode.TwoWay, propertyChanged: OnSelectedIndexChanged);

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    // SelectedItem BindableProperty
    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(NullableDateTimePickerSelectList), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    // ItemDisplayBinding BindableProperty
    public static readonly BindableProperty ItemDisplayBindingProperty =
        BindableProperty.Create(nameof(ItemDisplayBinding), typeof(string), typeof(NullableDateTimePickerSelectList), null);

    public string ItemDisplayBinding
    {
        get => (string)GetValue(ItemDisplayBindingProperty);
        set => SetValue(ItemDisplayBindingProperty, value);
    }

    // TextColor BindableProperty
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor), // Property name
            typeof(Color),      // Property type
            typeof(NullableDateTimePickerSelectList), // Declaring type
            Colors.Black,       // Default value
            BindingMode.OneWay, // Binding mode
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is NullableDateTimePickerSelectList control && newValue is Color newColor)
                {
                    control._label.TextColor = newColor; // Update the Label's TextColor
                }
            }
        );

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    private readonly CollectionView _collectionView;
    private Label _label;
    public NullableDateTimePickerSelectList()
    {
        VerticalOptions = LayoutOptions.Fill;
        HorizontalOptions = LayoutOptions.Fill;

        _collectionView = new CollectionView
        {
            ItemsLayout = new GridItemsLayout(3, ItemsLayoutOrientation.Vertical),
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        _collectionView.SelectionChanged += OnSelectionChanged;

        // Define the item template
        _collectionView.ItemTemplate = new DataTemplate(() =>
        {
            _label = new Label
            {
                TextColor = TextColor,
                FontSize = 12,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            _label.SetBinding(Label.TextProperty, new Binding(ItemDisplayBinding ?? ".", source: _label.BindingContext));

            var border = new Border
            {
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5, 5, 5, 5)
                },
                Margin = 2,
                Padding = 5
            };

            // Define visual states for the border
            VisualStateManager.SetVisualStateGroups(border, new VisualStateGroupList
            {
                new VisualStateGroup
                {
                    Name = "CommonStates",
                    States =
                    {
                        new VisualState
                        {
                            Name = "Normal",
                            Setters =
                            {
                                new Setter { Property = Border.BackgroundColorProperty, Value = Colors.White },
                                new Setter { Property = Border.BackgroundProperty, Value = Colors.White },
                                new Setter { Property = Border.StrokeProperty, Value = Colors.Gray },
                                new Setter { Property = Border.StrokeThicknessProperty, Value = 1 }
                            }
                        },
                        new VisualState
                        {
                            Name = "Selected",
                            Setters =
                            {
                                new Setter { Property = Border.BackgroundColorProperty, Value = Colors.LightBlue },
                                new Setter { Property = Border.BackgroundProperty, Value = Colors.LightBlue },
                                new Setter { Property = Border.StrokeProperty, Value = Colors.Blue },
                                new Setter { Property = Border.StrokeThicknessProperty, Value = 2 }
                            }
                        }
                    }
                }
            });

            border.Content = _label;
            return border;
        });

        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(20, GridUnitType.Absolute) }
            }
        };

        var closeButton = new Button
        {
            Text = " X ",
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.End,
            FontSize = 16,
            MaximumWidthRequest = 100,
            MaximumHeightRequest = 20,
            BorderWidth = 1,
            HeightRequest = 20,
            Padding = 0,
            Margin = 0
        };
        closeButton.Clicked += (s, e) =>
        {
            Closed?.Invoke(this, EventArgs.Empty);
        };
        grid.Add(_collectionView, 0, 0);
        grid.Add(closeButton, 0, 1);

        Content = grid;
    }

    // Update the CollectionView's ItemsSource when ItemsSource changes
    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (NullableDateTimePickerSelectList)bindable;
        control._collectionView.ItemsSource = (IList)newValue;
    }

    // Update the CollectionView's selected item when SelectedItem changes
    private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (NullableDateTimePickerSelectList)bindable;
        if (newValue != null)
        {
            control._collectionView.SelectedItem = newValue;
        }
    }

    // Update the selected item in CollectionView when SelectedIndex changes
    private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (NullableDateTimePickerSelectList)bindable;
        int selectedIndex = (int)newValue;

        // If the index is out of range, set selection to null
        if (selectedIndex >= 0 && selectedIndex < control.ItemsSource?.Count)
        {
            control._collectionView.SelectedItem = control.ItemsSource[selectedIndex];
        }
        else
        {
            control._collectionView.SelectedItem = null;
        }
    }

    // Update SelectedIndex and SelectedItem when selection changes
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
        {
            var selectedItem = e.CurrentSelection[0];
            SelectedItem = selectedItem;
            SelectedIndex = ItemsSource?.IndexOf(selectedItem) ?? -1;

            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            ScrollToSelectedItem();
        }
    }

    // Scroll to the selected item
    private void ScrollToSelectedItem()
    {
        if (ItemsSource == null || SelectedItem == null)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(300);
            _collectionView.ScrollTo(SelectedItem, position: ScrollToPosition.Center, animate: true);
        });
    }
}