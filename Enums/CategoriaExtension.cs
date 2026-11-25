namespace EstoqueAsilo.Enums
{
    internal static class CategoriaExtension
    {
        public static bool TemValidade(this Categoria c) => c switch
        {
            Categoria.Alimentos => true,
            Categoria.Limpeza => true,
            _ => false
        };
    }
}
