namespace EstoqueAsilo.Helpers
{
    internal class IdHelper
    {
        public static int NovoId<T>(List<T> lista, Func<T, int> getId) =>
            lista.Count == 0 ? 1 : lista.Max(getId) + 1;
    }
}
