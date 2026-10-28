using EstoqueAsilo.Helpers;
using EstoqueAsilo.Models;
using EstoqueAsilo.Storage;
using System.Globalization;

namespace EstoqueAsilo.Services
{
    public class AuthService
    {
        private readonly string _caminhoUsuarios = Path.Combine(FileSystem.AppDataDirectory, "Usuarios.xml");
        private readonly string _caminhoSenhas = Path.Combine(FileSystem.AppDataDirectory, "Senhas.xml");
        internal List<Usuario> Usuarios { get; private set; } = [];
        internal List<Senha> Senhas { get; private set; } = [];
        internal Usuario? UsuarioLogado { get; private set; }

        internal async Task Carregar()
        {
            Usuarios = CredenciaisRepository.CarregarUsuarios(_caminhoUsuarios);
            Senhas = await CredenciaisRepository.CarregarSenhasAsync(_caminhoSenhas);
        }

        internal void Salvar()
        {
            CredenciaisRepository.SalvarUsuarios(_caminhoUsuarios, Usuarios);
            CredenciaisRepository.SalvarSenhas(_caminhoSenhas, Senhas);
        }

        internal bool Login(string nome, string senha)
        {
            foreach (var s in Senhas)
            {
                if (SenhaHelper.Verificar(senha, s.Hash, s.Salt))
                {
                    var usuario = Usuarios.FirstOrDefault(u =>
                        u.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (usuario is null)
                    {
                        var ti = new CultureInfo("pt-BR").TextInfo;
                        string nomeFormatado = ti.ToTitleCase(nome.Trim().ToLower());
                        usuario = new Usuario
                        {
                            Nome = nomeFormatado,
                            Id = IdHelper.NovoId(Usuarios, n => n.Id)
                        };
                        Usuarios.Add(usuario);
                    }
                    UsuarioLogado = usuario;
                    return true;
                }
            }
            return false;

        }

        internal void Logout()
        {
            UsuarioLogado = null;
        }
    }
}
