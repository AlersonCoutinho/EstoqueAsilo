using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EstoqueAsilo.Enums;
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
    [ObservableProperty]
    public partial bool Carregando { get; set; }

    public DashboardViewModel(IServiceProvider services, AuthService auth)
    {
        _services = services;
        _auth = auth;

        NomeUsuario = _auth.UsuarioLogado?.Nome ?? "Nome Indisponível";
        WeakReferenceMessenger.Default.Register<NavegarParaEntradaMessage>(this, (r, m) =>
        {
            ConteudoAtual = _services.GetRequiredService<HomeView>();
        });
    }

    [RelayCommand]
    private async Task SelecionarItem(string item)
    {
        Carregando = true;
        await Task.Delay(50);

        ConteudoAtual = item switch
        {
            "inicio" => _services.GetRequiredService<HomeView>(),
            "entrada" => _services.GetRequiredService<EntradaView>(),
            "saida" => _services.GetRequiredService<SaidaView>(),
            "estoque_alimentos" => CriarEstoqueView(Categoria.Alimentos),
            "estoque_limpeza" => CriarEstoqueView(Categoria.Limpeza),
            "estoque_almoxarifado" => CriarEstoqueView(Categoria.Almoxarifado),
            "relatorios" => _services.GetRequiredService<RelatorioView>(),
            _ => null
        };

        await Task.Delay(50);
        Carregando = false;
    }

    private EstoqueView CriarEstoqueView(Categoria categoria)
    {
        var vm = _services.GetRequiredService<EstoqueViewModel>();
        vm.CarregarCategoria(categoria);
        return new EstoqueView(vm);
    }
}