namespace EstoqueAsilo.Models.Resultados
{
    public record VencimentoInfo
    (
        Produto Produto,
        DateTime DataValidade,
        int QuantidadeRestante,
        int TempoRestanteEmDias
    );
}
