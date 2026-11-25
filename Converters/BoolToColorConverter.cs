using System.Globalization;

namespace EstoqueAsilo.Converters
{
    internal class BoolToColorConverter : IValueConverter
    {

        public Color CorVerdadeiro { get; set; } = Color.FromArgb("#a6192e");
        public Color CorFalso { get; set; } = Color.FromArgb("#04b34f");

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is true ? CorVerdadeiro : CorFalso;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
