using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly EstoqueService _estoque;
    private readonly AuthService _auth;
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private View? conteudoAtual;

    [ObservableProperty]
    private string nomeUsuario = string.Empty;

    public DashboardViewModel(IServiceProvider services, EstoqueService estoque, AuthService auth)
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
            "inicio" => _services.GetRequiredService<HomeView>(),
            "entrada" => _services.GetRequiredService<EntradaView>(),
            "saida" => new SaidaView(),
            "estoque" => new EstoqueView(),
            "estoque_alimentos" => new EstoqueView(),
            "estoque_limpeza" => new EstoqueView(),
            "estoque_almoxarifado" => new EstoqueView(),
            _ => null
        };
    }
}