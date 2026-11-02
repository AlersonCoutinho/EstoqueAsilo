using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Models.Resultados
{
    internal record LoteSugerido
    (
        Lote Lote, 
        int Disponivel, 
        int Retirar
    );
}
