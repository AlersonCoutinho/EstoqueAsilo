using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class EntradaView : ContentView
{
    public EntradaView(EntradaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}