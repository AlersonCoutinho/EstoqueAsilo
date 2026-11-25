using EstoqueAsilo.Enums;
using EstoqueAsilo.Helpers;
using EstoqueAsilo.Models;
using EstoqueAsilo.Models.Resultados;
using EstoqueAsilo.Storage;
using System.Diagnostics;

namespace EstoqueAsilo.Services
{
    public class EstoqueService(AuthService auth)
    {
        private readonly string _caminhoMovimentacoes = Path.Combine(FileSystem.AppDataDirectory, "Movimentacoes.xml");
        private readonly string _caminhoProdutos = Path.Combine(FileSystem.AppDataDirectory, "Produtos.xml");
        private readonly string _caminhoLotes = Path.Combine(FileSystem.AppDataDirectory, "Lotes.xml");
        internal List<Produto> Produtos { get; private set; } = [];
        internal List<Movimentacao> Movimentacoes { get; private set; } = [];
        internal List<Lote> Lotes { get; private set; } = [];
        private readonly AuthService _auth = auth;

        // Carrega os dados de produtos, lotes e movimentações do armazenamento
        internal void Carregar()
        {
            Produtos = ProdutosRepository.Carregar(_caminhoProdutos);
            var produtosPorId = Produtos.ToDictionary(p => p.Id);
            Lotes = LotesRepository.Carregar(_caminhoLotes, produtosPorId);
            var lotePorId = Lotes.ToDictionary(l => l.Id);
            var userPorId = _auth.Usuarios.ToDictionary(u => u.Id);
            Movimentacoes = MovimentacoesRepository.Carregar(_caminhoMovimentacoes, produtosPorId, userPorId, lotePorId);
        }

        // Salva os dados de produtos, lotes e movimentações no armazenamento
        internal void Salvar()
        {
            if (Produtos.Count > 0) ProdutosRepository.Salvar(_caminhoProdutos, Produtos);
            if (Lotes.Count > 0) LotesRepository.Salvar(_caminhoLotes, Lotes);
            if (Movimentacoes.Count > 0) MovimentacoesRepository.Salvar(_caminhoMovimentacoes, Movimentacoes);
        }

        // Criações de novos objetos e registro de movimentações
        internal (bool Sucesso, string? Erro) RegNovaEntrada(string nomeProduto, string unidade, string quantidadeStr,
            bool isDoacao, string origem, Categoria categoria, DateTime? validade = null, decimal? preco = null,
            string? idNotaFiscal = null)
        {
            //Checando validações de entrada
            //Verificando Usuário Logado
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

        internal (bool Sucesso, string? Erro) RegNovaSaida(Produto produto, int quantidade,
            bool isDoacao, string instituicao, List<LoteSugerido> lotes)
        {

            // --- Checando validações de entrada
            // -- Verificando Usuário Logado
            if (_auth.UsuarioLogado is null)
                throw new InvalidOperationException("Usuário não está logado.");

            // -- Informações Movimentação
            if (isDoacao && string.IsNullOrWhiteSpace(instituicao))
                return (false, "Instituição inválida");

            //Criação da movimentação de saida, considerando lotes sugeridos se houver
            if (lotes.Count > 0)
            {
                foreach (var l in lotes)
                {
                    Movimentacao saidaComLote = new()
                    {
                        Id = IdHelper.NovoId(Movimentacoes, e => e.Id),
                        TipoMovimentacao = TipoMovimentacao.Saida,
                        Produto = produto,
                        Usuario = _auth.UsuarioLogado,
                        Quantidade = quantidade,
                        Data = DateTime.Now,
                        IsDoacao = isDoacao,
                        Lote = l.Lote,
                        Contraparte = isDoacao ? instituicao.Trim() : null
                    };
                    Movimentacoes.Add(saidaComLote);
                }
                return (true, null);
            }
            Movimentacao saida = new()
            {
                Id = IdHelper.NovoId(Movimentacoes, e => e.Id),
                TipoMovimentacao = TipoMovimentacao.Saida,
                Produto = produto,
                Usuario = _auth.UsuarioLogado,
                Quantidade = quantidade,
                Data = DateTime.Now,
                IsDoacao = isDoacao,
                Contraparte = isDoacao ? instituicao.Trim() : null
            };
            Movimentacoes.Add(saida);
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
            Lote lote = new()
            {
                Id = IdHelper.NovoId(Lotes, l => l.Id),
                Produto = produto,
                DataValidade = dataValidade,
                QuantidadeInicial = quantidadeInicial
            };
            Lotes.Add(lote);
            return lote;
        }

        internal bool ExcluirDados()
        {
            Produtos = [];
            Movimentacoes = [];
            Lotes = [];

            if (Produtos.Count == 0 && Movimentacoes.Count == 0 && Lotes.Count == 0)
                return true;

            return false;
        }
    }
}
