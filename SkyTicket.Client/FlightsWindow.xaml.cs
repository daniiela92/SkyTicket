using SkyTicket.Client.Models;
using SkyTicket.Client.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SkyTicket.Client
{
    /// <summary>
    /// Interaction logic for FlightsWindow.xaml
    /// </summary>
    public partial class FlightsWindow : Window
    {
        private ApiService _apiService;

        public FlightsWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadFlights();
        }

        public async void LoadFlights()
        {
            LabelStatus.Text = "A carregar voos...";

            var response = await _apiService.GetFlights("http://www.skyticketproject.somee.com/", "api/flights");

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            DataGridFlights.ItemsSource = (List<Flight>)response.Result;
            LabelStatus.Text = "Voos carregados.";
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var editor = new FlightEditorWindow();
            editor.Closed += Editor_Closed;
            editor.Show();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Flight)DataGridFlights.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um voo na tabela primeiro.");
                return;
            }

            var editor = new FlightEditorWindow(selected);
            editor.Closed += Editor_Closed;
            editor.Show();
        }

        private void Editor_Closed(object sender, EventArgs e)
        {
            LoadFlights();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Flight)DataGridFlights.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um voo na tabela primeiro.");
                return;
            }

            var confirmacao = MessageBox.Show(
                $"Tens a certeza que queres apagar o voo \"{selected.FlightNumber}\"?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            LabelStatus.Text = "A apagar voo...";

            var response = await _apiService.DeleteFlight("http://www.skyticketproject.somee.com/", "api/flights", selected.Id);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            LabelStatus.Text = "Voo apagado.";
            LoadFlights();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            new FlightSearchWindow().Show();
        }
    }
}

