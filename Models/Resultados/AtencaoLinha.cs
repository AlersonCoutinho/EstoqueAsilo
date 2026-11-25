using EstoqueAsilo.Enums;

namespace EstoqueAsilo.Models.Resultados
{
    public record AtencaoLinha
    (
        string Item,
        int Quantidade,
        SituacaoEstoque Situacao
    );
}
