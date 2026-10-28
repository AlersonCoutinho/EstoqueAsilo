using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Enums;
using EstoqueAsilo.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace EstoqueAsilo.ViewModels
{
    public partial class EntradaViewModel : ObservableObject
    {
        private readonly EstoqueService servicoEstoque;
        public ObservableCollection<Categoria> Categorias { get; } = new(Enum.GetValues<Categoria>().Where(c => c != Categoria.Outros));
        public ObservableCollection<string> Unidades { get; } = ["Unidades", "Litros", "Quilos", "Gramas"];

        [ObservableProperty]
        private string nomeProduto = string.Empty;
        [ObservableProperty]
        private string quantidade = string.Empty;
        [ObservableProperty]
        private string unidadeSelecionada = string.Empty;
        [ObservableProperty]
        private bool ehDoacao = false;
        [ObservableProperty]
        private Categoria categoriaSelecionada = Categoria.Outros;
        [ObservableProperty]
        private string origem = string.Empty;
        [ObservableProperty]
        private DateTime dataValidade = DateTime.Today;
        [ObservableProperty]
        private bool mostrarValidade = true;
        [ObservableProperty]
        private string preco = string.Empty;
        [ObservableProperty]
        private string notaFiscal = string.Empty;
        [ObservableProperty]
        private bool mostrarDoador = false;
        [ObservableProperty]
        private bool mostrarFornecedor = true;

        public EntradaViewModel(EstoqueService servicoEstoque)
        {
            this.servicoEstoque = servicoEstoque;
            UnidadeSelecionada = Unidades[0];
        }

        [RelayCommand]
        private async Task RegEntrada()
        {
            if (string.IsNullOrWhiteSpace(NomeProduto) || string.IsNullOrWhiteSpace(Quantidade))
                return;

            Decimal? precoNum = !string.IsNullOrEmpty(Preco) ? Decimal.Parse(Quantidade, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture) : null;

            var (sucesso, erro) = servicoEstoque.RegNovaEntrada(NomeProduto, UnidadeSelecionada, Quantidade, EhDoacao, Origem, CategoriaSelecionada, DataValidade, precoNum, NotaFiscal);
            if (sucesso)
            {
                NomeProduto = string.Empty;
                Quantidade = string.Empty;
                Origem = string.Empty;
                DataValidade = DateTime.Today;
                Preco = string.Empty;
                NotaFiscal = string.Empty;
            }
            else
            {
                Debug.WriteLine(erro);
            }
            await Task.CompletedTask;
        }
        partial void OnCategoriaSelecionadaChanged(Categoria value)
        {
            MostrarValidade = (value == Categoria.Alimentos);
        }
        partial void OnEhDoacaoChanged(bool value)
        {
            MostrarDoador = value;
            MostrarFornecedor = !value;

            // Se voltar para doação, limpamos os campos de compra por segurança
            if (value)
            {
                Preco = string.Empty;
                NotaFiscal = string.Empty;
            }
        }
    }
}
