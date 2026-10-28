using EstoqueAsilo.Enums;
using EstoqueAsilo.Models;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace EstoqueAsilo.Storage
{
    internal static class MovimentacoesRepository
    {
        public static List<Movimentacao> Carregar(string caminho, Dictionary<int, Produto> produtosPorId, Dictionary<int, Usuario> userPorId, Dictionary<int, Lote> lotePorId)
        {
            if (!File.Exists(caminho)) return [];
            var doc = XDocument.Load(caminho);
            List<Movimentacao> lista = [];

            foreach (var m in doc.Root?.Elements("movimentacao") ?? [])
            {
                //Checando validação de cada movimentação, caso algum dado esteja inválido, a movimentação é pulada e não é carregada na lista
                if ((int?)m.Attribute("id") is not int id)
                {
                    Debug.WriteLine($"Movimentação sem id pulada");
                    continue;
                }
                if (!produtosPorId.TryGetValue((int?)m.Attribute("produtoId") ?? 0, out var produto))
                {
                    Debug.WriteLine($"Movimentação {id} pulada: produto não encontrado");
                    continue;
                }
                if (!userPorId.TryGetValue((int?)m.Attribute("usuarioId") ?? 0, out var usuario))
                {
                    Debug.WriteLine($"Movimentação {id} pulada: usuário não encontrado");
                    continue;
                }
                if (!Enum.TryParse<TipoMovimentacao>((string?)m.Attribute("tipoMovimentacao"), true, out var tipo)
                    || !Enum.IsDefined(tipo))
                {
                    Debug.WriteLine($"Movimentação {id} pulada: tipo de movimentação não encontrado");
                    continue;
                }
                if (LerData(m.Element("data")) is not DateTime data)
                {
                    Debug.WriteLine($"Movimentação {id} pulada: data não é válida");
                    continue;
                }
                if ((int?)m.Element("quantidade") is not int quantidade || quantidade <= 0)
                {
                    Debug.WriteLine($"Movimentação {id} pulada: quantidade não é válida");
                    continue;
                }
                Lote? lote = null;
                int? loteId = (int?)m.Attribute("loteId");
                if (loteId is not null && (!lotePorId.TryGetValue(loteId.Value, out lote) || produto != lote.Produto))
                {
                    Debug.WriteLine($"Movimentação {id} pulada: lote {loteId} não encontrado ou produtos não são compatíveis");
                    continue;
                }

                //Movimentação válida, adicionando à lista
                lista.Add(new Movimentacao
                {
                    Id = id,
                    TipoMovimentacao = tipo,
                    Usuario = usuario,
                    Produto = produto,
                    Lote = lote,
                    Quantidade = quantidade,
                    Data = data,
                    IsDoacao = (bool?)m.Attribute("isDoacao") ?? false,
                    Contraparte = (string?)m.Element("contraparte"),
                    Preco = (decimal?)m.Element("preco"),
                    IdNotaFiscal = (string?)m.Element("idNotaFiscal")
                });
            }

            return lista;
        }
        public static void Salvar(string caminho, List<Movimentacao> movimentacoes)
        {
            var doc = new XDocument(
                new XElement("movimentacoes",
                    movimentacoes.Select(m =>
                        new XElement("movimentacao",
                                new XAttribute("id", m.Id),
                                new XAttribute("tipoMovimentacao", m.TipoMovimentacao.ToString()),
                                new XAttribute("usuarioId", m.Usuario.Id),
                                new XAttribute("produtoId", m.Produto.Id),
                                m.Lote != null ? new XAttribute("loteId", m.Lote.Id) : null,
                                new XAttribute("isDoacao", m.IsDoacao),
                                new XElement("quantidade", m.Quantidade),
                                new XElement("data", m.Data.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture)),
                                m.Contraparte != null ? new XElement("contraparte", m.Contraparte) : null,
                                m.Preco != null ? new XElement("preco", m.Preco) : null,
                                m.IdNotaFiscal != null ? new XElement("idNotaFiscal", m.IdNotaFiscal) : null
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
            return DateTime.TryParseExact(el?.Value, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture,
                                     DateTimeStyles.None, out var data)
                ? data
                : null;
        }
    }
}
