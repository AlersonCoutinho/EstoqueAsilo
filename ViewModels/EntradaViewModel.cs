using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EstoqueAsilo.Enums;
using EstoqueAsilo.Helpers;
using EstoqueAsilo.Models;
using EstoqueAsilo.Models.Resultados;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels
{
    public partial class EntradaViewModel : ObservableObject
    {
        #region Serviços

        private readonly EstoqueService _estoque;
        private readonly ConsultaService _consulta;
        #endregion

        #region Listas
        private readonly List<MovimentacaoLinha> _todasEntradas = [];
        public ObservableCollection<Categoria> Categorias { get; } = new(Enum.GetValues<Categoria>().Where(c => c != Categoria.Outros));
        public ObservableCollection<MovimentacaoLinha> Entradas { get; set; } = [];
        public ObservableCollection<string> OpcoesUsuario { get; } = ["Todos"];
        public List<string> OpcoesTipo { get; } = ["Todas", "Doação", "Compra"]; 
        #endregion

        #region Propriedades Filtros
        [ObservableProperty]
        public partial string TermoBusca { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string UsuarioFiltro { get; set; } = "Todos";
        [ObservableProperty]
        public partial string TipoFiltro { get; set; } = "Todas"; 
        #endregion

        #region Propriedades Criação Movimentação
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
        [ObservableProperty]
        public partial bool MostrarMensagem { get; set; } = false;
        [ObservableProperty]
        public partial bool TemErro { get; set; } = false;
        [ObservableProperty]
        public partial string Mensagem { get; set; } = string.Empty; 
        #endregion

        public EntradaViewModel(EstoqueService servicoEstoque, ConsultaService consulta)
        {
            _estoque = servicoEstoque;
            _consulta = consulta;

            Atualizar();
        }

        public void Atualizar()
        {
            // -- Atualiza a lista de entradas
            _todasEntradas.Clear();
            foreach (var m in _consulta.ListarEntradas().OrderByDescending(m => m.Data))
            {
                _todasEntradas.Add(new MovimentacaoLinha(
                    m.Data,
                    $"{m.Produto.Nome} {m.Produto.Unidade}",
                    m.IsDoacao,
                    m.Contraparte ?? "—",
                    m.Quantidade,
                    m.Usuario.Nome));
            }

            // -- Atualiza as lista de usuários
            OpcoesUsuario.Clear();
            OpcoesUsuario.Add("Todos");
            UsuarioFiltro = "Todos";
            foreach (var u in _todasEntradas.Select(e => e.Usuario).Where(u => u != "").Distinct().Order())
                OpcoesUsuario.Add(u);

            // -- Aplica Filtros
            FiltrarEntradas();

            // -- Atualiza as listas de produtos e unidades
            Produtos = _consulta.ListarNomeProdutos() ?? [];
            Unidades = _consulta.ListarUnidades() ?? [];
        }

        private void FiltrarEntradas()
        {
            var entradasFiltradas = FiltroMovimentacoesHelper.FiltrarMovimentacoes(_todasEntradas, TermoBusca, UsuarioFiltro, TipoFiltro);
            Entradas.Clear();
            foreach (var e in entradasFiltradas)
                Entradas.Add(e);
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
            {
                MostrarNotificacao(true, "Preencha todos os campos obrigatórios!");
                return;
            }

            Decimal? precoNum = Preco > 0 ? Preco : null;
            DateTime? validade = MostrarValidade ? DataValidade : null;

            var (sucesso, erro) = _estoque.RegNovaEntrada(NomeProduto, UnidadeSelecionada, Quantidade, EhDoacao, Origem, 
                CategoriaSelecionada, validade, precoNum, NotaFiscal);
            if (sucesso)
            {
                LimparCampos();
                Atualizar();
                MostrarNotificacao(false, "Entrada registrada com sucesso!");
                //WeakReferenceMessenger.Default.Send(new EstoqueAtualizadoMessage());
            }
            else
            {
                MostrarNotificacao(true, erro ?? "Erro ao registrar entrada!");
            }
            await Task.CompletedTask;
        }

        #region Métodos OnPropertyChanged

        // -- Filtros
        partial void OnTermoBuscaChanged(string value) =>
            FiltrarEntradas();
        partial void OnUsuarioFiltroChanged(string value) =>
            FiltrarEntradas();
        partial void OnTipoFiltroChanged(string value) =>
            FiltrarEntradas();

        // -- Criação Movimentação
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
        partial void OnCategoriaSelecionadaChanged(Categoria value) => 
            MostrarValidade = (value.TemValidade());
        partial void OnNomeProdutoChanged(string value) =>
            AtualizarCategoriaSelecionada(value, UnidadeSelecionada);
        partial void OnUnidadeSelecionadaChanged(string value) =>
            AtualizarCategoriaSelecionada(NomeProduto, value);
        #endregion

        //Metodos Auxiliares
        private void MostrarNotificacao(bool ehErro, string mensagem)
        {
            MostrarMensagem = true;
            TemErro = ehErro;
            Mensagem = mensagem;
        }
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
