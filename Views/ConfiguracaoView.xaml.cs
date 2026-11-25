using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class ConfiguracaoView : ContentView
{
    public ConfiguracaoView(ConfiguracaoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}