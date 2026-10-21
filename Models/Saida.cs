using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Models
{
    public class Saida
    {
        public required int Id { get; set; }
        public required bool Doacao { get; set; }
        public required decimal Quantidade { get; set; }
        public string? Doado { get; set; }
    }
}
