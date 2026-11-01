using CommunityToolkit.Mvvm.ComponentModel;
using EstoqueAsilo.Services;
using System.Collections.ObjectModel;

namespace EstoqueAsilo.ViewModels
{
    public partial class RelatorioViewModel : ObservableObject
    {
        private readonly ConsultaService _consulta;
        public ObservableCollection<BarraRelatorio> ConsumoInterno { get; } = [];
        public ObservableCollection<BarraRelatorio> OrigemEntradas { get; } = [];
        public ObservableCollection<DoacaoEnviadaLinha> DoacoesEnviadas { get; } = [];

        public RelatorioViewModel(ConsultaService consulta)
        {
            _consulta = consulta;
            CarregarDados();
        }

        private void CarregarDados()
        {
            var saidasConsumo = _consulta.ListarSaidasConsumo().ToList();
            var consumoAgrupado = saidasConsumo
                .GroupBy(s => s.Produto.Nome)
                .Select(g => new { Nome = g.Key, Total = g.Sum(s => s.Quantidade) })
                .OrderByDescending(x => x.Total)
                .Take(4)
                .ToList();

            int maxConsumo = consumoAgrupado.Any() ? consumoAgrupado.Max(x => x.Total) : 1;

            foreach (var item in consumoAgrupado)
            {
                ConsumoInterno.Add(new BarraRelatorio
                {
                    Nome = item.Nome,
                    Valor = item.Total,
                    PorcentagemBarra = (double)item.Total / maxConsumo
                });
            }
            int totalDoacoes = _consulta.ListarEntradasDoacao().Sum(e => e.Quantidade);
            int totalCompras = _consulta.ListarEntradasCompra().Sum(e => e.Quantidade);

            if (totalDoacoes > 0 || totalCompras > 0)
            {
                int maxEntradas = Math.Max(totalDoacoes, totalCompras);

                if (totalDoacoes > 0)
                    OrigemEntradas.Add(new BarraRelatorio { Nome = "Doação", Valor = totalDoacoes, PorcentagemBarra = (double)totalDoacoes / maxEntradas });

                if (totalCompras > 0)
                    OrigemEntradas.Add(new BarraRelatorio { Nome = "Compra", Valor = totalCompras, PorcentagemBarra = (double)totalCompras / maxEntradas });
            }
            var doacoesEnviadas = _consulta.ListarSaidasDoacao()
                .OrderByDescending(d => d.Data)
                .Take(5);

            foreach (var d in doacoesEnviadas)
            {
                DoacoesEnviadas.Add(new DoacaoEnviadaLinha
                {
                    Data = d.Data,
                    Item = d.Produto.Nome,
                    Quantidade = d.Quantidade,
                    Destino = d.Contraparte ?? "Não especificado"
                });
            }
        }
    }

    public class BarraRelatorio
    {
        public string Nome { get; set; } = string.Empty;
        public int Valor { get; set; }
        public double PorcentagemBarra { get; set; }
    }

    public class DoacaoEnviadaLinha
    {
        public DateTime Data { get; set; }
        public string Item { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public string Destino { get; set; } = string.Empty;
    }
}