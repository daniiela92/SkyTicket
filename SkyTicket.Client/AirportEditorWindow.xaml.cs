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
    /// Interaction logic for AirportEditorWindow.xaml
    /// </summary>
    public partial class AirportEditorWindow : Window
    {
        private Airport _airportToEdit;
        private ApiService _apiService;

        public AirportEditorWindow(Airport airport = null)
        {
            InitializeComponent();

            _apiService = new ApiService();
            _airportToEdit = airport;

            if (_airportToEdit == null)
            {
                Title = "Criar Aeroporto";
                LabelTitle.Text = "Criar Aeroporto";
            }
            else
            {
                Title = "Editar Aeroporto";
                LabelTitle.Text = "Editar Aeroporto";

                TxtName.Text = _airportToEdit.Name;
                TxtCity.Text = _airportToEdit.City;
                TxtCountry.Text = _airportToEdit.Country;
                TxtIataCode.Text = _airportToEdit.IataCode;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) ||
                string.IsNullOrWhiteSpace(TxtCity.Text) ||
                string.IsNullOrWhiteSpace(TxtCountry.Text) ||
                string.IsNullOrWhiteSpace(TxtIataCode.Text))
            {
                LabelStatus.Text = "Todos os campos são obrigatórios.";
                return;
            }

            var airport = new Airport
            {
                Name = TxtName.Text,
                City = TxtCity.Text,
                Country = TxtCountry.Text,
                IataCode = TxtIataCode.Text.ToUpper()
            };

            Response response;

            if (_airportToEdit == null)
            {
                response = await _apiService.PostAirport("https://localhost:44332/", "api/airports", airport);
            }
            else
            {
                airport.Id = _airportToEdit.Id;
                response = await _apiService.PutAirport("https://localhost:44332/", "api/airports", _airportToEdit.Id, airport);
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
