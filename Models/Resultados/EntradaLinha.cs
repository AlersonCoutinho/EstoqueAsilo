namespace EstoqueAsilo.Models.Resultados
{
    public record EntradaLinha
    (
        DateTime Data,
        string Item,
        string TipoOrigem,
        string Origem,
        int Quantidade
    );
}
