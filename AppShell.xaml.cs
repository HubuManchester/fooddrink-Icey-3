namespace FoodieTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(Views.AddEntryPage), typeof(Views.AddEntryPage));
        Routing.RegisterRoute(nameof(Views.DetailPage), typeof(Views.DetailPage));
    }
}