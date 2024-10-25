using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using fhir_codegen_app.Logging;
using fhir_codegen_app.ViewModels;

namespace fhir_codegen_app.Views;

public partial class LogWindow : Window
{
    private CGLogEntry? item;

    public LogWindow()
    {
        InitializeComponent();
        
        this.Closing += LogWindow_Closing; ;

        if (this.DataContext == null)
        {
            this.DataContext = new LogWindowViewModel();
        }

        if (DataContext is LogWindowViewModel vm)
        {
            vm.DataStore.Backing.CollectionChanged += Backing_CollectionChanged;
        }
    }

    private void LogWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is LogWindowViewModel vm)
        {
            vm.DataStore.Backing.CollectionChanged -= Backing_CollectionChanged;
        }
    }
    private void Backing_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if ((DataContext is not LogWindowViewModel vm) ||
            !vm.AutoScroll ||
            (e.NewItems == null))
        {
            return;
        }

        LogGrid.ScrollIntoView(e.NewItems[0], null);
    }
}
