using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EstoqueAsilo.ViewModels
{
    public partial class EntradaViewModel : ObservableObject
    {
        private readonly ServicoEstoque servicoEstoque;
        [ObservableProperty]
        private string nomeProduto = string.Empty;

        [ObservableProperty]
        private string quantidade = string.Empty;

        [ObservableProperty]
        private string doacao = string.Empty;

        [ObservableProperty]
        private string origem = string.Empty;

        [ObservableProperty]
        private string validade = string.Empty;

        public EntradaViewModel(ServicoEstoque servicoEstoque) => this.servicoEstoque = servicoEstoque;

        [RelayCommand]
        private async Task RegEntrada()
        {
            bool doacaoBool = Doacao.Equals("sim");
            servicoEstoque.RegNovaEntrada(NomeProduto, Quantidade, doacaoBool, Origem, Validade);
        }
    }
}
