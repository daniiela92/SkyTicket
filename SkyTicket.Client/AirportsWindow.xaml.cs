using System;
using System.Collections.Generic;
using System.Windows;
using SkyTicket.Client.Models;
using SkyTicket.Client.Services;

namespace SkyTicket.Client
{
    /// <summary>
    /// Interaction logic for AirportsWindow.xaml
    /// </summary>
    public partial class AirportsWindow : Window
    {
        private ApiService _apiService;

        public AirportsWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadAirports();
        }

        public async void LoadAirports()
        {
            LabelStatus.Text = "A carregar aeroportos...";

            var response = await _apiService.GetAirports("https://localhost:44332/", "api/airports");

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            DataGridAirports.ItemsSource = (List<Airport>)response.Result;
            LabelStatus.Text = "Aeroportos carregados.";
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var editor = new AirportEditorWindow();
            LoadAirports();
            editor.Show();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Airport)DataGridAirports.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um aeroporto na tabela primeiro.");
                return;
            }

            var editor = new AirportEditorWindow(selected);
            LoadAirports();
            editor.Show();
        }

        

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Airport)DataGridAirports.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um aeroporto na tabela primeiro.");
                return;
            }

            var confirmacao = MessageBox.Show(
                $"Tens a certeza que queres apagar o aeroporto \"{selected.Name}\"?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            LabelStatus.Text = "A apagar aeroporto...";

            var response = await _apiService.DeleteAirport("https://localhost:44332/", "api/airports", selected.Id);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            LabelStatus.Text = "Aeroporto apagado.";
            LoadAirports();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
