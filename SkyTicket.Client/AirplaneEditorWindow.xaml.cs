using System;
using System.Windows;
using SkyTicket.Client.Models;
using SkyTicket.Client.Services;

namespace SkyTicket.Client
{

    public partial class AirplaneEditorWindow : Window
    {

        private Airplane _airplaneToEdit;
        private ApiService _apiService;

        public AirplaneEditorWindow(Airplane airplane = null)
        {
            InitializeComponent();

            _apiService = new ApiService();
            _airplaneToEdit = airplane;

            if (_airplaneToEdit == null)
            {
                Title = "Criar Avião";
                LabelTitle.Text = "Criar Avião";
            }
            else
            {
                Title = "Editar Avião";
                LabelTitle.Text = "Editar Avião";

                TxtBrand.Text = _airplaneToEdit.Brand;
                TxtModel.Text = _airplaneToEdit.Model;
                TxtEconomySeats.Text = _airplaneToEdit.EconomySeats.ToString();
                TxtBusinessSeats.Text = _airplaneToEdit.BusinessSeats.ToString();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBrand.Text) || string.IsNullOrWhiteSpace(TxtModel.Text))
            {
                LabelStatus.Text = "Marca e Modelo são obrigatórios.";
                return;
            }

            if (!int.TryParse(TxtEconomySeats.Text, out int economySeats) ||
                !int.TryParse(TxtBusinessSeats.Text, out int businessSeats))
            {
                LabelStatus.Text = "Lugares Económicos e Executivos têm de ser números.";
                return;
            }

            var airplane = new Airplane
            {
                Brand = TxtBrand.Text,
                Model = TxtModel.Text,
                EconomySeats = economySeats,
                BusinessSeats = businessSeats
            };

            Response response;

            if (_airplaneToEdit == null)
            {
                response = await _apiService.PostAirplane("https://localhost:44332/", "api/airplanes", airplane);
            }
            else
            {
                airplane.Id = _airplaneToEdit.Id;
                response = await _apiService.PutAirplane("https://localhost:44332/", "api/airplanes", _airplaneToEdit.Id, airplane);
            }

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            this.Close();
        }
    }
}
