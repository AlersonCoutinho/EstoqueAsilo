using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ServicoEstoque servicoEstoque;
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private View? conteudoAtual;

    [ObservableProperty]
    private string nomeUsuario = string.Empty;

    public DashboardViewModel(IServiceProvider services, ServicoEstoque servicoEstoque)
    {
        _services = services;

        this.servicoEstoque = servicoEstoque;
        nomeUsuario = servicoEstoque.UsuarioLogado.Nome;
    }

    [RelayCommand]
    private void SelecionarItem(string item)
    {
        ConteudoAtual = item switch
        {
            "inicio" => new HomeView(),
            "entrada" => _services.GetRequiredService<EntradaView>(),
            "saida" => new SaidaView(),
            "estoque" => new EstoqueView(),
            _ => null
        };
    }
}