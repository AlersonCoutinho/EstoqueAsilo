namespace EstoqueAsilo.Views.Components;

public partial class ComboEditavel : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IEnumerable<string>), typeof(ComboEditavel));

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(ComboEditavel), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(ComboEditavel), string.Empty);

    public IEnumerable<string>? ItemsSource
    {
        get => (IEnumerable<string>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public ComboEditavel() => InitializeComponent();

    private void Filtrar(string? texto)
    {
        var itens = ItemsSource ?? [];
        texto = texto?.Trim() ?? "";

        var resultado = string.IsNullOrEmpty(texto)
            ? [.. itens]
            : itens.Where(i => i.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToList();

        Sugestoes.ItemsSource = resultado;
        Lista.IsVisible = resultado.Count > 0;
    }

    private void OnTextChanged(object? s, TextChangedEventArgs e)
    {
        if (CampoEntry.IsFocused) Filtrar(e.NewTextValue);
    }

    private void OnFocused(object? s, FocusEventArgs e) => Filtrar(CampoEntry.Text);

    private async void OnUnfocused(object? s, FocusEventArgs e)
    {
        await Task.Delay(200);          // lets a tap on a suggestion register first
        Lista.IsVisible = false;
    }

    private void OnSetaClicked(object? s, EventArgs e)
    {
        if (Lista.IsVisible) Lista.IsVisible = false;
        else Filtrar("");               // arrow shows the full list
    }

    private void OnSelecionado(object? s, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0 || e.CurrentSelection[0] is not string item) return;
        Text = item;
        Lista.IsVisible = false;
        Sugestoes.SelectedItem = null;
    }
}