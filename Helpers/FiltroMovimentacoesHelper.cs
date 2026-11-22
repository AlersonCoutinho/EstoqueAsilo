using EstoqueAsilo.Models.Resultados;

namespace EstoqueAsilo.Helpers
{
    internal static class FiltroMovimentacoesHelper
    {
        internal static IEnumerable<MovimentacaoLinha> FiltrarMovimentacoes(IEnumerable<MovimentacaoLinha> movimentacoes, 
            string termoBusca, string usuarioFiltro, string tipoFiltro)
        {
            var consulta = movimentacoes;
            if (!string.IsNullOrWhiteSpace(termoBusca))
                consulta = consulta.Where(e => (e.Item.Contains(termoBusca, StringComparison.OrdinalIgnoreCase) ||
                (e.Contraparte != null && e.Contraparte.Contains(termoBusca, StringComparison.OrdinalIgnoreCase))));
            if (usuarioFiltro != "Todos")
                consulta = consulta.Where(e => e.Usuario == usuarioFiltro);
            if (tipoFiltro == "Doação")
                consulta = consulta.Where(e => e.IsDoacao);
            else if (tipoFiltro == "Compra" || tipoFiltro == "Consumo")
                consulta = consulta.Where(e => !e.IsDoacao);
            return consulta;
        }
    }
}
