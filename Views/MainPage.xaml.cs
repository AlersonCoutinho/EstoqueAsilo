using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private void OnCampoNomeCompleted(object? sender, EventArgs e)
    {
        CaixaSenha.Focus();
    }
}