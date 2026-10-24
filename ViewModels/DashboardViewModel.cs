using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ServicoEstoque _estoque;
    private readonly ServicoAutenticacao _auth;
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private View? conteudoAtual;

    [ObservableProperty]
    private string nomeUsuario = string.Empty;

    public DashboardViewModel(IServiceProvider services, ServicoEstoque estoque, ServicoAutenticacao auth)
    {
        _services = services;

        _estoque = estoque;
        _auth = auth;
        nomeUsuario = _auth.UsuarioLogado?.Nome ?? "Nome Indisponível";
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