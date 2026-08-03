using SkyTicket.Client.Models;
using SkyTicket.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SkyTicket.Client
{
    public partial class FlightSearchWindow : Window
    {
        private ApiService _apiService;
        private List<Flight> _allFlights;
        private List<Airport> _airports;

        
     
        public FlightSearchWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadData();
        }

        private async void LoadData()
        {
            LabelStatus.Text = "A carregar dados...";

            var airportsResponse = await _apiService.GetAirports("http://www.skyticketproject.somee.com/", "api/airports");
            var flightsResponse = await _apiService.GetFlights("http://www.skyticketproject.somee.com/", "api/flights");

            if (!airportsResponse.IsSucess || !flightsResponse.IsSucess)
            {
                LabelStatus.Text = "Não foi possível carregar aeroportos/voos.";
                return;
            }

            _airports = (List<Airport>)airportsResponse.Result;
            _allFlights = (List<Flight>)flightsResponse.Result;

            CmbOrigin.ItemsSource = _airports;
            CmbDestination.ItemsSource = _airports;

            LabelStatus.Text = "";
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (CmbOrigin.SelectedValue == null || CmbDestination.SelectedValue == null)
            {
                LabelStatus.Text = "Escolhe origem e destino.";
                return;
            }

            if ((int)CmbOrigin.SelectedValue == (int)CmbDestination.SelectedValue)
            {
                LabelStatus.Text = "Origem e destino não podem ser iguais.";
                return;
            }

            if (DpDate.SelectedDate == null)
            {
                LabelStatus.Text = "Escolhe a data da viagem.";
                return;
            }

            int originId = (int)CmbOrigin.SelectedValue;
            int destinationId = (int)CmbDestination.SelectedValue;
            DateTime day = DpDate.SelectedDate.Value.Date;

            // Filtro sobre a lista local (sem chamar a API)
            var filtered = _allFlights
                .Where(f => f.DepartureAirportId == originId
                         && f.ArrivalAirportId == destinationId
                         && f.DepartureTime.Date == day)
                .ToList();

            // Ordenação
            switch (CmbSort.SelectedIndex)
            {
                case 0: // Preço
                    filtered = filtered.OrderBy(f => f.BasePrice).ToList();
                    break;
                case 1: // Duração
                    filtered = filtered.OrderBy(f => f.ArrivalTime - f.DepartureTime).ToList();
                    break;
                default: // Hora de partida
                    filtered = filtered.OrderBy(f => f.DepartureTime).ToList();
                    break;
            }

            bool isBusiness = CmbClass.SelectedIndex == 1; // 0 = Económica, 1 = Executiva

            var rows = new List<FlightResultRow>();

            foreach (var f in filtered)
            {
                var origin = _airports.FirstOrDefault(a => a.Id == f.DepartureAirportId);
                var destination = _airports.FirstOrDefault(a => a.Id == f.ArrivalAirportId);

                TimeSpan duration = f.ArrivalTime - f.DepartureTime;
                decimal fare = isBusiness ? f.BasePrice * 1.5m : f.BasePrice;

                rows.Add(new FlightResultRow
                {
                    FlightNumber = f.FlightNumber,
                    Origin = origin != null ? origin.ToString() : "?",
                    Destination = destination != null ? destination.ToString() : "?",
                    Departure = f.DepartureTime.ToString("dd/MM/yyyy HH:mm"),
                    Arrival = f.ArrivalTime.ToString("dd/MM/yyyy HH:mm"),
                    Duration = $"{(int)duration.TotalHours}h {duration.Minutes}m",
                    Fare = $"€{fare:0.00}"
                });
            }

            DataGridResults.ItemsSource = rows;
            LabelStatus.Text = rows.Count == 0 ? "Sem voos para esta pesquisa." : $"{rows.Count} voo(s) encontrado(s).";
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}