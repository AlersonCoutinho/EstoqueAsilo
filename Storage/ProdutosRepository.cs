using EstoqueAsilo.Enums;
using EstoqueAsilo.Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace EstoqueAsilo.Storage
{
    public static class ProdutosRepository
    {
        public static List<Produto> Carregar(string caminho)
        {
            if (!File.Exists(caminho)) return [];
            var doc = XDocument.Load(caminho);
            List<Produto> lista = [];

            foreach (var p in doc.Root?.Elements("produto") ?? [])
            {
                //Checando validação de cada produto, caso algum dado esteja inválido, o produto é pulado e não é carregado na lista
                if ((int?)p.Attribute("id") is not int id)
                {
                    Debug.WriteLine($"Produto sem id pulado");
                    continue;
                }
                if (!Enum.TryParse<Categoria>((string?)p.Attribute("categoria"), true, out var categoria)
                    || !Enum.IsDefined(categoria))
                {
                    Debug.WriteLine($"Produto {id} pulado: tipo de produto não encontrado");
                    continue;
                }
                if ((string?)p.Element("nome") is not string nome || string.IsNullOrWhiteSpace(nome))
                {
                    Debug.WriteLine($"Produto {id} pulado: nome não é válido");
                    continue;
                }
                if ((string?)p.Element("unidade") is not string unidade || string.IsNullOrWhiteSpace(unidade))
                {
                    Debug.WriteLine($"Produto {id} pulado: unidade não é válida");
                    continue;
                }

                //Produto válido, adicionando à lista
                lista.Add(new Produto
                {
                    Id = id,
                    Categoria = categoria,
                    Nome = nome.Trim(),
                    Unidade = unidade,
                    QuantidadeMinima = (int?)p.Element("quantidadeMinima")
                });
            }
            return lista;
        }

        public static void Salvar(string caminho, List<Produto> produtos)
        {
            var doc = new XDocument(
                new XElement("produtos",
                    produtos.Select(p =>
                        new XElement("produto",
                            new XAttribute("id", p.Id),
                            new XAttribute("categoria", p.Categoria.ToString()),
                            new XElement("nome", p.Nome.Trim()),
                            new XElement("unidade", p.Unidade),
                            p.QuantidadeMinima != null ? new XElement("quantidadeMinima", p.QuantidadeMinima) : null
                        )
                    )
                )
            );
            var temp = caminho + ".tmp";
            doc.Save(temp);
            File.Move(temp, caminho, overwrite: true);
        }
    }
}
