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
        get { return (ObservableCollection<ItemViewModel>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
      nameof(SelectedItem),
      typeof(ItemViewModel),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ItemViewModel? SelectedItem
    {
        get { return (ItemViewModel?)GetValue(SelectedItemProperty); }
        set
        {
            if (value != SelectedItem)
            {
                SetValue(SelectedItemProperty, value);
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
        get { return (ICommand)GetValue(AddNewCommandProperty); }
        set { SetValue(AddNewCommandProperty, value); }
    }
}