using Microsoft.Maui.Controls.Shapes;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Maui.NullableDateTimePicker;

internal class NullableDateTimePickerSelectList : ContentView
{
    public event EventHandler SelectedIndexChanged;
    public event EventHandler Closed;

    #region BindableProperties
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

    // ItemTextColor BindableProperty
    public static readonly BindableProperty ItemTextColorProperty =
        BindableProperty.Create(
            nameof(ItemTextColor), // Property name
            typeof(Color),      // Property type
            typeof(NullableDateTimePickerSelectList), // Declaring type
            Colors.Black,       // Default value
            BindingMode.OneWay // Binding mode
        );

    public Color ItemTextColor
    {
        get => (Color)GetValue(ItemTextColorProperty);
        set => SetValue(ItemTextColorProperty, value);
    }

    // ItemBackgroundColor BindableProperty
    public static readonly BindableProperty ItemBackgroundColorProperty =
        BindableProperty.Create(
            nameof(ItemBackgroundColor), // Property name
            typeof(Color),      // Property type
            typeof(NullableDateTimePickerSelectList), // Declaring type
            Colors.White,       // Default value
            BindingMode.OneWay // Binding mode
        );

    public Color ItemBackgroundColor
    {
        get => (Color)GetValue(ItemBackgroundColorProperty);
        set => SetValue(ItemBackgroundColorProperty, value);
    }

    // SelectedItemTextColor BindableProperty
    public static readonly BindableProperty SelectedItemTextColorProperty =
        BindableProperty.Create(
            nameof(SelectedItemTextColor), // Property name
            typeof(Color),      // Property type
            typeof(NullableDateTimePickerSelectList), // Declaring type
            Colors.Black,       // Default value
            BindingMode.OneWay // Binding mode
        );

    public Color SelectedItemTextColor
    {
        get => (Color)GetValue(SelectedItemTextColorProperty);
        set => SetValue(SelectedItemTextColorProperty, value);
    }
    // SelectedItemBackgroundColor BindableProperty
    public static readonly BindableProperty SelectedItemBackgroundColorProperty =
        BindableProperty.Create(
            nameof(SelectedItemBackgroundColor), // Property name
            typeof(Color),      // Property type
            typeof(NullableDateTimePickerSelectList), // Declaring type
            Colors.LightBlue,       // Default value
            BindingMode.OneWay // Binding mode
        );

    public Color SelectedItemBackgroundColor
    {
        get => (Color)GetValue(SelectedItemBackgroundColorProperty);
        set => SetValue(SelectedItemBackgroundColorProperty, value);
    }
    #endregion //BindableProperties

    private readonly CollectionView _collectionView;

    public NullableDateTimePickerSelectList()
    {
        VerticalOptions = LayoutOptions.Fill;
        HorizontalOptions = LayoutOptions.Fill;

        _collectionView = new CollectionView
        {
            ItemsLayout = new GridItemsLayout(3, ItemsLayoutOrientation.Vertical)
            {
                HorizontalItemSpacing = 3,
                VerticalItemSpacing = 3
            },
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem
        };

        _collectionView.SelectionChanged += OnSelectionChanged;

        // Define the item template
        _collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var label = new Label
            {
                FontSize = 12,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = 0,
                Margin = 0
            };

            label.SetBinding(Label.TextProperty, new Binding(ItemDisplayBinding ?? "."));

            VisualStateManager.SetVisualStateGroups(label, new VisualStateGroupList
            {
                new VisualStateGroup
                {
                    Name = "CommonStates",
                    States =
                    {
                        new VisualState
                        {
                            Name = "Normal",
                            Setters = { new Setter { Property = Label.TextColorProperty, Value = ItemTextColor ?? Colors.Black } }
                        },
                        new VisualState
                        {
                            Name = "Selected",
                            Setters = { new Setter { Property = Label.TextColorProperty, Value = SelectedItemTextColor ?? Colors.Black } }
                        }
                    }
                }

            });

            var border = new Border
            {
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5, 5, 5, 5)
                },
                Margin = 0,
                Padding = 5,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Content = label
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
                                new Setter { Property = Border.BackgroundColorProperty, Value = ItemBackgroundColor ?? Colors.White },
                                new Setter { Property = Border.BackgroundProperty, Value =  ItemBackgroundColor ?? Colors.White },
                                new Setter { Property = Border.StrokeProperty, Value = Colors.Gray },
                                new Setter { Property = Border.StrokeThicknessProperty, Value = 1 }
                            }
                        },
                        new VisualState
                        {
                            Name = "Selected",
                            Setters =
                            {
                                new Setter { Property = Border.BackgroundColorProperty, Value = SelectedItemBackgroundColor ?? Colors.LightBlue },
                                new Setter { Property = Border.BackgroundProperty, Value = SelectedItemBackgroundColor ?? Colors.LightBlue },
                                new Setter { Property = Border.StrokeProperty, Value = Colors.Blue },
                                new Setter { Property = Border.StrokeThicknessProperty, Value = 2 }
                            }
                        }
                    }
                }
            });

            return border;
        });

        var grid = new Grid
        {
            Background = Colors.Transparent,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            }
        };

        var closeButton = new Button
        {
            Text = "X",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.End,
            FontSize = 16,
            MaximumWidthRequest = 50,
            MaximumHeightRequest = DeviceInfo.Platform == DevicePlatform.WinUI ? 20 : 25,
            BorderWidth = 1,
            WidthRequest = 50,
            HeightRequest = DeviceInfo.Platform == DevicePlatform.WinUI ? 20 : 25,
            Padding = 0,
            Margin = new Thickness(5)
        };
        closeButton.Clicked += (s, e) =>
        {
            Closed?.Invoke(this, EventArgs.Empty);
        };
        grid.Add(closeButton, 0, 0);
        grid.Add(_collectionView, 0, 1);

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

        control._collectionView.SelectedItem = newValue;
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

            int selectedIndex = ItemsSource?.IndexOf(selectedItem) ?? -1;

            SelectedIndex = selectedIndex;
            SelectedItem = selectedItem;
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            ScrollToSelectedItem();
        }
    }

    // Scroll to the selected item
    private void ScrollToSelectedItem()
    {
        if (ItemsSource == null || SelectedItem == null || SelectedIndex == -1)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            int delay = Math.Min(ItemsSource.Count * 2, 2000);
            await Task.Delay(delay);
            _collectionView.ScrollTo(SelectedIndex, position: ScrollToPosition.Center, animate: false);
        });
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsEnabled))
        {
            _collectionView.IsEnabled = this.IsEnabled;
            this.IsEnabled = true;
        }
    }
}