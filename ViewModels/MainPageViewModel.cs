using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly AuthService _auth;

    [ObservableProperty]
    private string nome = string.Empty;

    [ObservableProperty]
    private string senha = string.Empty;

    [ObservableProperty]
    private string mensagemErro = string.Empty;

    [ObservableProperty]
    private bool temErro = false;

    public MainPageViewModel(AuthService auth) => this._auth = auth;

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