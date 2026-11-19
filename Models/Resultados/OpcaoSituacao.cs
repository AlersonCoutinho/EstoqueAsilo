using EstoqueAsilo.Enums;

namespace EstoqueAsilo.Models.Resultados
{
    public record OpcaoSituacao(string Texto, SituacaoEstoque? Valor);
}
