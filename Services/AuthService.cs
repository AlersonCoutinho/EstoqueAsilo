using EstoqueAsilo.Helpers;
using EstoqueAsilo.Models;
using EstoqueAsilo.Storage;

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

        internal bool AutenticarSenha(string senha)
        {
            foreach (var s in Senhas)
            {
                if (SenhaHelper.Verificar(senha, s.Hash, s.Salt)) return true;
            }
            return false;

        }

        internal bool RegSenha(string senha, bool admin = false)
        {
            var (hash, salt) = SenhaHelper.Gerar(senha);
            Senha novaSenha = new()
            {
                Hash = hash,
                Salt = salt,
                Admin = admin
            };
            Senhas.Add(novaSenha);
            return true;
        }
        internal bool AltSenha(string senhaAtual, string senhaNova)
        {
            Senha? senha = null;
            foreach (var s in Senhas)
            {
                if (SenhaHelper.Verificar(senhaAtual, s.Hash, s.Salt))
                {
                    senha = s;
                    break;
                }
            }
            if (senha is null) return false;
            var (hash, salt) = SenhaHelper.Gerar(senhaNova);
            senha.Hash = hash;
            senha.Salt = salt;
            return true;
        }

        internal bool TemUsuario(string nome)
        {
            var usuario = Usuarios.FirstOrDefault(u =>
                        u.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase));
            if (usuario is null)
            {
                return false;
            }
            UsuarioLogado = usuario;
            return true;
        }

        internal bool CadastrarUsuario(string nome)
        {
            Usuario usuario = new()
            {
                Nome = nome,
                Id = IdHelper.NovoId(Usuarios, n => n.Id)
            };
            Usuarios.Add(usuario);

            UsuarioLogado = usuario;
            return true;
        }
        internal bool AltUsuario(string nomeAtual, string nomeNovo)
        {
            var usuario = Usuarios.FirstOrDefault(u =>
                        u.Nome.Equals(nomeAtual.Trim(), StringComparison.OrdinalIgnoreCase));
            if (usuario is null) return false;
            usuario.Nome = nomeNovo;
            return true;
        }

        internal void Logout()
        {
            UsuarioLogado = null;
        }

        // -- Consultas
        internal IEnumerable<string> ListarUsuarios()
        {
            return Usuarios.Select(u => u.Nome).Distinct().OrderBy(u => u).ToList();
        }
    }
}
