using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace EstoqueAsilo.Data
{
    internal class ListaUsuarios
    {
        internal static List<Usuario> Carregar(string caminho)
        {
            if (!File.Exists(caminho)) return new List<Usuario>();
            var doc = XDocument.Load(caminho);

            return doc.Root!.Elements("usuario").Select(u =>
            {
                return new Usuario
                {
                    Nome = (string)u.Attribute("nome")!,
                    Admin = (bool)u.Attribute("admin")!,
                    Email = (string)u.Attribute("email")!,
                    Senha = (string)u.Attribute("senha")!
                };
            }).ToList();
        }

        internal static void Salvar(string caminho, List<Usuario> usuarios)
        {
            var doc = new XDocument(
                new XElement("usuarios",
                    usuarios.Select(u =>
                        new XElement("usuario",
                            new XAttribute("admin", u.Admin),
                            new XElement("nome", u.Nome),
                            new XElement("email", u.Email),
                            new XElement("senha", u.Senha)
                        )
                    )
                )
            );
            doc.Save(caminho);
        }
    }
}
