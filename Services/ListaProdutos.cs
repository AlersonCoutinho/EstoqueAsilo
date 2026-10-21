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
                        Id           = (int?)p.Attribute("id") ?? 0,
                        Categoria    = (string?)p.Attribute("categoria") ?? "",
                        Nome         = (string?)p.Element("nome") ?? "",
                        Unidade      = (string?)p.Element("unidade") ?? "",
                        DataValidade = LerData(p.Element("dataValidade")),

                        Entrada = new Entrada
                        {
                            Id           = (int?)e?.Attribute("id") ?? 0,
                            Doacao       = (bool?)e?.Attribute("doacao") ?? false,
                            Quantidade   = (int?)e?.Element("quantidade") ?? 0,
                            Doador       = (string?)e?.Element("doador"),
                            Fornecedor   = (string?)e?.Element("fornecedor"),
                            Preco        = (decimal?)e?.Element("preco"),
                            IdNotaFiscal = (string?)e?.Element("idNotaFiscal")
                        },

                        Saida = s == null ? null : new Saida
                        {
                            Id         = (int?)s.Attribute("id") ?? 0,
                            Doacao     = (bool?)s.Attribute("doacao") ?? false,
                            Quantidade = (int?)s.Element("quantidade") ?? 0,
                            Doado      = (string?)s.Element("doado")
                        }
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
                            p.DataValidade != null ? new XElement("dataValidade",p.DataValidade.Value.ToString("yyyy-MM-dd")) : null,
                            new XElement("entrada",
                                new XAttribute("id", p.Entrada.Id),
                                new XAttribute("doacao", p.Entrada.Doacao),
                                new XElement("quantidade", p.Entrada.Quantidade),
                                p.Entrada.Doador != null ? new XElement("doador", p.Entrada.Doador) : null,
                                p.Entrada.Fornecedor != null ? new XElement("fornecedor", p.Entrada.Fornecedor) : null,
                                p.Entrada.Preco != null ? new XElement("preco", p.Entrada.Preco) : null,
                                p.Entrada.IdNotaFiscal != null ? new XElement("idNotaFiscal", p.Entrada.IdNotaFiscal) : null
                            ),
                            p.Saida != null ? new XElement("saida",
                                new XAttribute("id", p.Saida.Id),
                                new XAttribute("doacao", p.Saida.Doacao),
                                new XElement("quantidade", p.Saida.Quantidade),
                                p.Saida.Doado != null ? new XElement("doado", p.Saida.Doado) : null
                            ) : null

                        )
                    )
                )
            );
            doc.Save(caminho);
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
