using EstoqueAsilo.ViewModels;
namespace EstoqueAsilo;

public partial class EstoqueView : ContentView
{
    public class NavegarParaEntradaMessage { }
    public EstoqueView(EstoqueViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}