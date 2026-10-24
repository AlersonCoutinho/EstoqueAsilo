using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Globalization;
using EstoqueAsilo.Helpers;

namespace EstoqueAsilo.Services
{
    public class ServicoEstoque
    {
        private readonly string _caminhoEntradas = Path.Combine(FileSystem.AppDataDirectory, "Entradas.xml");
        private readonly string _caminhoSaidas = Path.Combine(FileSystem.AppDataDirectory, "Saidas.xml");
        private readonly string _caminhoProdutos = Path.Combine(FileSystem.AppDataDirectory, "Produtos.xml");
        public List<Produto> Produtos { get; private set; } = [];
        public List<Entrada> Entradas { get; private set; } = [];
        public List<Saida> Saidas { get; private set; } = [];
        private readonly ServicoAutenticacao _auth;

        public ServicoEstoque(ServicoAutenticacao auth) => _auth = auth;

        public void Carregar()
        {
            Produtos = ListaProdutos.Carregar(_caminhoProdutos);
            var porId = Produtos.ToDictionary(p => p.Id);
            var userPorId = _auth.Nomes.ToDictionary(u => u.Id);
            Entradas = ListaEntradas.Carregar(_caminhoEntradas, porId, userPorId);
            Saidas = ListaSaidas.Carregar(_caminhoSaidas, porId, userPorId);
        }
        public void Salvar()
        {
            ListaProdutos.Salvar(_caminhoProdutos, Produtos);
            ListaEntradas.Salvar(_caminhoEntradas, Entradas);
            ListaSaidas.Salvar(_caminhoSaidas, Saidas);
        }

        public bool RegNovaEntrada(string nomeProduto, string quantidadeStr, bool doacao, string origem, string validadeStr)
        {
            if (_auth.UsuarioLogado is null)
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
                    IdHelper.NovoId(Produtos, p => p.Id),
                    nomeProduto.Trim(),
                    "Não especificada",
                    "Não especificada",
                    validade
                );
            if (doacao)
            {
                return ListaEntradas.NovaEntrada(
                    Entradas,
                    IdHelper.NovoId(Entradas, e => e.Id),
                    _auth.UsuarioLogado,
                    produto,
                    quantidade,
                    origem
                ) != null;
            }
            else
            {
                return ListaEntradas.NovaEntrada(
                    Entradas,
                    IdHelper.NovoId(Entradas, e => e.Id),
                    _auth.UsuarioLogado,
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
            if (_auth.UsuarioLogado is null)
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
                IdHelper.NovoId(Produtos, p => p.Id),
                nomeProduto.Trim(),
                "Não especificada",
                "Não especificada"
            );

            if (doacao)
            {
                return ListaEntradas.NovaEntrada(
                    Entradas,
                    IdHelper.NovoId(Entradas, e => e.Id),
                    _auth.UsuarioLogado,
                    produto,
                    quantidade,
                    origem
                ) != null;
            }
            else
            {
                return ListaEntradas.NovaEntrada(
                    Entradas,
                    IdHelper.NovoId(Entradas, e => e.Id),
                    _auth.UsuarioLogado,
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
