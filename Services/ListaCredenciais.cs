using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace EstoqueAsilo.Services
{
    internal class ListaCredenciais
    {
        internal static List<Usuario> CarregarNomes(string caminho)
        {
            if (!File.Exists(caminho)) return [];
            var doc = XDocument.Load(caminho);

            return [.. doc.Root!.Elements("usuario").Select(u =>
            {
                return new Usuario
                {
                    Id = (int)u.Attribute("id")!,
                    Nome = (string)u.Element("nome")!
                };
            })];
        }

        internal static void SalvarNomes(string caminho, List<Usuario> usuarios)
        {
            var doc = new XDocument(
                new XElement("usuarios",
                    usuarios.Select(u =>
                        new XElement("usuario",
                            new XAttribute("id", u.Id),
                            new XElement("nome", u.Nome)
                        )
                    )
                )
            );
            doc.Save(caminho);
        }

        internal static async Task<List<Senha>> CarregarSenhasAsync(string caminho)
        {
            await GarantirArquivoAsync("Senhas.xml", caminho);
            var doc = XDocument.Load(caminho);

            return [.. doc.Root!.Elements("senha").Select(u =>
            {
                return new Senha
                {
                    Admin = (bool)u.Attribute("admin")!,
                    Hash = (string)u.Element("hash")!,
                    Salt = (string)u.Element("salt")!
                };
            }) ?? []];
        }

        internal static async Task GarantirArquivoAsync(string nomeAsset, string destino)
        {
            if (File.Exists(destino))
                return;

            using var origem = await FileSystem.OpenAppPackageFileAsync(nomeAsset);
            using var saida = File.Create(destino);
            await origem.CopyToAsync(saida);
        }
        internal static void SalvarSenhas(string caminho, List<Senha> usuarios)
        {
            var doc = new XDocument(
                new XElement("senhas",
                    usuarios.Select(u =>
                        new XElement("senha",
                            new XAttribute("admin", u.Admin),
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
