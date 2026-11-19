using EstoqueAsilo.Enums;
using System.Globalization;

namespace EstoqueAsilo.Converters
{
    public class SituacaoColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not SituacaoEstoque s) return Colors.Black;

            if (s.HasFlag(SituacaoEstoque.Vencido))
                return Colors.Red;

            if (s.HasFlag(SituacaoEstoque.VenceEmBreve) || s.HasFlag(SituacaoEstoque.EstoqueBaixo))
                return Colors.Orange;

            return Color.FromArgb("#2E8B57");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}