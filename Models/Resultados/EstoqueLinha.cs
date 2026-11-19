using EstoqueAsilo.Enums;

namespace EstoqueAsilo.Models.Resultados
{
    public class EstoqueLinha(string nome, string quantidade, string minimo, string validade, SituacaoEstoque situacao)
    {
        public string Nome { get; set; } = nome;
        public string Quantidade { get; set; } = quantidade;
        public string Minimo { get; set; } = minimo;
        public string Validade { get; set; } = validade;
        public SituacaoEstoque Situacao { get; set; } = situacao;
        public string SituacaoTexto => Situacao.ParaTexto();
    }
}