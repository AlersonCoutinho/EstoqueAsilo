using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Models
{
    public class Entrada
    {
        public required int Id { get; set; }
        public required bool Doacao { get; set; }
        public required int Quantidade { get; set; }
        public string? Doador { get; set; }
        public string? Fornecedor { get; set; }
        public decimal? Preco { get; set; }
        public string? IdNotaFiscal { get; set; }
    }
}
