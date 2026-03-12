using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Shapes;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.NullableDateTimePicker;

internal class SelectList : ContentView
{
    public event EventHandler SelectedIndexChanged;
    public event EventHandler Closed;

    ObservableCollection<SelectListItem> _items = new();

    #region BindableProperties
    // ItemsSource BindableProperty
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(SelectList), null, propertyChanged: OnItemsSourceChanged);

    public IList ItemsSource
    {
        get => (IList)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    // SelectedIndex BindableProperty
    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(SelectList), -1, BindingMode.TwoWay, propertyChanged: OnSelectedIndexChanged);

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    // SelectedItem BindableProperty
    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SelectList), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    // ItemDisplayBinding BindableProperty
    public static readonly BindableProperty ItemDisplayBindingProperty =
        BindableProperty.Create(nameof(ItemDisplayBinding), typeof(string), typeof(SelectList), null);

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
            typeof(SelectList), // Declaring type
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
            typeof(SelectList), // Declaring type
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
            typeof(SelectList), // Declaring type
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
            typeof(SelectList), // Declaring type
            Colors.LightBlue,       // Default value
            BindingMode.OneWay // Binding mode
        );

    public Color? SelectedItemBackgroundColor
    {
        get => (Color)GetValue(SelectedItemBackgroundColorProperty);
        set => SetValue(SelectedItemBackgroundColorProperty, value);
    }
    #endregion //BindableProperties

    private readonly CollectionView _collectionView;

    public SelectList()
    {
        VerticalOptions = LayoutOptions.Fill;
        HorizontalOptions = LayoutOptions.Fill;

        _collectionView = new CollectionView
        {
            ItemsLayout = new GridItemsLayout(3, ItemsLayoutOrientation.Vertical)
            {
                HorizontalItemSpacing = DeviceInfo.Platform == DevicePlatform.WinUI ? 2 : 3,
                VerticalItemSpacing = DeviceInfo.Platform == DevicePlatform.WinUI ? 2 : 3
            },
            SelectionMode = SelectionMode.None,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        _collectionView.SelectionChanged += OnSelectionChanged;

        // Define the item template
        _collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var label = new Label
            {
                FontSize = 12,
                BackgroundColor = Colors.Transparent,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = ItemTextColor
            };


            if (string.IsNullOrEmpty(ItemDisplayBinding))
            {
                label.SetBinding(Label.TextProperty, "Item");
            }
            else
            {
                label.SetBinding(Label.TextProperty, $"Item.{ItemDisplayBinding}");
            }
            label.SetAppThemeColor(
    Label.TextColorProperty,
    ItemTextColor,
    ItemTextColor == Colors.Black ? Colors.White : ItemTextColor
);

            label.Triggers.Add(new DataTrigger(typeof(Label))
            {
                Binding = new Binding(nameof(SelectListItem.IsSelected)),
                Value = true,
                Setters =
    {
        new Setter
        {
            Property = Label.TextColorProperty,
            Value = SelectedItemTextColor
        }
    }
            });



            var border = new Border
            {
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5)
                },
                Stroke = Color.FromArgb("#D0D0D0"),
                StrokeThickness = 1,
                HeightRequest = 35,
                HorizontalOptions = LayoutOptions.Fill,
                Content = label
            };


            border.SetAppThemeColor(
    Border.BackgroundColorProperty,
    ItemBackgroundColor == Colors.Transparent ? Color.FromArgb("#F2F2F2") : ItemBackgroundColor,
    ItemBackgroundColor == Colors.Transparent ? Color.FromArgb("#2B2B2B") : ItemBackgroundColor
);

            border.Triggers.Add(new DataTrigger(typeof(Border))
            {
                Binding = new Binding(nameof(SelectListItem.IsSelected)),
                Value = true,
                Setters =
    {
        new Setter
        {
            Property = Border.BackgroundColorProperty,
            Value = SelectedItemBackgroundColor
        },
        new Setter
        {
            Property = Border.StrokeThicknessProperty,
            Value = 0
        }
    }
            });






            //if (DeviceInfo.Platform == DevicePlatform.iOS)
            //{
            //    // Tap to select (works reliably on iOS)
            //    var tap = new TapGestureRecognizer
            //    {
            //        NumberOfTapsRequired = 1,
            //        Command = new Command(() =>
            //        {
            //            _collectionView.SelectedItem = border.BindingContext;
            //        })
            //    };
            //    border.GestureRecognizers.Add(tap);
            //}

            var tap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() =>
                {
                    var item = (SelectListItem)border.BindingContext;

                    _collectionView.SelectedItem = item.Item;
                })
            };
            border.GestureRecognizers.Add(tap);



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
            MaximumWidthRequest = DeviceInfo.Platform == DevicePlatform.WinUI ? 40 : 50,
            MaximumHeightRequest = DeviceInfo.Platform == DevicePlatform.WinUI ? 20 : 25,
            MinimumHeightRequest = DeviceInfo.Platform == DevicePlatform.WinUI ? 20 : 25,
            BorderWidth = 1,
            WidthRequest = DeviceInfo.Platform == DevicePlatform.WinUI ? 40 : 50,
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
        var control = (SelectList)bindable;

        control._items.Clear();

        if (newValue is IList list)
        {
            foreach (var item in list)
            {
                control._items.Add(new SelectListItem
                {
                    Item = item
                });
            }
        }

        control._collectionView.ItemsSource = control._items;
    }

    // Update the CollectionView's selected item when SelectedItem changes
    private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SelectList)bindable;

        control._collectionView.SelectedItem = newValue;
    }

    // Update the selected item in CollectionView when SelectedIndex changes
    private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SelectList)bindable;

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
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
        {
            var selectedItem = e.CurrentSelection[0];

            int selectedIndex = ItemsSource?.IndexOf(selectedItem) ?? -1;
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            SyncSelection();
            ScrollToSelectedItem();
        }
    }

    private void SyncSelection()
    {
        if (_items == null || _items.Count == 0)
            return;

        foreach (var item in _items)
            item.IsSelected = false;

        if (SelectedItem != null)
        {
            var selected = _items.FirstOrDefault(x => ReferenceEquals(x.Item, SelectedItem) || Equals(x.Item, SelectedItem));
            if (selected != null)
            {
                selected.IsSelected = true;
                return;
            }
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

internal class SelectListItem : INotifyPropertyChanged
{
    public object? Item { get; set; }

    bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}