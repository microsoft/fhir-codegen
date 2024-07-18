using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using fhir_codegen.ViewModels;

namespace fhir_codegen.Views;

public partial class WelcomePageView : UserControl
{
    public WelcomePageView()
    {
        InitializeComponent();

        if (DataContext == null)
        {
            DataContext = new WelcomePageViewModel();
        }
    }
}
