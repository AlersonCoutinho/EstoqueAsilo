using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class RelatorioView : ContentView
{
	public RelatorioView(RelatorioViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}