using Microsoft.Extensions.Logging;
using EstoqueAsilo.ViewModels;
using EstoqueAsilo.Services;

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
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<EntradaView>();
            builder.Services.AddTransient<EntradaViewModel>();
            builder.Services.AddTransient<BarraSuperior>();
            builder.Services.AddSingleton<ServicoEstoque>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
