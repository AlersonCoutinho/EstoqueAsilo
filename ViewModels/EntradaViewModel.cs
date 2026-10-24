using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EstoqueAsilo.Services;

namespace EstoqueAsilo.ViewModels
{
    public partial class EntradaViewModel : ObservableObject
    {
        private readonly ServicoEstoque servicoEstoque;

        public ObservableCollection<string> TiposOrigem { get; } = new() { "Compra", "Doação" };
        public ObservableCollection<string> Categorias { get; } = new() { "Alimentos", "Limpeza", "Almoxarifado" };
        public ObservableCollection<string> Unidades { get; } = new() { "Unidades", "Litros", "Quilos", "Gramas" };

        [ObservableProperty]
        private string nomeProduto = string.Empty;

        [ObservableProperty]
        private string quantidade = string.Empty;
        [ObservableProperty] 
        private string unidadeSelecionada = string.Empty;
        [ObservableProperty]
        private bool ehDoacao = false;
        [ObservableProperty]
        private string categoriaSelecionada = string.Empty;
        [ObservableProperty]
        private string detalheOrigem = string.Empty;
        [ObservableProperty]
        private DateTime dataValidade = DateTime.Today;
        [ObservableProperty]
        private bool mostrarValidade = true;

        public EntradaViewModel(ServicoEstoque servicoEstoque)
        {
            this.servicoEstoque = servicoEstoque;
            CategoriaSelecionada = Categorias[0];
            UnidadeSelecionada = Unidades[0];
        }

        [RelayCommand]
        private async Task RegEntrada()
        {
            if (string.IsNullOrWhiteSpace(NomeProduto) || string.IsNullOrWhiteSpace(Quantidade))
                return;

            string validadeStr = DataValidade.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            servicoEstoque.RegNovaEntrada(NomeProduto, Quantidade, ehDoacao, DetalheOrigem, validadeStr);

            NomeProduto = string.Empty;
            Quantidade = string.Empty;
            DetalheOrigem = string.Empty;

            await Task.CompletedTask;
        }
        partial void OnCategoriaSelecionadaChanged(string value)
        {
            MostrarValidade = (value == "Alimentos");
        }
    }
}
