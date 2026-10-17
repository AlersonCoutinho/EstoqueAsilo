using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Models
{
    public class Produto
    {
        public required int Id { get; set; }
        public required string Categoria { get; set; }
        public required string Nome { get; set; }
        public required decimal Quantidade { get; set; }
        public required string Unidade { get; set; }
        public required bool Doacao { get; set; }

        public DateTime? DataValidade { get; set; }
        public string? Doador { get; set; }
        public decimal? Preco { get; set; }
        public string? IdNotaFiscal { get; set; }
    }
}
