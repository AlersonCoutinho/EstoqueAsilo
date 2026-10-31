using System.Globalization;

namespace EstoqueAsilo.Converters;

public class BoolToStringConverter : IValueConverter
{
    // Formate de parametro: "texto se true|texto se false"
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var partes = (parameter as string)?.Split('|');
        if (partes is not { Length: 2 }) return string.Empty;

        return value is true ? partes[0] : partes[1];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
