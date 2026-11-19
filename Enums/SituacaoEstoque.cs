namespace EstoqueAsilo.Enums
{
    [Flags]
    public enum SituacaoEstoque
    {
        Nenhuma = 0,
        EstoqueBaixo = 1,
        VenceEmBreve = 2,
        Vencido = 4
    }

    internal static class SituacaoEstoqueExtensions
    {
        internal static string ParaTexto(this SituacaoEstoque s)
        {
            if (s == SituacaoEstoque.Nenhuma) return "OK";

            var partes = new List<string>();
            if (s.HasFlag(SituacaoEstoque.Vencido)) partes.Add("Vencido");
            if (s.HasFlag(SituacaoEstoque.VenceEmBreve)) partes.Add("Vence em breve");
            if (s.HasFlag(SituacaoEstoque.EstoqueBaixo)) partes.Add("Estoque Baixo");
            return string.Join(" · ", partes);
        }
    }
}
