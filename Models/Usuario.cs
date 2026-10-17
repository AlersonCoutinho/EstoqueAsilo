using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Models
{
    public class Usuario
    {
        public required string Nome { get; set; }
        public bool Admin { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }

    }
}
