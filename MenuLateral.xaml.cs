namespace protótipo_acelera;

public partial class MenuLateral : ContentView
{

    public event EventHandler<string>? ItemSelecionado;

	public MenuLateral()
	{
		InitializeComponent();
	}

    private void inicio1_Clicked(object? sender, EventArgs e)
    {
        ItemSelecionado?.Invoke(this, "inicio");
    }

    private void Entrada1_Clicked(object? sender, EventArgs e)
    {
        ItemSelecionado?.Invoke(this, "entrada");
    }

    private void Saida1_Clicked(object? sender, EventArgs e)
    {
        ItemSelecionado?.Invoke(this, "saida");
    }

    private void Sair1_Clicked(object? sender, EventArgs e)
    {
        Shell.Current.GoToAsync("mainpage");
    }

    private void Estoque1_Clicked(object? sender, EventArgs e)
    {
        ItemSelecionado?.Invoke(this, "estoque");
    }
}