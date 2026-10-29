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
        public partial ObservableCollection<Movimentacao> Entradas { get; set; }
        [ObservableProperty]
        public partial string NomeProduto { get; set; }
        [ObservableProperty]
        public partial string Quantidade { get; set; }
        [ObservableProperty]
        public partial string UnidadeSelecionada { get; set; }
        [ObservableProperty]
        public partial bool EhDoacao { get; set; }
        [ObservableProperty]
        public partial bool NaoEhDoacao { get; set; }
        [ObservableProperty]
        public partial Categoria CategoriaSelecionada { get; set; }
        [ObservableProperty]
        public partial string Origem { get; set; }
        [ObservableProperty]
        public partial string TipoOrigem { get; set; }
        [ObservableProperty]
        public partial DateTime DataValidade { get; set; }
        [ObservableProperty]
        public partial bool MostrarValidade { get; set; }
        [ObservableProperty]
        public partial string Preco { get; set; }
        [ObservableProperty]
        public partial string NotaFiscal { get; set; }
        public EntradaViewModel(EstoqueService servicoEstoque, ConsultaService consulta)
        {
            _estoque = servicoEstoque;
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

            AtualizarEntradas();
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
