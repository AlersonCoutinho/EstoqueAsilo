using System.Windows.Input;

namespace EstoqueAsilo.Views.Components;

public partial class BorderBotao : ContentView
{
    public event EventHandler? Clicked;

    public static readonly BindableProperty EstiloProperty = BindableProperty.Create(
        nameof(Estilo), typeof(Style), typeof(BorderBotao), 
        null, propertyChanged: (b, _, novo) => ((BorderBotao)b).Caixa.Style = (Style?)novo);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(BorderBotao));

    public static readonly BindableProperty ImageProperty = BindableProperty.Create(
        nameof(Image), typeof(string), typeof(BorderBotao));

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(BorderBotao), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(BorderBotao), null);

    public Style Estilo {
        get => (Style)GetValue(EstiloProperty);
        set => SetValue(EstiloProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Image
    {
        get => (string)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public BorderBotao() 
		=> InitializeComponent();

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);

        Clicked?.Invoke(this, EventArgs.Empty);
    }
}