using EstoqueAsilo.Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace EstoqueAsilo.Storage
{
    internal static class CredenciaisRepository
    {
        internal static List<Usuario> CarregarUsuarios(string caminho)
        {
            if (!File.Exists(caminho)) return [];
            var doc = XDocument.Load(caminho);
            List<Usuario> lista = [];

            foreach (var u in doc.Root?.Elements("usuario") ?? [])
            {
                if ((int?)u.Attribute("id") is not int id)
                {
                    Debug.WriteLine($"Usuario sem id pulado");
                    continue;
                }
                if ((string?)u.Element("nome") is not string nome || string.IsNullOrWhiteSpace(nome))
                {
                    Debug.WriteLine($"Usuario {id} pulado: nome não é válido");
                    continue;
                }
                lista.Add(new Usuario
                {
                    Id = id,
                    Nome = nome.Trim()
                });
            }
            return lista;
        }

        internal static void SalvarUsuarios(string caminho, List<Usuario> usuarios)
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
            var temp = caminho + ".tmp";
            doc.Save(temp);
            File.Move(temp, caminho, overwrite: true);
        }

        internal static async Task<List<Senha>> CarregarSenhasAsync(string caminho)
        {
            await GarantirArquivoAsync("Senhas.xml", caminho);
            var doc = XDocument.Load(caminho);

            List<Senha> lista = [];

            foreach (var s in doc.Root?.Elements("senha") ?? [])
            {
                if ((string?)s.Element("hash") is not string hash || !EhBase64(hash))
                {
                    Debug.WriteLine($"Senha pulada: hash não é válido");
                    continue;
                }
                if ((string?)s.Element("salt") is not string salt || !EhBase64(salt))
                {
                    Debug.WriteLine($"Senha pulada: salt não é válido");
                    continue;
                }
                lista.Add(new Senha
                {
                    Admin = (bool?)s.Attribute("admin") ?? false,
                    Hash = hash,
                    Salt = salt
                });
            }
            return lista;
        }

        private static async Task GarantirArquivoAsync(string nomeAsset, string destino)
        {
            if (File.Exists(destino))
                return;

            using var origem = await FileSystem.OpenAppPackageFileAsync(nomeAsset);
            using var saida = File.Create(destino);
            await origem.CopyToAsync(saida);
        }
        internal static void SalvarSenhas(string caminho, List<Senha> senhas)
        {
            var doc = new XDocument(
                new XElement("senhas",
                    senhas.Select(s =>
                        new XElement("senha",
                            new XAttribute("admin", s.Admin),
                            new XElement("hash", s.Hash),
                            new XElement("salt", s.Salt)
                        )
                    )
                )
            );
            var temp = caminho + ".tmp";
            doc.Save(temp);
            File.Move(temp, caminho, overwrite: true);
        }

        private static bool EhBase64(string texto)
        {
            return Convert.TryFromBase64String(texto, new byte[texto.Length], out _);
        }
    }
}
