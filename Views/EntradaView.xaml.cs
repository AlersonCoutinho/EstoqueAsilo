using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class EntradaView : ContentView
{
	public EntradaView(EntradaViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
    private void OnSimChecked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value && BindingContext is EntradaViewModel vm)
            vm.EhDoacao = true;
    }

    private void OnNaoChecked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value && BindingContext is EntradaViewModel vm)
            vm.EhDoacao = false;
    }
}