using EstoqueAsilo.Helpers;
using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EstoqueAsilo.Services
{
    public class ServicoAutenticacao
    {
        private readonly string _caminhoNomes = Path.Combine(FileSystem.AppDataDirectory, "Nomes.xml");
        private readonly string _caminhoSenhas = Path.Combine(FileSystem.AppDataDirectory, "Senhas.xml");
        public List<Usuario> Nomes { get; private set; } = [];
        public List<Senha> Senhas { get; private set; } = [];
        public Usuario? UsuarioLogado { get; private set; }

        public async Task Carregar()
        {
            Nomes = ListaCredenciais.CarregarNomes(_caminhoNomes);
            Senhas = await ListaCredenciais.CarregarSenhasAsync(_caminhoSenhas);
        }

        public void Salvar()
        {
            ListaCredenciais.SalvarNomes(_caminhoNomes, Nomes);
            ListaCredenciais.SalvarSenhas(_caminhoSenhas, Senhas);
        }

        public bool Login(string nome, string senha)
        {
            foreach (var s in Senhas)
            {
                if (SenhaHelper.Verificar(senha, s.Hash, s.Salt))
                {
                    var usuario = Nomes.FirstOrDefault(u =>
                        u.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (usuario is null)
                    {
                        var ti = new CultureInfo("pt-BR").TextInfo;
                        string nomeFormatado = ti.ToTitleCase(nome.Trim().ToLower());
                        usuario = new Usuario
                        {
                            Nome = nomeFormatado,
                            Id = IdHelper.NovoId(Nomes, n => n.Id)
                        };
                        Nomes.Add(usuario);
                    }
                    UsuarioLogado = usuario;
                    return true;
                }
            }
            return false;

        }

        public void Logout()
        {
            UsuarioLogado = null;
        }
    }
}
