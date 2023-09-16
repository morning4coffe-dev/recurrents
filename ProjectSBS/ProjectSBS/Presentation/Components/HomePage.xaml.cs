using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;

namespace ProjectSBS.Presentation.Components;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();
    }

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 20, 1, 3, 5, 3, 4, 6 },
            Fill = null
        }
    };

    //public Axis[] XAxes { get; set; } =
    //{
    //    new Axis
    //    {
    //        LabelsPaint = new SolidColorPaint
    //        {
    //            Color = SKColors.White,
    //        },
    //        // Use the labels property for named or static labels 
    //        Labels = new string[] { "Jan", "Feb", "Mar", "Apr", "May" },
    //    }
    //};

    //public Axis[] YAxes { get; set; } =
    //{
    //    new Axis
    //    {
    //        LabelsPaint = new SolidColorPaint
    //        {
    //            Color = SKColors.Transparent,
    //        },
    //    }
    //};

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
      nameof(Items),
      typeof(ObservableCollection<ItemViewModel>),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ObservableCollection<ItemViewModel> Items
    {
        get => (ObservableCollection<ItemViewModel>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value); 
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
      nameof(SelectedItem),
      typeof(ItemViewModel),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ItemViewModel? SelectedItem
    {
        get => (ItemViewModel?)GetValue(SelectedItemProperty);
        set
        {
            if (value != SelectedItem)
            {
                SetValue(SelectedItemProperty, value);
            }
        }
    }

    public static readonly DependencyProperty CategoriesProperty = DependencyProperty.Register(
      nameof(Categories),
      typeof(List<FilterCategory>),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public List<FilterCategory> Categories
    {
        get => (List<FilterCategory>)GetValue(CategoriesProperty);
        set => SetValue(CategoriesProperty, value);
    }

    public static readonly DependencyProperty SelectedCategoryProperty = DependencyProperty.Register(
      nameof(SelectedCategory),
      typeof(FilterCategory),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public FilterCategory SelectedCategory
    {
        get => (FilterCategory)GetValue(SelectedCategoryProperty);
        set
        {
            if (value != SelectedCategory)
            {
                SetValue(SelectedCategoryProperty, value);
            }
        }
    }

    public static readonly DependencyProperty AddNewCommandProperty = DependencyProperty.Register(
      nameof(AddNewCommand),
      typeof(ICommand),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ICommand AddNewCommand
    {
        get => (ICommand)GetValue(AddNewCommandProperty);
        set => SetValue(AddNewCommandProperty, value);
    }

    public static readonly DependencyProperty DeleteItemCommandProperty = DependencyProperty.Register(
      nameof(DeleteItemCommand),
      typeof(ICommand),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ICommand DeleteItemCommand
    {
        get => (ICommand)GetValue(DeleteItemCommandProperty);
        set => SetValue(DeleteItemCommandProperty, value);
    }

    public static readonly DependencyProperty ArchiveItemCommandProperty = DependencyProperty.Register(
      nameof(ArchiveItemCommand),
      typeof(ICommand),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ICommand ArchiveItemCommand
    {
        get => (ICommand)GetValue(ArchiveItemCommandProperty);
        set => SetValue(ArchiveItemCommandProperty, value);
    }

    public static readonly DependencyProperty EditItemCommandProperty = DependencyProperty.Register(
      nameof(EditItemCommand),
      typeof(ICommand),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ICommand EditItemCommand
    {
        get => (ICommand)GetValue(EditItemCommandProperty);
        set => SetValue(EditItemCommandProperty, value);
    }

    private void DeleteItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        DeleteItemCommand.Execute(args.SwipeControl.DataContext);
    }
    private void ArchiveItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        ArchiveItemCommand.Execute(args.SwipeControl.DataContext);
    }
    private void EditItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        SelectedItem = (ItemViewModel)args.SwipeControl.DataContext;

        EditItemCommand.Execute(args.SwipeControl.DataContext);
    }
}