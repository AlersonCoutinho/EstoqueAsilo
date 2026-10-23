using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Xml.Linq;

namespace EstoqueAsilo.Services
{
    public class ListaProdutos
    {
        public static List<Produto> Carregar(string caminho)
        {
            if (!File.Exists(caminho)) return [];
            {
                var doc = XDocument.Load(caminho);

                return [.. doc.Root!.Elements("produto").Select(p =>
                {
                    var e = p.Element("entrada");
                    var s = p.Element("saida");

                    return new Produto
                    {
                        Id = (int?)p.Attribute("id") ?? 0,
                        Categoria  = (string?)p.Attribute("categoria") ?? "",
                        Nome = (string?)p.Element("nome") ?? "",
                        Unidade = (string?)p.Element("unidade") ?? "",
                        DatasValidade = [.. p.Element("datasValidade")?
                            .Elements("dataValidade")
                            .Select(x => LerData(x))
                            .Where(d => d.HasValue)
                            .Select(d => d!.Value) ?? []]
                    };
                }) ?? []];
            }
        }

        public static void Salvar(string caminho, List<Produto> produtos)
        {
            var doc = new XDocument(
                new XElement("produtos",
                    produtos.Select(p =>
                        new XElement("produto",
                            new XAttribute("id", p.Id),
                            new XAttribute("categoria", p.Categoria),
                            new XElement("nome", p.Nome),
                            new XElement("unidade", p.Unidade),
                            p.DatasValidade != null ? new XElement("datasValidade",
                                p.DatasValidade.Select(d =>
                                    new XElement("dataValidade", d.ToString("yyyy-MM-dd"))
                                )
                            ) : null

                        )
                    )
                )
            );
            doc.Save(caminho);
        }

        internal static Produto NovoProduto(List<Produto> produtos,int id, string nomeProduto, string categoria, string unidade)
        {
            Produto produto = new()
            {
                Id = id,
                Nome = nomeProduto.Trim(),
                Categoria = categoria,
                Unidade = unidade
            };
            produtos.Add(produto);
            return produto;
        }

        internal static Produto NovoProduto(List<Produto> produtos, int id, string nomeProduto, string categoria, string unidade, DateTime validade)
        {
            Produto produto = new()
            {
                Id = id,
                Nome = nomeProduto.Trim(),
                Categoria = categoria,
                Unidade = unidade,
                DatasValidade = new List<DateTime> { validade }
            };
            produtos.Add(produto);
            return produto;
        }

        private static DateTime? LerData(XElement? el)
        {
            var texto = el?.Value;
            return DateTime.TryParse(texto, System.Globalization.CultureInfo.InvariantCulture,
                                     System.Globalization.DateTimeStyles.None, out var data)
                ? data
                : null;
        }
    }
}
