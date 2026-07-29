using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SkyTicket.Client.Models;
using SkyTicket.Client.Services;

namespace SkyTicket.Client
{
    public partial class TicketEditorWindow : Window
    {
        private ApiService _apiService;

        public TicketEditorWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadPassengersAndFlights();
        }

        private async void LoadPassengersAndFlights()
        {
            var passengersResponse = await _apiService.GetPassengers("https://localhost:44332/", "api/passengers");
            var flightsResponse = await _apiService.GetFlights("https://localhost:44332/", "api/flights");

            if (!passengersResponse.IsSucess || !flightsResponse.IsSucess)
            {
                LabelStatus.Text = "Não foi possível carregar passageiros/voos.";
                return;
            }

            CmbPassenger.ItemsSource = (List<Passenger>)passengersResponse.Result;
            CmbFlight.ItemsSource = (List<Flight>)flightsResponse.Result;
        }

        private async void CmbFlight_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CmbSeat.ItemsSource = null;

            if (CmbFlight.SelectedValue == null)
            {
                return;
            }

            int flightId = (int)CmbFlight.SelectedValue;

            var response = await _apiService.GetSeatsByFlight("https://localhost:44332/", "api/flights", flightId);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro ao carregar lugares: " + response.Message;
                return;
            }

            var seats = (List<Seat>)response.Result;

            var availableSeats = seats.Where(s => s.IsAvailable).ToList();

            CmbSeat.ItemsSource = availableSeats;

            if (availableSeats.Count == 0)
            {
                LabelStatus.Text = "Este voo já não tem lugares disponíveis.";
            }
            else
            {
                LabelStatus.Text = "";
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbPassenger.SelectedValue == null)
            {
                LabelStatus.Text = "Escolhe o passageiro.";
                return;
            }

            if (CmbFlight.SelectedValue == null)
            {
                LabelStatus.Text = "Escolhe o voo.";
                return;
            }

            if (CmbSeat.SelectedValue == null)
            {
                LabelStatus.Text = "Escolhe o lugar.";
                return;
            }

            var ticket = new Ticket
            {
                PassengerId = (int)CmbPassenger.SelectedValue,
                FlightId = (int)CmbFlight.SelectedValue,
                SeatId = (int)CmbSeat.SelectedValue
            };

            var response = await _apiService.PostTicket("https://localhost:44332/", "api/tickets", ticket);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            this.Close();
        }
    }
}