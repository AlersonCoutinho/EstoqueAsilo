namespace EstoqueAsilo.Models
{
    public class Lote
    {
        public required int Id { get; set; }
        public required Produto Produto { get; set; }
        public required DateTime DataValidade { get; set; }
        public required int QuantidadeInicial { get; set; }
    }
}
