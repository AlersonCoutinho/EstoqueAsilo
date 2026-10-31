using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class SaidaView : ContentView
{
    public SaidaView(SaidaViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}