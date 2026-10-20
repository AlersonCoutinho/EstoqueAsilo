using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace EstoqueAsilo.Services
{
    internal class ListaUsuarios
    {
        internal static List<Usuario> Carregar(string caminho)
        {
            if (!File.Exists(caminho)) return [];
            var doc = XDocument.Load(caminho);

            return [.. doc.Root!.Elements("usuario").Select(u =>
            {
                return new Usuario
                {
                    Admin = (bool)u.Attribute("admin")!,
                    Nome = (string)u.Element("nome")!,
                    Hash = (string)u.Element("hash")!,
                    Salt = (string)u.Element("salt")!
                };
            })];
        }

        internal static void Salvar(string caminho, List<Usuario> usuarios)
        {
            var doc = new XDocument(
                new XElement("usuarios",
                    usuarios.Select(u =>
                        new XElement("usuario",
                            new XAttribute("admin", u.Admin),
                            new XElement("nome", u.Nome),
                            new XElement("hash", u.Hash),
                            new XElement("salt", u.Salt)
                        )
                    )
                )
            );
            doc.Save(caminho);
        }
    }
}
