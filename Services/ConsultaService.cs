using EstoqueAsilo.Enums;
using EstoqueAsilo.Models;
using EstoqueAsilo.Models.Resultados;

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
            var hoje = DateTime.Today;

            return [.. _estoque.Lotes
                .Where(l => (min is null || l.DataValidade >= min) && (max is null || l.DataValidade <= max))
                .Select(l => new VencimentoInfo(
                    l.Produto, 
                    l.DataValidade,
                    RestantePorLote().GetValueOrDefault(l.Id),
                    (l.DataValidade - hoje).Days))
                .Where(v => v.QuantidadeRestante > 0)
                .OrderBy(v => v.DataValidade)];
        }

        //Filtrar movimento por se é doação ou não
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
    }
}
