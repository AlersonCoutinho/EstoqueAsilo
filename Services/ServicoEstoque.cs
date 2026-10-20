using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EstoqueAsilo.Services
{
    public class ServicoEstoque
    {
        private readonly string _caminhoProdutos = Path.Combine(FileSystem.AppDataDirectory, "Produtos.xml");
        private readonly string _caminhoUsuarios = Path.Combine(FileSystem.AppDataDirectory, "Usuarios.xml");
        public List<Produto> Produtos { get; private set; } = [];
        public List<Usuario> Usuarios { get; private set; } = [];
        public Usuario? UsuarioLogado { get; private set; }

        public void Carregar()
        {
            Produtos = ListaProdutos.Carregar(_caminhoProdutos);
            Usuarios = ListaUsuarios.Carregar(_caminhoUsuarios);
        }
        public void Salvar()
        {
            ListaProdutos.Salvar(_caminhoProdutos, Produtos);
            ListaUsuarios.Salvar(_caminhoUsuarios, Usuarios);
        }

        public bool Login(string nome, string senha)
        {
            if (!Usuarios.Any()) Debug.WriteLine("Lista Usuarios vazia");
            var usuario = Usuarios.FirstOrDefault(u =>
                u.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase));

            if (usuario is null || !SenhaHelper.Verificar(senha, usuario.Hash, usuario.Salt)) return false;

            UsuarioLogado = usuario;
            return true;
        }

        public void Logout()
        {
            UsuarioLogado = null;
        }

        public bool CadastrarUsuario(string nome, string senha, bool admin)
        {
            if (Usuarios.Any(u => u.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase))) return false;
            var (hash, salt) = SenhaHelper.Gerar(senha);
            Usuarios.Add(new Usuario
            {
                Nome = nome.Trim(),
                Admin = admin,
                Hash = hash,
                Salt = salt
            });
            return true;
        }
    }
}
