using CommunityToolkit.Mvvm.ComponentModel;
using EstoqueAsilo.Models.Resultados;
using EstoqueAsilo.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace EstoqueAsilo.ViewModels
{
    public partial class RelatorioViewModel : ObservableObject
    {
        private static readonly CultureInfo PtBr = new("pt-BR");
        private readonly ConsultaService _consulta;
        public ObservableCollection<BarraRelatorio> OrigemSaidas { get; } = [];
        [ObservableProperty]
        public partial int TotalSaidas { get; set; } = 0;
        public ObservableCollection<BarraRelatorio> OrigemEntradas { get; } = [];
        [ObservableProperty]
        public partial int TotalEntradas { get; set; } = 0;
        public ObservableCollection<ProdutoConsumidoLinha> ProdutosConsumidos { get; } = [];
        public ObservableCollection<ProdutoCompradoLinha> ProdutosComprados { get; } = [];
        [ObservableProperty]
        public partial string TotalPreco { get; set; } = String.Empty;

        public RelatorioViewModel(ConsultaService consulta)
        {
            _consulta = consulta;
            CarregarDados();
        }

        private void CarregarDados()
        {
            // -- Relatório origem das entradas
            int totalDoacoesEnt = _consulta.ListarEntradasDoacaoMes().Sum(e => e.Quantidade);
            int totalCompras = _consulta.ListarEntradasCompraMes().Sum(e => e.Quantidade);
            TotalEntradas = totalDoacoesEnt + totalCompras;
            OrigemEntradas.Add(new BarraRelatorio
            {
                Nome = "Compra",
                Valor = totalCompras,
                PorcentagemBarra = (double)totalCompras / TotalEntradas
            });
            OrigemEntradas.Add(new BarraRelatorio
            {
                Nome = "Doação",
                Valor = totalDoacoesEnt,
                PorcentagemBarra = (double)totalDoacoesEnt / TotalEntradas
            });


            // -- Relatorio destino de saidas
            int totalDoacoesSai = _consulta.ListarSaidasDoacaoMes().Sum(e => e.Quantidade);
            int totalConsumo = _consulta.ListarSaidasConsumoMes().Sum(e => e.Quantidade);
            TotalSaidas = totalDoacoesSai + totalConsumo;
            OrigemSaidas.Add(new BarraRelatorio
            {
                Nome = "Consumo",
                Valor = totalConsumo,
                PorcentagemBarra = (double)totalConsumo / TotalSaidas
            });
            OrigemSaidas.Add(new BarraRelatorio
            {
                Nome = "Doação",
                Valor = totalDoacoesSai,
                PorcentagemBarra = (double)totalDoacoesSai / TotalSaidas
            });
            /*var saidasConsumo = _consulta.ListarSaidasConsumo().ToList();
            var consumoAgrupado = saidasConsumo
                .GroupBy(s => s.Produto.Nome)
                .Select(g => new { Nome = g.Key, Total = g.Sum(s => s.Quantidade) })
                .OrderByDescending(x => x.Total)
                .Take(4)
                .ToList();

            int maxConsumo = consumoAgrupado.Count!=0 ? consumoAgrupado.Max(x => x.Total) : 1;

            foreach (var item in consumoAgrupado)
            {
                ConsumoInterno.Add(new BarraRelatorio
                {
                    Nome = item.Nome,
                    Valor = item.Total,
                    PorcentagemBarra = (double)item.Total / maxConsumo
                });
            }*/

            // -- Relatório itens consumidos
            var consumos = _consulta.ListarSaidasConsumoMes()
                .GroupBy(c => c.Produto.Id)
                .Select(g => new ProdutoConsumidoLinha
                {
                    Item = $"{g.First().Produto.Nome} {g.First().Produto.Unidade}",
                    Quantidade = g.Sum(c => c.Quantidade)
                })
                .OrderBy(l => l.Item, StringComparer.CurrentCultureIgnoreCase);

            foreach (var c in consumos)
            {
                ProdutosConsumidos.Add(c);
            }

            // -- Relatório itens comprados
            var compras = _consulta.ListarEntradasCompraMes()
                .GroupBy(c => c.Produto.Id)
                .Select(g => new ProdutoCompradoLinha
                {
                    Item = $"{g.First().Produto.Nome} {g.First().Produto.Unidade}",
                    Quantidade = g.Sum(c => c.Quantidade),
                    Preco = g.Sum(c => c.Preco ?? 0).ToString("C2", PtBr)
                })
                .OrderBy(l => l.Item, StringComparer.CurrentCultureIgnoreCase);

            foreach (var c in compras)
            {
                ProdutosComprados.Add(c);
            }
            TotalPreco = _consulta.ListarPrecoCompraMes().ToString("C2", PtBr);
        }
    }

    public class BarraRelatorio
    {
        public string Nome { get; set; } = string.Empty;
        public int Valor { get; set; }
        public double PorcentagemBarra { get; set; }
        public GridLength LarguraPreenchida => new(Math.Clamp(PorcentagemBarra, 0, 1), GridUnitType.Star);
        public GridLength LarguraVazia => new(1 - Math.Clamp(PorcentagemBarra, 0, 1), GridUnitType.Star);
    }
}