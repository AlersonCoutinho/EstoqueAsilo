namespace EstoqueAsilo.Models.Resultados
{
    public record ProdutoCompradoLinha
    {
        public string Item { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public string Preco { get; set; } = string.Empty;
    }
}
