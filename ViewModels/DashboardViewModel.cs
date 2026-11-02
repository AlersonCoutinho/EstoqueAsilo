using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EstoqueAsilo.Enums;
using EstoqueAsilo.Services;
using static EstoqueAsilo.EstoqueView;

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

        _estoque = estoque;
        _auth = auth;
        nomeUsuario = _auth.UsuarioLogado?.Nome ?? "Nome Indisponível";
        WeakReferenceMessenger.Default.Register<NavegarParaEntradaMessage>(this, (r, m) =>
        {
            SelecionarItem("entrada");
        });
    }

    [RelayCommand]
    private void SelecionarItem(string item)
    {
        ConteudoAtual = item switch
        {
            "inicio" => _services.GetRequiredService<HomeView>(),
            "entrada" => _services.GetRequiredService<EntradaView>(),
            "saida" => new SaidaView(),
            "estoque_alimentos" => CriarEstoqueView(Categoria.Alimentos),
            "estoque_limpeza" => CriarEstoqueView(Categoria.Limpeza),
            "estoque_almoxarifado" => CriarEstoqueView(Categoria.Almoxarifado),
            "relatorios" => _services.GetRequiredService<RelatorioView>(),
            _ => null
        };
    }

    private EstoqueView CriarEstoqueView(Categoria categoria)
    {
        var vm = _services.GetRequiredService<EstoqueViewModel>();
        vm.CarregarCategoria(categoria);
        return new EstoqueView(vm);
    }
}