using EstoqueAsilo.Data;
using EstoqueAsilo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.Services
{
    public class ServicoEstoque
    {
        private readonly string _caminhoProdutos = Path.Combine(FileSystem.AppDataDirectory, "Produtos.xml");
        private readonly string _caminhoUsuarios = Path.Combine(FileSystem.AppDataDirectory, "Usuarios.xml");
        public List<Produto> Produtos { get; private set; } = new();
        public List<Usuario> Usuarios { get; private set; } = new();
        public void Carregar() { 
            Produtos = ListaProdutos.Carregar(_caminhoProdutos);
            Usuarios = ListaUsuarios.Carregar(_caminhoUsuarios);
        }
        public void Salvar()
        {
            ListaProdutos.Salvar(_caminhoProdutos, Produtos);
            ListaUsuarios.Salvar(_caminhoUsuarios, Usuarios);
        }
    }
}
