using EstoqueAsilo.Enums;

namespace EstoqueAsilo.Models
{
    public class Produto
    {
        public required int Id { get; set; }
        public required Categoria Categoria { get; set; }
        public required string Nome { get; set; }
        public required string Unidade { get; set; }
        public int? QuantidadeMinima { get; set; }
    }
}
