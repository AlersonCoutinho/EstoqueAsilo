using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class HomeView : ContentView
{
    public HomeView(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}