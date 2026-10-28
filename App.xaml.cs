using EstoqueAsilo.Services;

namespace EstoqueAsilo
{
    public partial class App : Application
    {
        private readonly EstoqueService _estoque;
        private readonly AuthService _auth;
        public App(EstoqueService estoque, AuthService auth)
        {
            InitializeComponent();
            _estoque = estoque;
            _estoque.Carregar();
            _auth = auth;
            _ = _auth.Carregar();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell())
            {
                MinimumWidth = 800,
                MinimumHeight = 500,
                Width = 1280,
                Height = 720,
            };
            window.Destroying += (_, _) =>
            {
                _estoque.Salvar();
                _auth.Salvar();
            };
            return window;
        }
    }
}