namespace EstoqueAsilo.Models.Resultados
{
    public class EstoqueLinha
    {
        public string Nome { get; set; }
        public string Quantidade { get; set; }
        public string Minimo { get; set; }
        public string Validade { get; set; }
        public string Situacao { get; set; }

        public EstoqueLinha(string nome, string quantidade, string minimo, string validade, string situacao)
        {
            Nome = nome;
            Quantidade = quantidade;
            Minimo = minimo;
            Validade = validade;
            Situacao = situacao;
        }
    }
}