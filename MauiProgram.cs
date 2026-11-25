using EstoqueAsilo.Services;
using EstoqueAsilo.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
#endif

namespace EstoqueAsilo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeView>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<EntradaView>();
            builder.Services.AddTransient<EntradaViewModel>();
            builder.Services.AddTransient<SaidaView>();
            builder.Services.AddTransient<SaidaViewModel>();
            builder.Services.AddTransient<BarraSuperior>();
            builder.Services.AddSingleton<EstoqueService>();
            builder.Services.AddTransient<EstoqueViewModel>();
            builder.Services.AddTransient<EstoqueView>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ConsultaService>();
            builder.Services.AddTransient<RelatorioView>();
            builder.Services.AddTransient<RelatorioViewModel>();
            builder.Services.AddTransient<ConfiguracaoView>();
            builder.Services.AddTransient<ConfiguracaoViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windows => windows.OnWindowCreated(window =>
                {
                    if (window.AppWindow.Presenter is OverlappedPresenter presenter)
                        presenter.Maximize();

                    var titleBar = window.AppWindow.TitleBar;

                    titleBar.ButtonForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.Gray;

                    titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 229, 229, 229);

                    titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 204, 204, 204);
                }));
#endif
            });

            return builder.Build();
        }
    }
}
