using EstoqueAsilo.Models;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace EstoqueAsilo.Storage
{
    internal static class LotesRepository
    {
        internal static List<Lote> Carregar(string caminho, Dictionary<int, Produto> porId)
        {
            if (!File.Exists(caminho)) return [];
            var doc = XDocument.Load(caminho);
            List<Lote> lista = [];

            foreach (var l in doc.Root?.Elements("lote") ?? [])
            {
                //Checando validação de cada lote, caso algum dado esteja inválido, o lote é pulado e não é carregado na lista
                if ((int?)l.Attribute("id") is not int id)
                {
                    Debug.WriteLine($"Lote sem id pulado");
                    continue;
                }
                if (!porId.TryGetValue((int?)l.Attribute("produtoId") ?? 0, out var produto))
                {
                    Debug.WriteLine($"Lote {id} pulado: produto não encontrado");
                    continue;
                }
                if (LerData(l.Element("dataValidade")) is not DateTime data)
                {
                    Debug.WriteLine($"Lote {id} pulado: data não é válida");
                    continue;
                }
                if ((int?)l.Element("quantidadeInicial") is not int quantidade || quantidade <= 0)
                {
                    Debug.WriteLine($"Lote {id} pulado: quantidade inicial não é válida");
                    continue;
                }

                //Lote válido, adicionando à lista
                lista.Add(new Lote
                {
                    Id = id,
                    Produto = produto,
                    DataValidade = data,
                    QuantidadeInicial = quantidade
                });
            }
            return lista;
        }

        internal static void Salvar(string caminho, List<Lote> lotes)
        {
            var doc = new XDocument(
                new XElement("lotes",
                    lotes.Select(l =>
                        new XElement("lote",
                            new XAttribute("id", l.Id),
                            new XAttribute("produtoId", l.Produto.Id),
                            new XElement("dataValidade", l.DataValidade.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                            new XElement("quantidadeInicial", l.QuantidadeInicial)
                        )
                    )
                )
            );
            var temp = caminho + ".tmp";
            doc.Save(temp);
            File.Move(temp, caminho, overwrite: true);
        }
        private static DateTime? LerData(XElement? el)
        {
            return DateTime.TryParseExact(el?.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                     DateTimeStyles.None, out var data)
                ? data
                : null;
        }
    }
}
