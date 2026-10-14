using Microsoft.Extensions.DependencyInjection;

namespace protótipo_acelera
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell())
            {
                MinimumWidth = 800,
                MinimumHeight = 500,
                Width = 1280,
                Height = 720
            };
            return window;
        }
    }
}