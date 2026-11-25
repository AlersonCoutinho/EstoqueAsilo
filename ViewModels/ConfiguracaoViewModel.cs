using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;
using System.Collections.ObjectModel;

namespace EstoqueAsilo.ViewModels
{
    public enum SecaoConfig { Nenhuma, Senha, Usuario, ExcluirDados }

    public partial class ConfiguracaoViewModel(AuthService auth, EstoqueService estoque) : ObservableObject
    {
        private readonly AuthService _auth = auth;
        private readonly EstoqueService _estoque = estoque;

        #region Mudança de janela
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PainelAberto), nameof(MostrarSenha),
                                  nameof(MostrarUsuario), nameof(MostrarExcluir))]
        public partial SecaoConfig SecaoAtual { get; set; } = SecaoConfig.Nenhuma;

        public bool PainelAberto => SecaoAtual != SecaoConfig.Nenhuma;
        public bool MostrarSenha => SecaoAtual == SecaoConfig.Senha;
        public bool MostrarUsuario => SecaoAtual == SecaoConfig.Usuario;
        public bool MostrarExcluir => SecaoAtual == SecaoConfig.ExcluirDados;

        [RelayCommand] private void AbrirSenha() => Abrir(SecaoConfig.Senha);
        [RelayCommand] private void AbrirUsuario() => Abrir(SecaoConfig.Usuario);
        [RelayCommand] private void AbrirExcluir() => Abrir(SecaoConfig.ExcluirDados);
        [RelayCommand] private void Fechar() => Abrir(SecaoConfig.Nenhuma);

        private void Abrir(SecaoConfig secao)
        {
            LimparCampos();
            SecaoAtual = secao;
            if (secao == SecaoConfig.Usuario)
                UpdateOpcoesUsuarios();
        }
        #endregion


        #region Campos Senha
        [ObservableProperty]
        public partial string SenhaAtual { get; set; } = "";
        [ObservableProperty]
        public partial string NovaSenha { get; set; } = "";
        [ObservableProperty]
        public partial string ConfirmarSenha { get; set; } = "";
        #endregion

        // -- Campo Usuário
        public ObservableCollection<string> OpcoesUsuario { get; } = [];
        [ObservableProperty]
        public partial string NovoUsuario { get; set; } = "";
        [ObservableProperty]
        public partial string UsuarioSelecionado { get; set; } = "";

        // -- Campo de confirmação para exclusão de dados
        [ObservableProperty]
        public partial string TextoConfirmacao { get; set; } = "";

        // -- Mensagem
        [ObservableProperty]
        public partial string Mensagem { get; set; } = "";
        [ObservableProperty]
        public partial bool TemErro { get; set; }
        [ObservableProperty]
        public partial bool MostrarMensagem { get; set; }

        private void Mostrar(string texto, bool erro)
        {
            Mensagem = texto;
            TemErro = erro;
            MostrarMensagem = true;
        }

        private void LimparCampos()
        {
            SenhaAtual = NovaSenha = ConfirmarSenha = NovoUsuario = TextoConfirmacao = "";
            MostrarMensagem = false;
        }

        // -- Metodos Senha
        [RelayCommand]
        private async Task CriarSenha()
        {
            if (!AutenticarSenha()) return;

            bool ok = await MostrarJanelaConf("Criar nova senha?",
                "Isso vai CRIAR uma nova senha. Continuar?",
                "Criar", "Cancelar"
                );
            if (!ok) return;

            if (!_auth.RegSenha(NovaSenha))
            {
                Mostrar("Erro ao cadastrar senha.", true);
                return;
            }
            LimparCampos();
            Mostrar("Senha cadastrada com sucesso.", false);
        }

        [RelayCommand]
        private async Task AlterarSenha()
        {
            if (!AutenticarSenha()) return;

            bool ok = await MostrarJanelaConf("Alterar a senha?",
                "Isso vai ALTERAR a senha inserida. Continuar?",
                "Alterar", "Cancelar"
                );
            if (!ok) return;

            if (!_auth.AltSenha(SenhaAtual, NovaSenha))
            {
                Mostrar("Erro ao alterar senha.", true);
                return;
            }
            LimparCampos();
            Mostrar("Senha alterada com sucesso.", false);
        }
        [RelayCommand]
        private void AlterarUsuario()
        {
            if (OpcoesUsuario.Count == 0 || UsuarioSelecionado == "Nenhum usuário cadastrado")
            {
                Mostrar("Nenhum usuário para alterar.", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(NovoUsuario))
            {
                Mostrar("Digite um nome de usuário.", true);
                return;
            }

            if (!_auth.AltUsuario(UsuarioSelecionado, NovoUsuario.Trim()))
            {
                Mostrar("Erro ao alterar usuario.", true);
                return;
            }
            LimparCampos();
            UpdateOpcoesUsuarios();
            Mostrar("Usuário alterado.", false);
        }

        [RelayCommand]
        private async Task ExcluirDadosAsync()
        {
            if (!_auth.AutenticarSenha(SenhaAtual))
            {
                Mostrar("Senha atual incorreta.", true);
                return;
            }
            if (TextoConfirmacao != "EXCLUIR")
            {
                Mostrar("Digite EXCLUIR para confirmar.", true);
                return;
            }

            bool ok = await MostrarJanelaConf("Excluir todos os dados",
                "Isso apaga produtos, lotes e movimentações. Não é possível desfazer. Continuar?",
                "Excluir", "Cancelar"
                );
            if (!ok) return;

            if (!_estoque.ExcluirDados())
            {
                Mostrar("Erro ao excluir dados.", true);
                return;
            }

            LimparCampos();
            Mostrar("Todos os dados foram excluídos.", false);
        }

        // -- Método helpers
        private bool AutenticarSenha()
        {
            if (!_auth.AutenticarSenha(SenhaAtual))
            {
                Mostrar("Senha atual incorreta.", true);
                return false;
            }

            if (string.IsNullOrWhiteSpace(NovaSenha) || NovaSenha.Length < 4)
            {
                Mostrar("A nova senha deve ter pelo menos 4 caracteres.", true);
                return false;
            }

            if (NovaSenha != ConfirmarSenha)
            {
                Mostrar("As senhas não coincidem.", true);
                return false;
            }
            return true;
        }

        private void UpdateOpcoesUsuarios()
        {
            OpcoesUsuario.Clear();
            foreach (var usuario in _auth.ListarUsuarios())
                OpcoesUsuario.Add(usuario);
            if (OpcoesUsuario.Count == 0) OpcoesUsuario.Add("Nenhum usuário cadastrado");
            UsuarioSelecionado = OpcoesUsuario[0];
        }

        private static async Task<bool> MostrarJanelaConf(string title, string message, string accept, string cancel)
        {
            var page = Application.Current?.Windows[0]?.Page;
            if (page is null) return false;
            return await page.DisplayAlertAsync(title, message, accept, cancel);
        }
    }
}
