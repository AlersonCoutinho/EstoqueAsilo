using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Enums;
using EstoqueAsilo.Helpers;
using EstoqueAsilo.Models;
using EstoqueAsilo.Models.Resultados;
using EstoqueAsilo.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace EstoqueAsilo.ViewModels
{
    public partial class SaidaViewModel : ObservableObject
    {
        #region Serviços
        private readonly EstoqueService _estoque;
        private readonly ConsultaService _consulta;
        #endregion

        #region Listas
        private readonly List<MovimentacaoLinha> _todasSaidas = [];
        public ObservableCollection<MovimentacaoLinha> Saidas { get; set; } = [];
        public ObservableCollection<string> OpcoesUsuario { get; } = ["Todos"];
        public List<string> OpcoesTipo { get; } = ["Todas", "Doação", "Consumo"];
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
        public partial string ProdutoSelecionado { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string UnidadeSelecionada { get; set; } = string.Empty;
        [ObservableProperty]
        public partial bool UnidadeIsEnabled { get; set; } = true;
        [ObservableProperty]
        public partial string Quantidade { get; set; } = string.Empty;
        [ObservableProperty]
        public partial bool EhDoacao { get; set; } = true;
        [ObservableProperty]
        public partial string Instituicao { get; set; } = string.Empty;
        [ObservableProperty]
        public partial bool MostrarMensagem { get; set; } = false;
        [ObservableProperty]
        public partial bool TemErro { get; set; } = false;
        [ObservableProperty]
        public partial string Mensagem { get; set; } = string.Empty;
        #endregion

        public SaidaViewModel(EstoqueService servicoEstoque, ConsultaService consulta)
        {
            _estoque = servicoEstoque;
            _consulta = consulta;

            Atualizar();
        }

        private void Atualizar()
        {
            // -- Atualiza a lista de saídas
            _todasSaidas.Clear();
            foreach (var m in _consulta.ListarSaidas().OrderByDescending(m => m.Data))
            {
                _todasSaidas.Add(new MovimentacaoLinha(
                    m.Data,
                    $"{m.Produto.Nome} {m.Produto.Unidade}",
                    m.IsDoacao,
                    m.Contraparte ?? "Consumo",
                    m.Quantidade,
                    m.Usuario.Nome));
            }

            // -- Atualiza as lista de usuários
            OpcoesUsuario.Clear();
            OpcoesUsuario.Add("Todos");
            UsuarioFiltro = "Todos";
            foreach (var u in _todasSaidas.Select(e => e.Usuario).Where(u => u != "").Distinct().Order())
                OpcoesUsuario.Add(u);

            // -- Aplicar Filtro
            FiltrarSaidas();

            // -- Atualiza a lista de produtos
            Produtos = _consulta.ListarNomeProdutos() ?? [];
            AtualizarUnidades();
        }

        private void AtualizarUnidades()
        {
            Unidades = _consulta.ListarUnidadesProduto(ProdutoSelecionado) ?? [];
            UnidadeSelecionada = Unidades.FirstOrDefault() ?? string.Empty;
        }

        private void FiltrarSaidas()
        {
            var saidasFiltradas = FiltroMovimentacoesHelper.FiltrarMovimentacoes(_todasSaidas, TermoBusca, UsuarioFiltro, TipoFiltro);
            Saidas.Clear();
            foreach (var s in saidasFiltradas)
                Saidas.Add(s);
        }

        #region Métodos OnPropertyChanged 
        // -- Filtros
        partial void OnTermoBuscaChanged(string value) =>
            FiltrarSaidas();
        partial void OnUsuarioFiltroChanged(string value) =>
            FiltrarSaidas();
        partial void OnTipoFiltroChanged(string value) =>
            FiltrarSaidas();
        partial void OnProdutoSelecionadoChanged(string value)
        {
            AtualizarUnidades();
            UnidadeIsEnabled = Unidades.Count() > 1;
        }
        #endregion

        [RelayCommand]
        private async Task RegSaida()
        {
            if (_consulta.ProdutoPorNome(ProdutoSelecionado, UnidadeSelecionada) is not Produto produto)
            {
                MostrarNotificacao(true, "Produto não encontrado!");
                return;
            }
            if (!int.TryParse(Quantidade, out int quantidade) || quantidade <= 0)
            {
                MostrarNotificacao(true, "Quantidade inválida!");
                return;
            }

            // -- Confirmacao Lote
            var lotesSugeridos = _consulta.SugerirLotes(produto, quantidade);
            if (produto.Categoria == Categoria.Alimentos)
            {
                if (lotesSugeridos.Count == 0)
                {
                    MostrarNotificacao(true, "Não há lotes disponíveis para este produto!");
                    return;
                }

                foreach (var l in lotesSugeridos)
                {
                    bool continuar = await Shell.Current.DisplayAlertAsync(
                        "Confirme Data de Validade do Alimento!",
                        $"A Próxima Data de Validade é {l.Lote.DataValidade.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}",
                        "Registrar", "Cancelar");

                    if (!continuar) return;
                }
            }
            int qtProduto = _consulta.CalcularQuantidadeDeProduto(produto);
            if (quantidade > qtProduto)
            {
                bool continuar = await Shell.Current.DisplayAlertAsync(
                    "Não há quantidade suficiente registrado no estoque!",
                    $"Deseja registrar saída de {qtProduto} unidades?",
                    "Registrar", "Cancelar");

                if (!continuar) return;

                quantidade = qtProduto;
            }

            var (sucesso, erro) = _estoque.RegNovaSaida(produto, quantidade, EhDoacao, Instituicao, lotesSugeridos);
            if (sucesso)
            {
                LimparCampos();
                Atualizar();
                MostrarNotificacao(false, "Saída registrada com sucesso!");
            }
            else
            {
                MostrarNotificacao(true, erro ?? "Erro ao registrar saída!");
                Debug.WriteLine(erro);
            }
            await Task.CompletedTask;
        }

        //Metodos auxiliares
        private void MostrarNotificacao(bool ehErro, string mensagem)
        {
            MostrarMensagem = true;
            TemErro = ehErro;
            Mensagem = mensagem;
        }
        private void LimparCampos()
        {
            ProdutoSelecionado = string.Empty;
            UnidadeSelecionada = string.Empty;
            Quantidade = string.Empty;
            EhDoacao = true;
            Instituicao = string.Empty;
        }
    }
}
