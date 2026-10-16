using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EstoqueAsilo.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private View? conteudoAtual;

    [RelayCommand]
    private void SelecionarItem(string item)
    {
        ConteudoAtual = item switch
        {
            "inicio" => new HomeView(),
            "entrada" => new EntradaView(),
            "saida" => new SaidaView(),
            "estoque" => new EstoqueView(),
            _ => null
        };
    }
}