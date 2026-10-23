using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Globalization;

namespace EstoqueAsilo.Services
{
    public class ServicoEstoque
    {
        private readonly string _caminhoEntradas = Path.Combine(FileSystem.AppDataDirectory, "Entradas.xml");
        private readonly string _caminhoSaidas = Path.Combine(FileSystem.AppDataDirectory, "Saidas.xml");
        private readonly string _caminhoProdutos = Path.Combine(FileSystem.AppDataDirectory, "Produtos.xml");
        private readonly string _caminhoNomes = Path.Combine(FileSystem.AppDataDirectory, "Nomes.xml");
        private readonly string _caminhoSenhas = Path.Combine(FileSystem.AppDataDirectory, "Senhas.xml");
        public List<Produto> Produtos { get; private set; } = [];
        public List<Entrada> Entradas { get; private set; } = [];
        public List<Saida> Saidas { get; private set; } = [];
        public List<Usuario> Nomes { get; private set; } = [];
        public List<Senha> Senhas { get; private set; } = [];
        public Usuario? UsuarioLogado { get; private set; }
        public required Dictionary<int, Produto> porId;
        public required Dictionary<int, Usuario> userPorId;

        public void Carregar()
        {
            Nomes = ListaCredenciais.CarregarNomes(_caminhoNomes);
            GarantirArquivoAsync("Senhas.xml", _caminhoSenhas).Wait();
            Senhas = ListaCredenciais.CarregarSenhas(_caminhoSenhas);
            Produtos = ListaProdutos.Carregar(_caminhoProdutos);
            porId = Produtos.ToDictionary(p => p.Id);
            userPorId = Nomes.ToDictionary(u => u.Id);
            Entradas = ListaEntradas.Carregar(_caminhoEntradas, porId, userPorId);
            Saidas = ListaSaidas.Carregar(_caminhoSaidas, porId, userPorId);
        }
        internal static async Task GarantirArquivoAsync(string nomeAsset, string destino)
        {
            if (File.Exists(destino))
                return;

            using var origem = await FileSystem.OpenAppPackageFileAsync(nomeAsset);
            using var saida = File.Create(destino);
            await origem.CopyToAsync(saida);
        }
        public void Salvar()
        {
            ListaProdutos.Salvar(_caminhoProdutos, Produtos);
            ListaEntradas.Salvar(_caminhoEntradas, Entradas);
            ListaSaidas.Salvar(_caminhoSaidas, Saidas);
            ListaCredenciais.SalvarNomes(_caminhoNomes, Nomes);
            ListaCredenciais.SalvarSenhas(_caminhoSenhas, Senhas);
        }

        public bool Login(string nome, string senha)
        {
            foreach (var s in Senhas)
            {
                if (SenhaHelper.Verificar(senha, s.Hash, s.Salt))
                {
                    var usuario = Nomes.FirstOrDefault(u =>
                        u.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (usuario is null)
                    {
                        var ti = new CultureInfo("pt-BR").TextInfo;
                        string nomeFormatado = ti.ToTitleCase(nome.Trim().ToLower());
                        usuario = new Usuario
                        {
                            Nome = nomeFormatado,
                            Id = NovoId(Nomes, n => n.Id)
                        };
                        Nomes.Add(usuario);
                    }
                    UsuarioLogado = usuario;
                    return true;
                }
            }
            return false;

        }

        public void Logout()
        {
            UsuarioLogado = null;
        }

        public static int NovoId<T>(List<T> lista, Func<T, int> getId) =>
            lista.Count == 0 ? 1 : lista.Max(getId) + 1;

        public bool RegNovaEntrada(string nomeProduto, string quantidadeStr, bool doacao, string origem, string validadeStr)
        {
            if (UsuarioLogado is null)
                throw new InvalidOperationException("Usuário não está logado.");
            if (string.IsNullOrWhiteSpace(nomeProduto) || string.IsNullOrWhiteSpace(quantidadeStr) ||
                string.IsNullOrWhiteSpace(origem) || string.IsNullOrWhiteSpace(validadeStr))
            {
                return false;
            }
            if (!int.TryParse(quantidadeStr, out int quantidade) || quantidade <= 0)
            {
                return false;
            }
            if (!DateTime.TryParseExact(validadeStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validade))
            {
                return false;
            }
            var produto = Produtos.FirstOrDefault(p => p.Nome.Equals(nomeProduto.Trim(), StringComparison.OrdinalIgnoreCase));
            produto ??= ListaProdutos.NovoProduto(
                    Produtos,
                    NovoId(Produtos, p => p.Id),
                    nomeProduto.Trim(),
                    "Não especificada",
                    "Não especificada",
                    validade
                );
            if (doacao)
            {
                return ListaEntradas.NovaEntrada(
                    Entradas,
                    NovoId(Entradas, e => e.Id),
                    UsuarioLogado,
                    produto,
                    quantidade,
                    origem
                ) != null;
            }
            else
            {
                return ListaEntradas.NovaEntrada(
                    Entradas,
                    NovoId(Entradas, e => e.Id),
                    UsuarioLogado,
                    produto,
                    quantidade,
                    origem,
                    0,
                    "notafiscal"
                ) != null;
            }
        }

        public bool RegNovaEntrada(string nomeProduto, string quantidadeStr, bool doacao, string origem)
        {
            if (UsuarioLogado is null)
                throw new InvalidOperationException("Usuário não está logado.");
            if (string.IsNullOrWhiteSpace(nomeProduto) || string.IsNullOrWhiteSpace(quantidadeStr) ||
                string.IsNullOrWhiteSpace(origem))
            {
                return false;
            }
            if (!int.TryParse(quantidadeStr, out int quantidade) || quantidade <= 0)
            {
                return false;
            }
            var produto = Produtos.FirstOrDefault(p => p.Nome.Equals(nomeProduto.Trim(), StringComparison.OrdinalIgnoreCase));
            produto ??= ListaProdutos.NovoProduto(
                Produtos,
                NovoId(Produtos, p => p.Id),
                nomeProduto.Trim(),
                "Não especificada",
                "Não especificada"
            );

            if (doacao)
            {
                return ListaEntradas.NovaEntrada(
                    Entradas,
                    NovoId(Entradas, e => e.Id),
                    UsuarioLogado,
                    produto,
                    quantidade,
                    origem
                ) != null;
            }
            else
            {
                return ListaEntradas.NovaEntrada(
                    Entradas,
                    NovoId(Entradas, e => e.Id),
                    UsuarioLogado,
                    produto,
                    quantidade,
                    origem,
                    0,
                    "notafiscal"
                ) != null;
            }
        }
    }
}
