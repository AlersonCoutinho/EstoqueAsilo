using EstoqueAsilo.Services;
using EstoqueAsilo.ViewModels;
using Microsoft.Extensions.Logging;

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
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ConsultaService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
