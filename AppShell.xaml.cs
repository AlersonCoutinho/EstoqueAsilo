namespace EstoqueAsilo
{ 
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("mainpage", typeof(MainPage));
        }

    }
}
