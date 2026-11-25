using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EstoqueAsilo.Enums;
using EstoqueAsilo.Models.Resultados;
using EstoqueAsilo.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace EstoqueAsilo.ViewModels
{
    public class NavegarParaEntradaMessage { }
    public class EstoqueAtualizadoMessage { }
    public partial class EstoqueViewModel : ObservableObject
    {
        private readonly GridLength _colunaValidadePadrao = new(100);
        private readonly GridLength _colunaValidadeVazia = new(0);
        private readonly ConsultaService _consulta;

        public List<OpcaoSituacao> OpcoesSituacao { get; set; } = [];

        [ObservableProperty]
        public partial bool MostrarValidade { get; set; } = true;
        [ObservableProperty]
        public partial GridLength ColunaValidade { get; set; }

        [ObservableProperty]
        public partial string TituloCategoria { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string TermoBusca { get; set; } = string.Empty;

        [ObservableProperty]
        public partial OpcaoSituacao? SituacaoSelecionada { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<EstoqueLinha> ProdutosFiltrados { get; set; } = [];

        private readonly List<EstoqueLinha> _todosProdutos = [];

        public EstoqueViewModel(ConsultaService consulta)
        {
            _consulta = consulta;

            ColunaValidade = _colunaValidadePadrao;
            WeakReferenceMessenger.Default.Register<EstoqueAtualizadoMessage>(this, (r, m) =>
            {
                if (!string.IsNullOrEmpty(TituloCategoria))
                {
                    var categoriaAtual = Enum.Parse<Categoria>(TituloCategoria);
                    CarregarCategoria(categoriaAtual);
                }
            });
        }

        public void CarregarCategoria(Categoria categoria)
        {
            TituloCategoria = categoria.ToString();
            MostrarValidade = categoria.TemValidade();
            ColunaValidade = MostrarValidade ? _colunaValidadePadrao : _colunaValidadeVazia;
            OpcoesSituacao = Enum.Parse<Categoria>(TituloCategoria).TemValidade() ?
            [
                new("Todas", null),
                new("OK", SituacaoEstoque.Nenhuma),
                new("Estoque baixo", SituacaoEstoque.EstoqueBaixo),
                new("Vence em breve", SituacaoEstoque.VenceEmBreve),
                new("Vencido", SituacaoEstoque.Vencido)
            ] :
            [
                new("Todas", null),
                new("OK", SituacaoEstoque.Nenhuma),
                new("Estoque baixo", SituacaoEstoque.EstoqueBaixo)
            ];
            SituacaoSelecionada = OpcoesSituacao[0];

            _todosProdutos.Clear();

            var produtosDaCategoria = _consulta.ListarProdutosPorCategoria(categoria);

            foreach (var p in produtosDaCategoria)
            {
                int quantidadeAtual = _consulta.CalcularQuantidadeDeProduto(p);

                var validades = categoria.TemValidade() ? _consulta.ListarValidadesDeProduto(p).ToList() : [];
                DateTime? proxima = validades.Count != 0 ? validades.Min() : null;
                string dataValidadeStr = proxima?.ToString("dd/MM/yyyy") ?? "-";

                var situacao = SituacaoEstoque.Nenhuma;
                if (p.QuantidadeMinima is not null && quantidadeAtual <= p.QuantidadeMinima)
                    situacao |= SituacaoEstoque.EstoqueBaixo;
                if (proxima is {  } data)
                {
                    if (data < DateTime.Today)
                        situacao |= SituacaoEstoque.Vencido;
                    else if (data <= DateTime.Today.AddDays(30))
                        situacao |= SituacaoEstoque.VenceEmBreve;
                } 

                _todosProdutos.Add(new EstoqueLinha(
                    $"{p.Nome} - {p.Unidade}", 
                    quantidadeAtual.ToString(),
                    p.QuantidadeMinima?.ToString() ?? "—", 
                    dataValidadeStr, 
                    situacao));
            }

            FiltrarProdutos();
        }

        partial void OnTermoBuscaChanged(string value) => FiltrarProdutos();

        private void FiltrarProdutos()
        {
            IEnumerable<EstoqueLinha> consulta = _todosProdutos;

            if (!string.IsNullOrWhiteSpace(TermoBusca))
                consulta = consulta.Where(p => p.Nome.Contains(TermoBusca, StringComparison.OrdinalIgnoreCase));

            var filtro = SituacaoSelecionada?.Valor;
            if (filtro is not null)
            {
                consulta = filtro == SituacaoEstoque.Nenhuma
                    ? consulta.Where(p => p.Situacao == SituacaoEstoque.Nenhuma)
                    : consulta.Where(p => p.Situacao.HasFlag(filtro.Value));
            }

            ProdutosFiltrados = new ObservableCollection<EstoqueLinha>(consulta);
        }
        
        partial void OnSituacaoSelecionadaChanged(OpcaoSituacao? value) => FiltrarProdutos();



        [RelayCommand]
        private async Task EditarMinimoAsync(EstoqueLinha linha)
        {
            if (linha is null) return;

            var page = Application.Current?.Windows[0]?.Page;
            if (page is null) return;

            string? resposta = await page.DisplayPromptAsync(
                title: "Estoque mínimo",
                message: $"Novo mínimo para \"{linha.Nome}\":",
                accept: "Salvar",
                cancel: "Cancelar",
                placeholder: "0",
                maxLength: 8,
                keyboard: Keyboard.Numeric,
                initialValue: linha.Minimo.ToString().Equals("—") ? "" : linha.Minimo.ToString());

            if (resposta is null) return;

            if (!int.TryParse(resposta, NumberStyles.Number, CultureInfo.CurrentCulture, out var novoMinimo)
                || novoMinimo < 0)
            {
                await page.DisplayAlertAsync("Valor inválido", "Digite um número maior ou igual a zero.", "OK");
                return;
            }
            string[] produtoInfo = linha.Nome.Split('-');
            var produto = _consulta.ProdutoPorNome(produtoInfo[0].Trim(), produtoInfo[1].Trim());
            if (produto is null) return;

            produto.QuantidadeMinima = novoMinimo;

            CarregarCategoria(Enum.Parse<Categoria>(TituloCategoria));
        }

        [RelayCommand]
        private static void NovoItem()
        {
            WeakReferenceMessenger.Default.Send(new NavegarParaEntradaMessage());
        }
    }
}