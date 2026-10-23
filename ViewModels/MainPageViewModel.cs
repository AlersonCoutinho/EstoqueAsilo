using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Models;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly ServicoEstoque servicoEstoque;

    [ObservableProperty]
    private string nome = string.Empty;

    [ObservableProperty]
    private string senha = string.Empty;

    [ObservableProperty]
    private string mensagemErro = string.Empty;

    [ObservableProperty]
    private bool temErro = false;

    public MainPageViewModel(ServicoEstoque servicoEstoque) => this.servicoEstoque = servicoEstoque;

    [RelayCommand]
    private async Task Entrar()
    {
        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Senha))
        {
            MensagemErro = "Preencha todos os campos!!!!";
            TemErro = true;
            return;
        }

        if (!servicoEstoque.Login(Nome, Senha))
        {
            MensagemErro = "Senha inválida!!!!";
            TemErro = true;
            return;
        }

        TemErro = false;
        await Shell.Current.GoToAsync("dashboard");
    }

    //[RelayCommand]
    //private async Task Cadastrar()
    //{
    //    if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Senha))
    //    {
    //        MensagemErro = "Preencha todos os campos!!!!";
    //        TemErro = true;
    //        return;
    //    }
    //    if (!servicoEstoque.CadastrarUsuario(Nome, Senha, false))
    //    {
    //        MensagemErro = "Usuário já existe!!!!";
    //        TemErro = true;
    //        return;
    //    }
    //    TemErro = false;
    //    await Shell.Current.GoToAsync("dashboard");
    //}

    [RelayCommand]
    private async Task Debug()
    {
        System.Diagnostics.Process.Start("explorer.exe", FileSystem.AppDataDirectory);
    }
}