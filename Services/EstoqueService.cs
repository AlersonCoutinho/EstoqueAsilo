using EstoqueAsilo.Enums;
using EstoqueAsilo.Helpers;
using EstoqueAsilo.Models;
using EstoqueAsilo.Storage;
using System.Diagnostics;

namespace EstoqueAsilo.Services
{
    public class EstoqueService(AuthService auth)
    {
        private readonly string _caminhoMovimentacoes = Path.Combine(FileSystem.AppDataDirectory, "Movimentacoes.xml");
        private readonly string _caminhoProdutos = Path.Combine(FileSystem.AppDataDirectory, "Produtos.xml");
        private readonly string _caminhoLotes = Path.Combine(FileSystem.AppDataDirectory, "Lotes.xml");
        public List<Produto> Produtos { get; private set; } = [];
        public List<Movimentacao> Movimentacoes { get; private set; } = [];
        public List<Lote> Lotes { get; private set; } = [];
        private readonly AuthService _auth = auth;

        public void Carregar()
        {
            Produtos = ProdutosRepository.Carregar(_caminhoProdutos);
            var produtosPorId = Produtos.ToDictionary(p => p.Id);
            Lotes = LotesRepository.Carregar(_caminhoLotes, produtosPorId);
            var lotePorId = Lotes.ToDictionary(l => l.Id);
            var userPorId = _auth.Usuarios.ToDictionary(u => u.Id);
            Movimentacoes = MovimentacoesRepository.Carregar(_caminhoMovimentacoes, produtosPorId, userPorId, lotePorId);
        }
        public void Salvar()
        {
            if (Produtos.Count > 0) ProdutosRepository.Salvar(_caminhoProdutos, Produtos);
            if (Lotes.Count > 0) LotesRepository.Salvar(_caminhoLotes, Lotes);
            if (Movimentacoes.Count > 0) MovimentacoesRepository.Salvar(_caminhoMovimentacoes, Movimentacoes);
        }

        public (bool Sucesso, string? Erro) RegNovaEntrada(string nomeProduto, string unidade, string quantidadeStr, bool isDoacao, string origem, Categoria categoria,
            DateTime? validade = null, decimal? preco = null, string? idNotaFiscal = null)
        {
            //Checando validações de entrada
            if (_auth.UsuarioLogado is null)
                throw new InvalidOperationException("Usuário não está logado.");

            //Informações Produto
            if (string.IsNullOrWhiteSpace(nomeProduto)) return (false, "Nome produto inválido");
            if (string.IsNullOrWhiteSpace(unidade)) return (false, "Unidade do produto inválida");

            //Informações Movimentação
            if (!int.TryParse(quantidadeStr, out int quantidade) || quantidade <= 0) return (false, "Quantidade inválida");
            if (string.IsNullOrWhiteSpace(origem)) return (false, "Origem inválida");
            if (preco is not null && preco <= 0) return (false, "Preço inválido");

            //Procurando produto existente ou criando novo
            var produto = Produtos.FirstOrDefault(p => 
                p.Nome.Equals(nomeProduto.Trim(), StringComparison.OrdinalIgnoreCase) && 
                p.Unidade.Equals(unidade.Trim(), StringComparison.OrdinalIgnoreCase));
            if (produto is not null && produto.Categoria != categoria)
            {
                Debug.WriteLine($"Categoria inválida, desconsiderando seleção");
            }
            produto ??= NovoProduto(
                    nomeProduto.Trim(),
                    categoria,
                    unidade.Trim()
                );

            //Criação lote se tiver validade
            var lote = validade is DateTime v ? 
                NovoLote(produto, v, quantidade) : null;

            //Criação da movimentação de entrada
            Movimentacao entrada = new()
            {
                Id = IdHelper.NovoId(Movimentacoes, e => e.Id),
                TipoMovimentacao = TipoMovimentacao.Entrada,
                Produto = produto,
                Usuario = _auth.UsuarioLogado,
                Quantidade = quantidade,
                Data = DateTime.Now,
                IsDoacao = isDoacao,
                Lote = lote,
                Contraparte = origem.Trim(),
                Preco = isDoacao ? null : preco,
                IdNotaFiscal = isDoacao ? null : idNotaFiscal
            };
            Movimentacoes.Add(entrada);
            return (true, null);

        }

        private Produto NovoProduto(string nomeProduto, Categoria categoria, string unidade)
        {
            Produto produto = new()
            {
                Id = IdHelper.NovoId(Produtos, p => p.Id),
                Nome = nomeProduto.Trim(),
                Categoria = categoria,
                Unidade = unidade
            };
            Produtos.Add(produto);
            return produto;
        }

        private Lote NovoLote(Produto produto, DateTime dataValidade, int quantidadeInicial)
        {
            Lote lote = new() { 
                Id = IdHelper.NovoId(Lotes, l => l.Id),
                Produto = produto, 
                DataValidade = dataValidade, 
                QuantidadeInicial = quantidadeInicial
            };
            Lotes.Add(lote);
            return lote;
        }
    }
}
