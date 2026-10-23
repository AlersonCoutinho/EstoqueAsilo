using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace EstoqueAsilo.Services
{
    internal class ListaEntradas
    {
        public static List<Entrada> Carregar(string caminho, Dictionary<int, Produto> porId, Dictionary<int,Usuario> userPorId)
        {
            if (!File.Exists(caminho)) return [];
            var doc = XDocument.Load(caminho);
            List<Entrada> lista = [];

            foreach (var s in doc.Root?.Elements("saida") ?? [])
            {
                if (!porId.TryGetValue((int?)s.Attribute("produtoId") ?? 0, out var produto)) continue;
                if (!userPorId.TryGetValue((int?)s.Attribute("produtoId") ?? 0, out var usuario)) continue;
                lista.Add(new Entrada
                {
                    Id = (int?)s.Attribute("id") ?? 0,
                    Usuario = usuario,
                    Produto = produto,
                    Doacao = (bool?)s.Attribute("doacao") ?? false,
                    Quantidade = (int?)s.Element("quantidade") ?? 0,
                    Doador = (string?)s.Element("doador"),
                    Fornecedor = (string?)s.Element("fornecedor"),
                    Preco = (decimal?)s.Element("preco"),
                    IdNotaFiscal = (string?)s.Element("idNotaFiscal")
                });
            }

            return lista;
        }


        public static void Salvar(string caminho, List<Entrada> entradas)
        {
            var doc = new XDocument(
                new XElement("entradas",
                    entradas.Select(p =>
                        new XElement("entrada",
                                new XAttribute("id", p.Id),
                                new XAttribute("usuarioId", p.Usuario.Id),
                                new XAttribute("produtoId", p.Produto.Id),
                                new XAttribute("doacao", p.Doacao),
                                new XElement("quantidade", p.Quantidade),
                                p.Doador != null ? new XElement("doador", p.Doador) : null,
                                p.Fornecedor != null ? new XElement("fornecedor", p.Fornecedor) : null,
                                p.Preco != null ? new XElement("preco", p.Preco) : null,
                                p.IdNotaFiscal != null ? new XElement("idNotaFiscal", p.IdNotaFiscal) : null
                        )
                    )
                )
            );
            doc.Save(caminho);
        }

        internal static Entrada NovaEntrada(List<Entrada> entradas, int id, Usuario usuario, 
            Produto produto, int quantidade, string fornecedor, decimal preco, string idNotaFiscal)
        {
            Entrada entrada = new()
            {
                Id = id,
                Usuario = usuario,
                Produto = produto,
                Doacao = false,
                Quantidade = quantidade,
                Fornecedor = fornecedor,
                Preco = preco,
                IdNotaFiscal = idNotaFiscal
            };
            entradas.Add(entrada);
            return entrada;
        }

        internal static Entrada NovaEntrada(List<Entrada> entradas, int id, Usuario usuario, 
            Produto produto, int quantidade, string doador)
        {
            Entrada entrada = new()
            {
                Id = id,
                Usuario = usuario,
                Produto = produto,
                Doacao = true,
                Quantidade = quantidade,
                Doador = doador
            };
            entradas.Add(entrada);
            return entrada;
        }
    }
}
