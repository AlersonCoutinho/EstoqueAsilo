namespace EstoqueAsilo;

public partial class MenuLateral : ContentView
{

    public MenuLateral()
    {
        InitializeComponent();
    }

    private void Sair1_Clicked(object? sender, EventArgs e)
    {
        Shell.Current.GoToAsync("loginview");
    }
}