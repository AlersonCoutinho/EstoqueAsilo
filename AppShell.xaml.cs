namespace EstoqueAsilo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("loginview", typeof(LoginView));
        }

    }
}
