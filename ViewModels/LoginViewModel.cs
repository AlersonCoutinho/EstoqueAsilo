using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels;

public partial class LoginViewModel(AuthService auth) : ObservableObject
{
    private readonly AuthService _auth = auth;

    [ObservableProperty]
    public partial string Nome { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Senha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string MensagemErro { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool TemErro { get; set; } = false;

    [RelayCommand]
    private async Task Entrar()
    {
        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Senha))
        {
            MensagemErro = "Preencha todos os campos!!!!";
            TemErro = true;
            return;
        }

        if (!_auth.Login(Nome, Senha))
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