using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EstoqueAsilo.Enums;
using EstoqueAsilo.Models.Resultados;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels
{
    public class NavegarParaEntradaMessage { }
    public class EstoqueAtualizadoMessage { }
    public partial class EstoqueViewModel : ObservableObject
    {
        private readonly ConsultaService _consulta;
        private readonly EstoqueService _estoque;

        [ObservableProperty]
        public partial string TituloCategoria { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string TermoBusca { get; set; } = string.Empty;

        [ObservableProperty]
        public partial ObservableCollection<EstoqueLinha> ProdutosFiltrados { get; set; } = [];

        private readonly List<EstoqueLinha> _todosProdutos = [];

        public EstoqueViewModel(ConsultaService consulta, EstoqueService estoque)
        {
            _consulta = consulta;
            _estoque = estoque;
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
            _todosProdutos.Clear();

            var produtosDaCategoria = _estoque.Produtos
                .Where(p => p.Categoria == categoria)
                .ToList();

            foreach (var p in produtosDaCategoria)
            {
                int quantidadeAtual = _consulta.CalcularQuantidadeDeProduto(p);

                var validades = _consulta.ListarValidadesDeProduto(p).ToList();
                string dataValidadeStr = validades.Count != 0 ? validades.Min().ToString("dd/MM/yyyy") : "—";

                string situacao = "OK";
                if (p.QuantidadeMinima is not null && quantidadeAtual <= p.QuantidadeMinima)
                    situacao = "Estoque Baixo";
                else if (validades.Count != 0 && validades.Min() < DateTime.Today)
                    situacao = "Vencido";
                else if (validades.Count != 0 && validades.Min() <= DateTime.Today.AddDays(30))
                    situacao = "Vence em breve";

                _todosProdutos.Add(new EstoqueLinha(p.Nome, quantidadeAtual.ToString(), p.QuantidadeMinima?.ToString() ?? "—", dataValidadeStr, situacao));
            }

            FiltrarProdutos();
        }

        partial void OnTermoBuscaChanged(string value) => FiltrarProdutos();

        private void FiltrarProdutos()
        {
            if (string.IsNullOrWhiteSpace(TermoBusca))
            {
                ProdutosFiltrados = new ObservableCollection<EstoqueLinha>(_todosProdutos);
            }
            else
            {
                var filtrados = _todosProdutos
                    .Where(p => p.Nome.Contains(TermoBusca, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                ProdutosFiltrados = new ObservableCollection<EstoqueLinha>(filtrados);
            }
        }
        [RelayCommand]
        private static void NovoItem()
        {
            WeakReferenceMessenger.Default.Send(new NavegarParaEntradaMessage());
        }
    }
}