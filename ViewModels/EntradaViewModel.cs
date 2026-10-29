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
        private partial ObservableCollection<Movimentacao> Entradas { get; set; }
        [ObservableProperty]
        public partial string NomeProduto { get; set; }
        [ObservableProperty]
        private partial string Quantidade { get; set; }
        [ObservableProperty]
        private partial string UnidadeSelecionada { get; set; }
        [ObservableProperty]
        private partial bool EhDoacao { get; set; }
        [ObservableProperty]
        private partial bool NaoEhDoacao { get; set; }
        [ObservableProperty]
        private partial Categoria CategoriaSelecionada { get; set; }
        [ObservableProperty]
        private partial string Origem { get; set; }
        [ObservableProperty]
        private partial string TipoOrigem { get; set; }
        [ObservableProperty]
        private partial DateTime DataValidade { get; set; }
        [ObservableProperty]
        private partial bool MostrarValidade { get; set; }
        [ObservableProperty]
        private partial string Preco { get; set; }
        [ObservableProperty]
        private partial string NotaFiscal { get; set; }
        public EntradaViewModel(EstoqueService servicoEstoque, ConsultaService consulta)
        {
            _estoque = servicoEstoque;
            AtualizarEntradas();
            _consulta = consulta;

            Entradas = [];
            NomeProduto = string.Empty;
            Quantidade = string.Empty;
            UnidadeSelecionada = Unidades[0];
            EhDoacao = true;
            NaoEhDoacao = false;
            CategoriaSelecionada = Categoria.Outros;
            Origem = string.Empty;
            TipoOrigem = "Doador";
            DataValidade = DateTime.Today;
            MostrarValidade = true;
            Preco = string.Empty;
            NotaFiscal = string.Empty;
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
