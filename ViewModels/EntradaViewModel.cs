using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Enums;
using EstoqueAsilo.Models;
using EstoqueAsilo.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace EstoqueAsilo.ViewModels
{
    public partial class EntradaViewModel : ObservableObject
    {
        private readonly EstoqueService _estoque;
        private readonly ConsultaService _consulta;
        public ObservableCollection<Categoria> Categorias { get; } = new(Enum.GetValues<Categoria>().Where(c => c != Categoria.Outros));
        public ObservableCollection<string> Unidades { get; } = ["Unidades", "Litros", "Quilos", "Gramas"];

        [ObservableProperty]
        private ObservableCollection<Movimentacao> entradas = [];
        [ObservableProperty]
        private string nomeProduto = string.Empty;
        [ObservableProperty]
        private string quantidade = string.Empty;
        [ObservableProperty]
        private string unidadeSelecionada = string.Empty;
        [ObservableProperty]
        private bool ehDoacao = true;
        [ObservableProperty]
        private bool naoEhDoacao = false;
        [ObservableProperty]
        private Categoria categoriaSelecionada = Categoria.Outros;
        [ObservableProperty]
        private string origem = string.Empty;
        [ObservableProperty]
        private string tipoOrigem = "Doador";
        [ObservableProperty]
        private DateTime dataValidade = DateTime.Today;
        [ObservableProperty]
        private bool mostrarValidade = true;
        [ObservableProperty]
        private string preco = string.Empty;
        [ObservableProperty]
        private string notaFiscal = string.Empty;

        public EntradaViewModel(EstoqueService servicoEstoque, ConsultaService consulta)
        {
            _estoque = servicoEstoque;
            AtualizarEntradas();
            UnidadeSelecionada = Unidades[0];
            _consulta = consulta;
        }

        public void AtualizarEntradas()
        {
            Entradas = new ObservableCollection<Movimentacao>(_consulta.ListarEntradas());
        }

        public void LimparCampos()
        {
            NomeProduto = string.Empty;
            Quantidade = string.Empty;
            UnidadeSelecionada = Unidades[0];
            Origem = string.Empty;
            DataValidade = DateTime.Today;
            Preco = string.Empty;
            NotaFiscal = string.Empty;
        }

        [RelayCommand]
        private async Task RegEntrada()
        {
            if (string.IsNullOrWhiteSpace(NomeProduto) || string.IsNullOrWhiteSpace(Quantidade))
                return;

            Decimal? precoNum = !string.IsNullOrEmpty(Preco) ? Decimal.Parse(Quantidade, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture) : null;

            var (sucesso, erro) = _estoque.RegNovaEntrada(NomeProduto, UnidadeSelecionada, Quantidade, EhDoacao, Origem, CategoriaSelecionada, DataValidade, precoNum, NotaFiscal);
            if (sucesso)
            {
                LimparCampos();
                AtualizarEntradas();
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
            NaoEhDoacao = !value;
            TipoOrigem = value ? "Doador" : "Fornecedor";

            // Se voltar para doação, limpamos os campos de compra por segurança
            if (value)
            {
                Preco = string.Empty;
                NotaFiscal = string.Empty;
            }
        }
    }
}
