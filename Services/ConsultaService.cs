using EstoqueAsilo.Enums;
using EstoqueAsilo.Models;
using EstoqueAsilo.Models.Resultados;
using System.Collections.ObjectModel;

namespace EstoqueAsilo.Services
{
    // Busca de informações
    public class ConsultaService(EstoqueService estoque)
    {
        private readonly EstoqueService _estoque = estoque;

        // -- Listagem de movimentações
        internal IEnumerable<Movimentacao> ListarEntradas()
        {
            return _estoque.Movimentacoes.Where(m => m.TipoMovimentacao == TipoMovimentacao.Entrada).ToList();
        }
        internal IEnumerable<Movimentacao> ListarSaidas()
        {
            return _estoque.Movimentacoes.Where(m => m.TipoMovimentacao == TipoMovimentacao.Saida).ToList();
        }
        internal IEnumerable<Movimentacao> ObterEntradasMes()
        {
            return MovimentacoesUltimosNDias(TipoMovimentacao.Entrada, -30);
        }
        internal IEnumerable<Movimentacao> ObterSaidasMes()
        {
            return MovimentacoesUltimosNDias(TipoMovimentacao.Saida, -30);
        }

        // -- Listagem objetos, variáveis e propriedades
        internal IEnumerable<string> ListarUnidades()
        {
            return _estoque.Produtos
                .Select(p => p.Unidade).Distinct().OrderBy(u => u).ToList();
        }
        internal IEnumerable<Produto> ListarProdutos()
        {
            return _estoque.Produtos
                .Distinct().OrderBy(p => p.Nome).ToList();
        }
        internal IEnumerable<string> ListarNomeProdutos()
        {
            return _estoque.Produtos
                .Select(p => p.Nome).Distinct().OrderBy(p => p).ToList();
        }
        internal IEnumerable<Produto> ListarProdutosPorCategoria(Categoria categoria)
        {
            return _estoque.Produtos
                .Where(p => p.Categoria == categoria)
                .OrderBy(p => p.Nome, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }
        internal IEnumerable<string> ListarUnidadesProduto(string nomeProduto)
        {
            if (string.IsNullOrWhiteSpace(nomeProduto))
                return [];

            return _estoque.Produtos
                .Where(p => p.Nome.Equals(nomeProduto.Trim(), StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Unidade)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(u => u).ToList();
        }
        internal Produto? ProdutoPorNome(string nomeProduto, string unidade)
        {
            if (nomeProduto == null || unidade == null) return null;
            return _estoque.Produtos.FirstOrDefault(p =>
                p.Nome.Equals(nomeProduto.Trim(), StringComparison.OrdinalIgnoreCase) &&
                p.Unidade.Equals(unidade.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        // -- Listagem de quantidade de produtos
        internal int CalcularQuantidadeDeProduto(Produto produto)
        {
            return _estoque.Movimentacoes.Where(m => m.Produto.Id == produto.Id)
                .Sum(m => m.QuantidadeComSinal);
        }
        internal int CalcularQuantidadeDeProdutoEmLote(Lote lote)
        {
            return _estoque.Movimentacoes.Where(m => m.Lote?.Id == lote.Id)
                .Sum(m => m.QuantidadeComSinal);
        }

        // -- Listagem de produtos com quantidade baixa
        internal IEnumerable<Produto> ListarProdutosComQuantidadeBaixa()
        {
            return _estoque.Produtos.Where(p =>
            p.QuantidadeMinima is not null && CalcularQuantidadeDeProduto(p) <= p.QuantidadeMinima).ToList();
        }

        // -- Listagem de Produtos com vencimento
        internal IEnumerable<Lote> ListarLotesDeProduto(Produto produto)
        {
            return _estoque.Lotes.Where(l => l.Produto.Id == produto.Id)
                .OrderBy(l => l.DataValidade).ToList();
        }

        // -- Proximo Lote a vencer de um produto
        internal List<LoteSugerido> SugerirLotes(Produto produto, int falta)
        {
            var restante = RestantePorLote();
            var sugeridos = new List<LoteSugerido>();

            var lotes = ListarLotesDeProduto(produto);

            foreach (var lote in lotes)
            {
                if (falta <= 0) break;

                int disponivel = restante.GetValueOrDefault(lote.Id);
                if (disponivel <= 0) continue;

                int retirar = Math.Min(disponivel, falta);
                sugeridos.Add(new LoteSugerido(lote, disponivel, retirar));
                falta -= retirar;
            }

            return sugeridos;
        }

        internal IEnumerable<DateTime> ListarValidadesDeProduto(Produto produto)
        {
            return _estoque.Lotes.Where(l => l.Produto.Id == produto.Id).Select(l => l.DataValidade).ToList();
        }

        // -- Listagem de Produtos organizados por vencimento
        internal IEnumerable<VencimentoInfo> ListarProdutosVencidos()
        {
            return VencimentosEntre(null, DateTime.Today.AddDays(-1));
        }
        internal IEnumerable<VencimentoInfo> ListarVencimentosNoTempoLimite(int dias = 30)
        {
            return VencimentosEntre(DateTime.Today, DateTime.Today.AddDays(dias));
        }
        internal IEnumerable<VencimentoInfo> VencimentosEntre(DateTime? min, DateTime? max)
        {
            var restante = RestantePorLote();
            return _estoque.Lotes
                .Where(l => (min is null || l.DataValidade >= min) && (max is null || l.DataValidade <= max))
                .Select(l => new VencimentoInfo(
                    l.Produto,
                    l.DataValidade,
                    restante.GetValueOrDefault(l.Id),
                    (l.DataValidade - DateTime.Today).Days))
                .Where(v => v.QuantidadeRestante > 0)
                .OrderBy(v => v.DataValidade).ToList();
        }

        // -- Listagem de produtos com atenção
        internal ObservableCollection<AtencaoLinha> ListarProdutosComAtencao(IEnumerable<Produto>? produtos = null)
        {
            produtos ??= ListarProdutos();
            ObservableCollection<AtencaoLinha> produtosAtencao = [];

            foreach (var p in produtos)
            {
                int quantidadeAtual = CalcularQuantidadeDeProduto(p);

                var validades = p.Categoria.TemValidade() ? ListarValidadesDeProduto(p).ToList() : [];
                DateTime? proxima = validades.Count != 0 ? validades.Min() : null;

                var situacao = SituacaoEstoque.Nenhuma;
                if (p.QuantidadeMinima is not null && quantidadeAtual <= p.QuantidadeMinima)
                    situacao |= SituacaoEstoque.EstoqueBaixo;
                if (proxima is { } data)
                {
                    if (data < DateTime.Today)
                        situacao |= SituacaoEstoque.Vencido;
                    else if (data <= DateTime.Today.AddDays(30))
                        situacao |= SituacaoEstoque.VenceEmBreve;
                }

                if (situacao != SituacaoEstoque.Nenhuma)
                {
                    produtosAtencao.Add(new AtencaoLinha(
                        $"{p.Nome} {p.Unidade}",
                        quantidadeAtual,
                        situacao,
                        situacao.ParaTexto()));
                }
            }
            return produtosAtencao;
        }

        // -- Listagem de movimentações filtradas por doação
        internal IEnumerable<Movimentacao> ListarEntradasDoacao()
        {
            return ListarMovimentacoesPorDoacao(TipoMovimentacao.Entrada, true);
        }
        internal IEnumerable<Movimentacao> ListarEntradasDoacaoMes(int m = 0, int a = 0)
        {
            return FiltrarMovimentacoesPorMes(ListarEntradasDoacao(), m, a);
        }
        internal IEnumerable<Movimentacao> ListarEntradasCompra()
        {
            return ListarMovimentacoesPorDoacao(TipoMovimentacao.Entrada, false);
        }
        internal IEnumerable<Movimentacao> ListarEntradasCompraMes(int m = 0, int a = 0)
        {
            return FiltrarMovimentacoesPorMes(ListarEntradasCompra(), m, a);
        }
        internal decimal ListarPrecoCompraMes(int m = 0, int a = 0)
        {
            return ListarEntradasCompraMes(m, a).Sum(s => s.Preco ?? 0);
        }
        internal IEnumerable<Movimentacao> ListarSaidasDoacao()
        {
            return ListarMovimentacoesPorDoacao(TipoMovimentacao.Saida, true);
        }
        internal IEnumerable<Movimentacao> ListarSaidasDoacaoMes(int m = 0, int a = 0)
        {
            return FiltrarMovimentacoesPorMes(ListarSaidasDoacao(), m, a);
        }
        internal IEnumerable<Movimentacao> ListarSaidasConsumo()
        {
            return ListarMovimentacoesPorDoacao(TipoMovimentacao.Saida, false).OrderByDescending(d => d.Data);
        }
        internal IEnumerable<Movimentacao> ListarSaidasConsumoMes(int m = 0, int a = 0)
        {
            return FiltrarMovimentacoesPorMes(ListarSaidasConsumo(), m, a);
        }
        private IEnumerable<Movimentacao> ListarMovimentacoesPorDoacao(TipoMovimentacao tipo, bool isDoacao)
        {
            return _estoque.Movimentacoes.Where(m => m.TipoMovimentacao == tipo && m.IsDoacao == isDoacao).ToList();
        }

        // Métodos auxiliares
        private Dictionary<int, int> RestantePorLote() =>
            _estoque.Movimentacoes
                .Where(m => m.Lote is not null)
                .GroupBy(m => m.Lote!.Id)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(m => m.QuantidadeComSinal));

        private IEnumerable<Movimentacao> MovimentacoesUltimosNDias(TipoMovimentacao tipo, int n)
        {
            return _estoque.Movimentacoes
                .Where(m => (m.TipoMovimentacao == tipo && m.Data >= DateTime.Today.AddDays(n))).ToList();
        }

        private IEnumerable<Movimentacao> FiltrarMovimentacoesPorMes(IEnumerable<Movimentacao> movimentacoes, int mes, int ano)
        {
            var inicio = new DateTime(
                ano == 0 ? DateTime.Today.Year : ano,
                mes == 0 ? DateTime.Today.Month : mes,
                1);
            var fim = inicio.AddMonths(1);
            return movimentacoes.Where(s => s.Data >= inicio && s.Data < fim);
        }
    }
}
