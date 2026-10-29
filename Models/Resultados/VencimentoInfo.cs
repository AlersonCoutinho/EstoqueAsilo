namespace EstoqueAsilo.Models.Resultados
{
    internal record VencimentoInfo
    (
        Produto Produto,
        DateTime DataValidade,
        int QuantidadeRestante,
        int TempoRestanteEmDias
    );
}
