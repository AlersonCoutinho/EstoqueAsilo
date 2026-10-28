using EstoqueAsilo.Enums;

namespace EstoqueAsilo.Models
{
    public class Movimentacao
    {
        public required int Id { get; set; }
        public required TipoMovimentacao TipoMovimentacao { get; set; }
        public required Produto Produto { get; set; }
        public required Usuario Usuario { get; set; }
        public required int Quantidade { get; set; }
        public required DateTime Data { get; set; }
        public bool IsDoacao { get; set; }
        public Lote? Lote { get; set; }
        public string? Contraparte { get; set; }
        public decimal? Preco { get; set; }
        public string? IdNotaFiscal { get; set; }
    }
}
