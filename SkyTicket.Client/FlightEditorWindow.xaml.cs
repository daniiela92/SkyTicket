using SkyTicket.Client.Models;
using SkyTicket.Client.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace SkyTicket.Client
{
    /// <summary>
    /// Interaction logic for FlightEditorWindow.xaml
    /// </summary>
    public partial class FlightEditorWindow : Window
    {
        private Flight _flightToEdit;
        private ApiService _apiService;

        public FlightEditorWindow(Flight flight = null)
        {
            InitializeComponent();


            _apiService = new ApiService();
            _flightToEdit = flight;

            if (_flightToEdit == null)
            {
                Title = "Criar Voo";
                LabelTitle.Text = "Criar Voo";
            }
            else
            {
                Title = "Editar Voo";
                LabelTitle.Text = "Editar Voo";
            }

            LoadComboBoxes();
        }

        private async void LoadComboBoxes()
        {
            var airportsResponse = await _apiService.GetAirports("http://www.skyticketproject.somee.com/", "api/airports");
            var airplanesResponse = await _apiService.GetAirplanes("http://www.skyticketproject.somee.com/", "api/airplanes");

            if (!airportsResponse.IsSucess || !airplanesResponse.IsSucess)
            {
                LabelStatus.Text = "Não foi possível carregar aeroportos/aviões.";
                return;
            }

            var airports = (List<Airport>)airportsResponse.Result;
            var airplanes = (List<Airplane>)airplanesResponse.Result;

            CmbDepartureAirport.ItemsSource = airports;
            CmbArrivalAirport.ItemsSource = airports;
            CmbAirplane.ItemsSource = airplanes;


            if (_flightToEdit != null)
            {
                TxtFlightNumber.Text = _flightToEdit.FlightNumber;
                CmbDepartureAirport.SelectedValue = _flightToEdit.DepartureAirportId;
                CmbArrivalAirport.SelectedValue = _flightToEdit.ArrivalAirportId;
                CmbAirplane.SelectedValue = _flightToEdit.AirplaneId;
                TxtDepartureTime.Text = _flightToEdit.DepartureTime.ToString("dd/MM/yyyy HH:mm");
                TxtArrivalTime.Text = _flightToEdit.ArrivalTime.ToString("dd/MM/yyyy HH:mm");
                TxtBasePrice.Text = _flightToEdit.BasePrice.ToString();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFlightNumber.Text))
            {
                LabelStatus.Text = "O número do voo é obrigatório.";
                return;
            }

            if (CmbDepartureAirport.SelectedValue == null || CmbArrivalAirport.SelectedValue == null)
            {
                LabelStatus.Text = "Escolhe o aeroporto de origem e de destino.";
                return;
            }

            if ((int)CmbDepartureAirport.SelectedValue == (int)CmbArrivalAirport.SelectedValue)
            {
                LabelStatus.Text = "Origem e destino não podem ser o mesmo aeroporto.";
                return;
            }

            if (CmbAirplane.SelectedValue == null)
            {
                LabelStatus.Text = "Escolhe o avião.";
                return;
            }

            if (!DateTime.TryParseExact(TxtDepartureTime.Text, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime departureTime))
            {
                LabelStatus.Text = "Data de partida inválida. Usa o formato dd/MM/aaaa HH:mm.";
                return;
            }

            if (!DateTime.TryParseExact(TxtArrivalTime.Text, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime arrivalTime))
            {
                LabelStatus.Text = "Data de chegada inválida. Usa o formato dd/MM/aaaa HH:mm.";
                return;
            }

            if (arrivalTime <= departureTime)
            {
                LabelStatus.Text = "A chegada tem de ser depois da partida.";
                return;
            }

            if (!decimal.TryParse(TxtBasePrice.Text, out decimal basePrice) || basePrice <= 0)
            {
                LabelStatus.Text = "Indica um preço base válido (maior que zero).";
                return;
            }

            var flight = new Flight
            {
                FlightNumber = TxtFlightNumber.Text,
                DepartureAirportId = (int)CmbDepartureAirport.SelectedValue,
                ArrivalAirportId = (int)CmbArrivalAirport.SelectedValue,
                AirplaneId = (int)CmbAirplane.SelectedValue,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                BasePrice = basePrice
            };

            Response response;

            if (_flightToEdit == null)
            {
                response = await _apiService.PostFlight("http://www.skyticketproject.somee.com/", "api/flights", flight);
            }
            else
            {
                flight.Id = _flightToEdit.Id;
                response = await _apiService.PutFlight("http://www.skyticketproject.somee.com/", "api/flights", _flightToEdit.Id, flight);
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

