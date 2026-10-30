using EstoqueAsilo.ViewModels;

namespace EstoqueAsilo;

public partial class LoginView : ContentPage
{
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private void OnCampoNomeCompleted(object? sender, EventArgs e)
    {
        CaixaSenha.Focus();
    }
}