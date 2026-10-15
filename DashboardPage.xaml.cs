namespace protótipo_acelera;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
        AreaConteudo.Content = new HomeView();
        MenuLateralControl.ItemSelecionado += OnItemSelecionado;
    }
    private void OnItemSelecionado(object? sender, string item)
    {
        AreaConteudo.Content = item switch
        {
            "inicio" => new HomeView(),
            "entrada" => new EntradaView(),
            "saida" => new SaidaView(),
            "estoque" => new EstoqueView(),
            _ => null
        };
    }
}