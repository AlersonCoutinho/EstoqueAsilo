using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Models
{
    public class Senha
    {
        public bool Admin { get; set; }
        public required string Hash { get; set; }
        public required string Salt { get; set; }
    }
}
