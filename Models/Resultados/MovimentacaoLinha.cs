namespace EstoqueAsilo.Models.Resultados
{
    public record MovimentacaoLinha
    (
        DateTime Data,
        string Item,
        bool IsDoacao,
        string? Contraparte,
        int Quantidade,
        string Usuario
    );
}
