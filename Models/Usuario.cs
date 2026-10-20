using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public bool Admin { get; set; }
        public required string Hash { get; set; }
        public required string Salt { get; set; }

    }
}
