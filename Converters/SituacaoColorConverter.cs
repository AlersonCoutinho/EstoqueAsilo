using System.Globalization;

namespace EstoqueAsilo.Converters
{
    public class SituacaoColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string situacao = value as string ?? string.Empty;

            if (situacao.Contains("Vencido"))
                return Colors.Red;

            if (situacao.Contains("Vence em breve") || situacao.Contains("Estoque Baixo"))
                return Colors.Orange;

            if (situacao.Contains("OK"))
                return Color.FromArgb("#2E8B57");

            return Colors.Black;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}