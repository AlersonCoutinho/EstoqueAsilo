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
                    var d = p.Element("details");
                    var qtd = p.Element("quantidade")!;

                    return new Produto
                    {
                        Id = (int)p.Attribute("id")!,
                        Categoria = (string)p.Attribute("categoria")!,
                        Nome = (string)p.Element("nome")!,
                        Quantidade = (decimal)qtd,
                        Unidade = (string?)qtd.Attribute("unidade") ?? "",
                        Doacao = (bool)p.Element("doacao")!,

                        Doador = (string?)d?.Element("doador"),
                        DataValidade = (DateTime?)d?.Element("dataValidade"),
                        Preco = (decimal?)d?.Element("preco"),
                        IdNotaFiscal = (string?)d?.Element("idNotaFiscal")
                    };
                })];
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
                            new XElement("quantidade", new XAttribute("unidade", p.Unidade), p.Quantidade),
                            new XElement("doacao", p.Doacao),
                            new XElement("details",
                                p.Doador is not null ? new XElement("doador", p.Doador) : null,
                                p.DataValidade is not null ? new XElement("dataValidade", p.DataValidade) : null,
                                p.Preco is not null ? new XElement("preco", p.Preco) : null,
                                p.IdNotaFiscal is not null ? new XElement("idNotaFiscal", p.IdNotaFiscal) : null
                            )
                        )
                    )
                )
            );
            doc.Save(caminho);
        }
    }
}
