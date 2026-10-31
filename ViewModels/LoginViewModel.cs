using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _auth;

    [ObservableProperty]
    public partial IEnumerable<string> Usuarios { get; set; } = [];

    [ObservableProperty]
    public partial string Usuario { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Senha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string MensagemErro { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool TemErro { get; set; } = false;

    public LoginViewModel(AuthService auth)
    {
        _auth = auth;
        Usuarios = _auth.ListarUsuarios();
    }

    [RelayCommand]
    private async Task Entrar()
    {
        if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Senha))
        {
            MensagemErro = "Preencha todos os campos!!!!";
            TemErro = true;
            return;
        }

        if (!_auth.Login(Usuario, Senha))
        {
            MensagemErro = "Senha inválida!!!!";
            TemErro = true;
            return;
        }

        TemErro = false;
        await Shell.Current.GoToAsync("dashboard");
    }

    [RelayCommand]
    private static async Task Debug() => System.Diagnostics.Process.Start("explorer.exe", FileSystem.AppDataDirectory);
}