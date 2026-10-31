using EstoqueAsilo.Enums;
using EstoqueAsilo.Models;
using EstoqueAsilo.Models.Resultados;
using System.Collections.ObjectModel;

namespace EstoqueAsilo.Services
{
        // Busca de informações
    public class ConsultaService (EstoqueService estoque)
    {
        private readonly EstoqueService _estoque = estoque;

        // -- Listagem de movimentações
        internal IEnumerable<Movimentacao> ListarEntradas()
        {
            return [.. _estoque.Movimentacoes.Where(m => m.TipoMovimentacao == TipoMovimentacao.Entrada)];
        }
        internal IEnumerable<Movimentacao> ListarSaidas()
        {
            return [.. _estoque.Movimentacoes.Where(m => m.TipoMovimentacao == TipoMovimentacao.Saida)];
        }
        internal IEnumerable<Movimentacao> ListarMovimentacoesPorUsuario(Usuario user)
        {
            return [.. _estoque.Movimentacoes.Where(m => m.Usuario.Id == user.Id)];
        }
        internal IEnumerable<Movimentacao> ListarMovimentacoesDeProduto(Produto produto)
        {
            return [.. _estoque.Movimentacoes.Where(m => m.Produto.Id == produto.Id)];
        }
        internal IEnumerable<Movimentacao> ObterEntradasMes()
        {
            return MovimentacoesUltimosNDias(TipoMovimentacao.Entrada, -30);
        }
        internal IEnumerable<Movimentacao> ObterSaidasMes()
        {
            return MovimentacoesUltimosNDias(TipoMovimentacao.Saida, -30);
        }

        // -- Listagem de produtos
        internal IEnumerable<Produto> ListarProdutos()
        {
            return [.. _estoque.Produtos];
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
            return [.. _estoque.Produtos.Where(p => p.QuantidadeMinima is not null && CalcularQuantidadeDeProduto(p) <= p.QuantidadeMinima)];
        }

        // -- Listagem de Produtos com vencimento
        internal IEnumerable<Lote> ListarLotesDeProduto(Produto produto)
        {
            return [.. _estoque.Lotes.Where(l => l.Produto.Id == produto.Id)];
        }
        internal IEnumerable<DateTime> ListarValidadesDeProduto(Produto produto)
        {
            return [.. _estoque.Lotes.Where(l => l.Produto.Id == produto.Id).Select(l => l.DataValidade)];
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
            return [.. _estoque.Lotes
                .Where(l => (min is null || l.DataValidade >= min) && (max is null || l.DataValidade <= max))
                .Select(l => new VencimentoInfo(
                    l.Produto, 
                    l.DataValidade,
                    RestantePorLote().GetValueOrDefault(l.Id),
                    (l.DataValidade - DateTime.Today).Days))
                .Where(v => v.QuantidadeRestante > 0)
                .OrderBy(v => v.DataValidade)];
        }

        // -- Listagem de produtos com atenção
        internal ObservableCollection<AtencaoLinha> ListarProdutosComAtencao()
        {
            var produtosBaixa = ListarProdutosComQuantidadeBaixa()
                .Select(p => new AtencaoLinha(
                    p.Nome,
                    CalcularQuantidadeDeProduto(p),
                    "Quantidade baixa"));
            var produtosVencidos = ListarProdutosVencidos()
                .Select(v => new AtencaoLinha(
                    v.Produto.Nome,
                    v.QuantidadeRestante,
                    "Produto vencido"));
            var produtosVencendo = ListarVencimentosNoTempoLimite()
                .Select(v => new AtencaoLinha(
                    v.Produto.Nome,
                    v.QuantidadeRestante,
                    $"Vence em {v.TempoRestanteEmDias} dias"));
            return [.. produtosBaixa.Concat(produtosVencidos).Concat(produtosVencendo)];
        }

        // -- Listagem de movimentações filtradas por doação
        internal IEnumerable<Movimentacao> ListarEntradasDoacao()
        {
            return ListarMovimentacoesPorDoacao(TipoMovimentacao.Entrada, true);
        }
        internal IEnumerable<Movimentacao> ListarEntradasCompra()
        {
            return ListarMovimentacoesPorDoacao(TipoMovimentacao.Entrada, false);
        }
        internal IEnumerable<Movimentacao> ListarSaidasDoacao()
        {
            return ListarMovimentacoesPorDoacao(TipoMovimentacao.Saida, true);
        }
        internal IEnumerable<Movimentacao> ListarSaidasConsumo()
        {
            return ListarMovimentacoesPorDoacao(TipoMovimentacao.Saida, false);
        }
        private IEnumerable<Movimentacao> ListarMovimentacoesPorDoacao(TipoMovimentacao tipo, bool isDoacao)
        {
            return [.. _estoque.Movimentacoes.Where(m => m.TipoMovimentacao == tipo && m.IsDoacao == isDoacao)];
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
            return [.. _estoque.Movimentacoes
                .Where(m => (m.TipoMovimentacao == tipo && m.Data >= DateTime.Today.AddDays(n)))];
        }
    }
}
