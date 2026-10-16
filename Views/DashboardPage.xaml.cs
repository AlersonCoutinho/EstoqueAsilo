using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.SelecionarItemCommand.Execute("inicio");
    }
}