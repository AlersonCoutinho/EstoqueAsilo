using EstoqueAsilo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EstoqueAsilo
{ 
    public partial class App : Application
    {
        private readonly ServicoEstoque _estoque;
        public App(ServicoEstoque estoque)
        {
            InitializeComponent();
            _estoque = estoque;
            _estoque.Carregar();
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
            window.Destroying += (_, _) => _estoque.Salvar();
            return window;
        }
    }
}