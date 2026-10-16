namespace EstoqueAsilo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void Clicado(object? sender, EventArgs e)
        {
            string email = Caixa1.Text;
            string senha = Caixa2.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                await DisplayAlertAsync("Ops", "Você não preencheu os campos corretamente", "Ok");
            }else{
                await Shell.Current.GoToAsync("dashboard");
            }
            
        }
    }
}
