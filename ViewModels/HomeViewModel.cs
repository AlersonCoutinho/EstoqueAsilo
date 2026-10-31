using CommunityToolkit.Mvvm.ComponentModel;
using EstoqueAsilo.Models.Resultados;
using EstoqueAsilo.Services;
using System.Collections.ObjectModel;

namespace EstoqueAsilo.ViewModels
{
    public partial class HomeViewModel:ObservableObject
    {
        private readonly ConsultaService _consulta;

        [ObservableProperty]
        public partial int EntradasMes { get; set; }
        [ObservableProperty]
        public partial int SaidasMes { get; set; }
        [ObservableProperty]
        public partial int PrecisaRepor { get; set; }
        [ObservableProperty]
        public partial int QuantVencendoVencido { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<AtencaoLinha> PrecisaAtencao { get; set; }

        public HomeViewModel(ConsultaService consulta){
            _consulta = consulta;

            EntradasMes = _consulta.ObterEntradasMes().Count();
            SaidasMes = _consulta.ObterSaidasMes().Count();
            PrecisaRepor = _consulta.ListarProdutosComQuantidadeBaixa().Count();
            QuantVencendoVencido = _consulta.ListarVencimentosNoTempoLimite().Count() + _consulta.ListarProdutosVencidos().Count();
            PrecisaAtencao = _consulta.ListarProdutosComAtencao();
            
        }
    }
}
