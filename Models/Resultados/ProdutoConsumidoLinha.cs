namespace EstoqueAsilo.Models.Resultados
{
    public record ProdutoConsumidoLinha
    {
        public string Item { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}
