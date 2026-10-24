using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private void OnCampoNomeCompleted(object sender, EventArgs e)
    {
        Caixa2.Focus();
    }
    private void OnCampoSenhaCompleted(object sender, EventArgs e)
    {
        if (BindingContext is MainPageViewModel viewModel && viewModel.EntrarCommand.CanExecute(null))
        {
            viewModel.EntrarCommand.Execute(null);
        }
    }
}