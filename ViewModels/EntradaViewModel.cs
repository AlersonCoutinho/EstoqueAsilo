using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EstoqueAsilo.Enums;
using EstoqueAsilo.Models;
using EstoqueAsilo.Models.Resultados;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels
{
    public partial class EntradaViewModel : ObservableObject
    {
        // -- Serviços
        private readonly EstoqueService _estoque;
        private readonly ConsultaService _consulta;

        // -- Listas
        public ObservableCollection<Categoria> Categorias { get; } = new(Enum.GetValues<Categoria>().Where(c => c != Categoria.Outros));
        public ObservableCollection<MovimentacaoLinha> Entradas { get; set; } = [];

        // -- Propriedades
        [ObservableProperty]
        public partial IEnumerable<string> Produtos { get; set; } = [];
        [ObservableProperty]
        public partial IEnumerable<string> Unidades { get; set; } = [];
        [ObservableProperty]
        public partial string NomeProduto { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string Quantidade { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string UnidadeSelecionada { get; set; } = string.Empty;
        [ObservableProperty]
        public partial bool EhDoacao { get; set; } = true;
        [ObservableProperty]
        public partial bool NaoEhDoacao { get; set; } = false;
        [ObservableProperty]
        public partial Categoria CategoriaSelecionada { get; set; } = Categoria.Alimentos;
        [ObservableProperty]
        public partial bool CategoriaIsEnabled { get; set; } = true;
        [ObservableProperty]
        public partial string Origem { get; set; } = string.Empty;
        [ObservableProperty]
        public partial DateTime DataValidade { get; set; } = DateTime.Today;
        [ObservableProperty]
        public partial bool MostrarValidade { get; set; } = true;
        [ObservableProperty]
        public partial decimal Preco { get; set; } = 0m;
        [ObservableProperty]
        public partial string NotaFiscal { get; set; } = string.Empty;

        public EntradaViewModel(EstoqueService servicoEstoque, ConsultaService consulta)
        {
            _estoque = servicoEstoque;
            _consulta = consulta;

            Atualizar();
        }

        public void Atualizar()
        {
            Entradas.Clear();

            foreach (var m in _consulta.ListarEntradas().OrderByDescending(m => m.Data))
            {
                Entradas.Add(new MovimentacaoLinha(
                    m.Data,
                    $"{m.Produto.Nome} {m.Produto.Unidade}",
                    m.IsDoacao,
                    m.Contraparte ?? "—",
                    m.Quantidade));
            }

            Produtos = _consulta.ListarProdutos() ?? [];
            Unidades = _consulta.ListarUnidades() ?? [];
        }

        public void LimparCampos()
        {
            NomeProduto = string.Empty;
            Quantidade = string.Empty;
            UnidadeSelecionada = string.Empty;
            Origem = string.Empty;
            DataValidade = DateTime.Today;
            Preco = 0m;
            NotaFiscal = string.Empty;
        }

        [RelayCommand]
        private async Task RegEntrada()
        {
            if (string.IsNullOrWhiteSpace(NomeProduto) || string.IsNullOrWhiteSpace(Quantidade))
                return;

            Decimal? precoNum = Preco > 0 ? Preco : null;
            DateTime? validade = MostrarValidade ? DataValidade : null;

            var (sucesso, erro) = _estoque.RegNovaEntrada(NomeProduto, UnidadeSelecionada, Quantidade, EhDoacao, Origem, 
                CategoriaSelecionada, validade, precoNum, NotaFiscal);
            if (sucesso)
            {
                LimparCampos();
                Atualizar();
                WeakReferenceMessenger.Default.Send(new EstoqueAtualizadoMessage());
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

            // Se voltar para doação, limpamos os campos de compra por segurança
            if (value)
            {
                Preco = 0m;
                NotaFiscal = string.Empty;
            }
        }

        partial void OnNomeProdutoChanged(string value) => 
            AtualizarCategoriaSelecionada(value, UnidadeSelecionada);
        partial void OnUnidadeSelecionadaChanged(string value) =>
            AtualizarCategoriaSelecionada(NomeProduto, value);

        private void AtualizarCategoriaSelecionada(string nome, string unidade)
        {
            if (_consulta.ProdutoPorNome(nome, unidade) is not Produto produto) 
            {
                CategoriaIsEnabled = true;
                return;
            }
            CategoriaSelecionada = produto.Categoria;
            CategoriaIsEnabled = false;
        }
    }
}
