using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Enums
{
    internal static class CategoriaExtension
    {
        public static bool TemValidade(this Categoria c) => c switch
        {
            Categoria.Alimentos => true,
            _ => false
        };
    }
}
