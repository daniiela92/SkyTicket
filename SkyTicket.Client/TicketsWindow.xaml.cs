using SkyTicket.Client.Models;
using SkyTicket.Client.Services;
using System.Collections.Generic;
using System.Windows;

namespace SkyTicket.Client
{
    /// <summary>
    /// Interaction logic for TicketsWindow.xaml
    /// </summary>
    public partial class TicketsWindow : Window
    {
        private ApiService _apiService;

        public TicketsWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadTickets();
        }

        public async void LoadTickets()
        {
            LabelStatus.Text = "A carregar bilhetes...";

            var response = await _apiService.GetTickets("https://localhost:44332/", "api/tickets");

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            DataGridTickets.ItemsSource = (List<Ticket>)response.Result;
            LabelStatus.Text = "Bilhetes carregados.";
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var editor = new TicketEditorWindow();
            editor.Closed += (s, args) => LoadTickets();
            editor.Show();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Ticket)DataGridTickets.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um bilhete na tabela primeiro.");
                return;
            }

            var confirmacao = MessageBox.Show(
                "Tens a certeza que queres cancelar este bilhete?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            LabelStatus.Text = "A cancelar bilhete...";

            var response = await _apiService.DeleteTicket("https://localhost:44332/", "api/tickets", selected.Id);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            LabelStatus.Text = "Bilhete cancelado.";
            LoadTickets();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

