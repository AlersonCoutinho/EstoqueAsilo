using System.Globalization;

namespace EstoqueAsilo.Behaviors
{
    internal partial class MascaraMoedaBehavior : Behavior<Entry>
    {
        private static readonly CultureInfo Cultura = new("pt-BR");
        private Entry? _entry;
        private bool _atualizando;

        public static readonly BindableProperty ValorProperty = BindableProperty.Create(
            nameof(Valor), typeof(decimal), typeof(MascaraMoedaBehavior), 0m,
            BindingMode.TwoWay, propertyChanged: OnValorChanged);

        public decimal Valor
        {
            get => (decimal)GetValue(ValorProperty);
            set => SetValue(ValorProperty, value);
        }

        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);
            _entry = entry;

            BindingContext = entry.BindingContext;
            entry.BindingContextChanged += OnEntryBindingContextChanged;

            entry.TextChanged += OnTextChanged;
            Mostrar(Valor);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnTextChanged;
            entry.BindingContextChanged -= OnEntryBindingContextChanged;
            _entry = null;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryBindingContextChanged(object? sender, EventArgs e) =>
            BindingContext = _entry?.BindingContext;

        private void OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (_atualizando || _entry is null) return;

            var digitos = new string([.. (e.NewTextValue ?? "").Where(char.IsDigit)]);
            if (digitos.Length > 12) digitos = digitos[..12];

            var valor = digitos.Length == 0
                ? 0m
                : decimal.Parse(digitos, CultureInfo.InvariantCulture) / 100m;

            _atualizando = true;
            Valor = valor;
            _atualizando = false;

            Mostrar(valor);
        }

        private static void OnValorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var b = (MascaraMoedaBehavior)bindable;
            if (!b._atualizando) b.Mostrar((decimal)newValue);
        }

        private void Mostrar(decimal valor)
        {
            if (_entry is null) return;

            var texto = valor == 0 ? "R$ 0,00" : valor.ToString("C", Cultura);   // R$ 2,50

            _atualizando = true;
            if (_entry.Text != texto)
            {
                _entry.Text = texto;
            }
            _atualizando = false;

            MoverCursorParaOFim();
        }

        private void MoverCursorParaOFim()
        {
            var entry = _entry;
            if (entry is null) return;

            entry.Dispatcher.Dispatch(() =>
            {
                entry.CursorPosition = entry.Text?.Length ?? 0;
                entry.SelectionLength = 0;
            });
        }
    }
}
