using SkyTicket.Client.Models;
using SkyTicket.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SkyTicket.Client
{
    /// <summary>
    /// Interaction logic for PassengersWindow.xaml
    /// </summary>
    public partial class PassengersWindow : Window
    {
        private ApiService _apiService;

        public PassengersWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadPassengers();
        }

        public async void LoadPassengers()
        {
            LabelStatus.Text = "A carregar passageiros...";

            var response = await _apiService.GetPassengers("http://www.skyticketproject.somee.com/", "api/passengers");

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            DataGridPassengers.ItemsSource = (List<Passenger>)response.Result;
            LabelStatus.Text = "Passageiros carregados.";
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var editor = new PassengerEditorWindow();
            editor.Closed += (s, args) => LoadPassengers();
            editor.Show();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Passenger)DataGridPassengers.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um passageiro na tabela primeiro.");
                return;
            }

            var editor = new PassengerEditorWindow(selected);
            editor.Closed += (s, args) => LoadPassengers();
            editor.Show();
        }

        

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Passenger)DataGridPassengers.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um passageiro na tabela primeiro.");
                return;
            }

            var confirmacao = MessageBox.Show(
                $"Tens a certeza que queres apagar o passageiro \"{selected.FirstName} {selected.LastName}\"?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            LabelStatus.Text = "A apagar passageiro...";

            var response = await _apiService.DeletePassenger("http://www.skyticketproject.somee.com/", "api/passengers", selected.Id);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            LabelStatus.Text = "Passageiro apagado.";
            LoadPassengers();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}

