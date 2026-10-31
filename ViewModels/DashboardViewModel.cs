using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly AuthService _auth;
    private readonly IServiceProvider _services;

    [ObservableProperty]
    public partial View? ConteudoAtual { get; set; } = null;

    [ObservableProperty]
    public partial string NomeUsuario { get; set; } = string.Empty;

    public DashboardViewModel(IServiceProvider services, AuthService auth)
    {
        _services = services;
        _auth = auth;

        NomeUsuario = _auth.UsuarioLogado?.Nome ?? "Nome Indisponível";
    }

    [RelayCommand]
    private void SelecionarItem(string item)
    {
        ConteudoAtual = item switch
        {
            "inicio" => _services.GetRequiredService<HomeView>(),
            "entrada" => _services.GetRequiredService<EntradaView>(),
            "saida" => _services.GetRequiredService<SaidaView>(),
            "estoque" => new EstoqueView(),
            "estoque_alimentos" => new EstoqueView(),
            "estoque_limpeza" => new EstoqueView(),
            "estoque_almoxarifado" => new EstoqueView(),
            _ => null
        };
    }
}