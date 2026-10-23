using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace EstoqueAsilo.Services
{
    internal class ListaSaidas
    {
        public static List<Saida> Carregar(string caminho, Dictionary<int, Produto> porId, Dictionary<int, Usuario> userPorId)
        {
            if (!File.Exists(caminho)) return [];
            var doc = XDocument.Load(caminho);
            List<Saida> lista = [];

            foreach (var s in doc.Root?.Elements("saida") ?? [])
            {
                if (!porId.TryGetValue((int?)s.Attribute("produtoId") ?? 0, out var produto)) continue;
                if (!userPorId.TryGetValue((int?)s.Attribute("produtoId") ?? 0, out var usuario)) continue;
                lista.Add(new Saida
                {
                    Id = (int?)s.Attribute("id") ?? 0,
                    Usuario = usuario,
                    Produto = produto,
                    Doacao = (bool?)s.Attribute("doacao") ?? false,
                    Quantidade = (int?)s.Element("quantidade") ?? 0,
                    Doado = (string?)s.Element("doado")
                });
            }

            return lista;
        }

        public static void Salvar(string caminho, List<Saida> saidas)
        {
            var doc = new XDocument(
                new XElement("saidas",
                    saidas.Select(s =>
                        new XElement("saida",
                            new XAttribute("id", s.Id),
                            new XAttribute("usuarioId", s.Usuario.Id),
                            new XAttribute("produtoId", s.Produto.Id),
                            new XAttribute("doacao", s.Doacao),
                            new XElement("quantidade", s.Quantidade),
                            s.Doado != null ? new XElement("doado", s.Doado) : null
                        )
                    )
                )
            );
            doc.Save(caminho);
        }
    }
}
