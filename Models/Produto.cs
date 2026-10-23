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
        public required string Unidade { get; set; }
        public List<DateTime>? DatasValidade { get; set; }
    }
}
