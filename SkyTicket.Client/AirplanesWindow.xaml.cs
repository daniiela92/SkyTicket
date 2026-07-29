using System;
using System.Collections.Generic;
using System.Windows;
using SkyTicket.Client.Models;
using SkyTicket.Client.Services;

namespace SkyTicket.Client
{

    public partial class AirplanesWindow : Window
    {
        private ApiService apiService;

        public AirplanesWindow()
        {
            InitializeComponent();
            apiService = new ApiService();
            LoadAirplanes();
        }

        private async void LoadAirplanes()
        {
            LabelStatus.Text = "A carregar aviões...";

            var response = await apiService.GetAirplanes("https://localhost:44332/", "api/airplanes");

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            DataGridAirplanes.ItemsSource = (List<Airplane>)response.Result;
            LabelStatus.Text = "Aviões carregados.";
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            new AirplaneEditorWindow().Show();
            LoadAirplanes();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

            var selected = (Airplane)DataGridAirplanes.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um avião na tabela primeiro.");
                return;
            }

            new AirplaneEditorWindow(selected).Show();

            LoadAirplanes();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var aviao = (Airplane)DataGridAirplanes.SelectedItem;

            if (aviao == null)
            {
                MessageBox.Show("Seleciona um avião na tabela primeiro.");
                return;
            }

            var confirmacao = MessageBox.Show(
                $"Tens a certeza que queres apagar o avião \"{aviao.Brand} {aviao.Model}\"?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            LabelStatus.Text = "A apagar avião...";

            var response = await apiService.DeleteAirplane("https://localhost:44332/", "api/airplanes", aviao.Id);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            LabelStatus.Text = "Avião apagado.";
            LoadAirplanes();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       
    }
}
