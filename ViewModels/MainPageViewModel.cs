using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EstoqueAsilo.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string senha = string.Empty;

    [ObservableProperty]
    private string mensagemErro = string.Empty;

    [ObservableProperty]
    private bool temErro = false;

    [RelayCommand]
    private async Task Entrar()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
        {
            MensagemErro = "Preencha todos os campos!!!!";
            TemErro = true;
            return;
        }

        TemErro = false;
        await Shell.Current.GoToAsync("dashboard");
    }
}