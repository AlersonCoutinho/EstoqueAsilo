using EstoqueAsilo.Services;
using EstoqueAsilo.ViewModels;
using EstoqueAsilo.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

#if WINDOWS
using Microsoft.UI.Windowing;
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
                }));
#endif
            });

            return builder.Build();
        }
    }
}
