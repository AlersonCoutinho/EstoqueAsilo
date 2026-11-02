using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Enums;
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
        private readonly EstoqueService _estoque;
        private readonly ConsultaService _consulta;

        public ObservableCollection<MovimentacaoLinha> Saidas { get; set; } = [];

        // -- Propriedades
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

        public SaidaViewModel(EstoqueService servicoEstoque, ConsultaService consulta)
        {
            _estoque = servicoEstoque;
            _consulta = consulta;

            Atualizar();
        }

        private void Atualizar()
        {
            Saidas.Clear();

            foreach (var m in _consulta.ListarSaidas().OrderByDescending(m => m.Data))
            {
                Saidas.Add(new MovimentacaoLinha(
                    m.Data,
                    $"{m.Produto.Nome} {m.Produto.Unidade}",
                    m.IsDoacao,
                    m.Contraparte ?? "Consumo",
                    m.Quantidade));
            }

            Produtos = _consulta.ListarProdutos() ?? [];
            AtualizarUnidades();
        }

        private void AtualizarUnidades() {
            Unidades = _consulta.ListarUnidadesProduto(ProdutoSelecionado) ?? [];
            UnidadeSelecionada = Unidades.FirstOrDefault() ?? string.Empty;
        }

        partial void OnProdutoSelecionadoChanged(string value)
        {
            AtualizarUnidades();
            UnidadeIsEnabled = Unidades.Count() > 1;
        }

        [RelayCommand]
        private async Task RegSaida()
        {
            if (_consulta.ProdutoPorNome(ProdutoSelecionado, UnidadeSelecionada) is not Produto produto)
                return;
            if (!int.TryParse(Quantidade, out int quantidade) || quantidade <= 0)
                return;

            // -- Confirmacao Lote
            var lotesSugeridos = _consulta.SugerirLotes(produto, quantidade);
            if (produto.Categoria == Categoria.Alimentos)
            {
                if (lotesSugeridos.Count == 0) return;

                foreach (var l in lotesSugeridos)
                {
                    bool continuar = await Shell.Current.DisplayAlertAsync(
                        "Confirme Data de Validade do Alimento!",
                        $"A Próxima Data de Validade é {l.Lote.DataValidade.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
                        "Registrar", "Cancelar");

                    if (!continuar) return;
                }
            }

            var (sucesso, erro) = _estoque.RegNovaSaida(produto, quantidade, EhDoacao, Instituicao, lotesSugeridos);
            if (sucesso)
            {
                LimparCampos();
                Atualizar();
            }
            else
            {
                Debug.WriteLine(erro);
            }
            await Task.CompletedTask;
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
