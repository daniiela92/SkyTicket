using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SkyTicket.Client.Models;
using SkyTicket.Client.Services;

namespace SkyTicket.Client
{
    public partial class TicketsWindow : Window
    {
        private ApiService apiService;
        private List<Seat> currentSeats;
        private int? selectedSeatId = null;
        private Button selectedSeatButton = null;

        // Classe simples só para mostrar a tabela com nomes legíveis
        private class TicketRow
        {
            public int Id { get; set; }
            public string PassengerName { get; set; }
            public string SeatCode { get; set; }
        }

        public TicketsWindow()
        {
            InitializeComponent();
            apiService = new ApiService();
            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            var passengersResponse = await apiService.GetPassengers("https://localhost:44332/", "api/passengers");
            var flightsResponse = await apiService.GetFlights("https://localhost:44332/", "api/flights");

            if (!passengersResponse.IsSucess || !flightsResponse.IsSucess)
            {
                LabelStatus.Text = "Não foi possível carregar passageiros/voos.";
                return;
            }

            CmbPassenger.ItemsSource = (List<Passenger>)passengersResponse.Result;
            CmbFlight.ItemsSource = (List<Flight>)flightsResponse.Result;
        }

        private async void CmbFlight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSeatId = null;
            selectedSeatButton = null;
            BtnBuy.IsEnabled = false;
            LabelSelectedSeat.Text = "Nenhum lugar escolhido";

            PanelBusinessSeats.Children.Clear();
            PanelEconomySeats.Children.Clear();
            DataGridFlightTickets.ItemsSource = null;

            if (CmbFlight.SelectedValue == null)
            {
                return;
            }

            int flightId = (int)CmbFlight.SelectedValue;

            await LoadSeats(flightId);
            await LoadTicketsForFlight(flightId);
        }

        private async System.Threading.Tasks.Task LoadSeats(int flightId)
        {
            LabelStatus.Text = "A carregar lugares...";

            var response = await apiService.GetSeatsByFlight("https://localhost:44332/", "api/flights", flightId);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro ao carregar lugares: " + response.Message;
                return;
            }

            currentSeats = (List<Seat>)response.Result;

            var businessSeats = currentSeats.Where(s => s.Class == SeatClass.Business).ToList();
            var economySeats = currentSeats.Where(s => s.Class == SeatClass.Economy).ToList();

            foreach (var seat in businessSeats)
            {
                PanelBusinessSeats.Children.Add(CreateSeatButton(seat));
            }

            foreach (var seat in economySeats)
            {
                PanelEconomySeats.Children.Add(CreateSeatButton(seat));
            }

            LabelStatus.Text = "";
        }

        private Button CreateSeatButton(Seat seat)
        {
            var btn = new Button
            {
                Content = seat.Code,
                Width = 60,
                Height = 36,
                Margin = new Thickness(4),
                Tag = seat,
                IsEnabled = seat.IsAvailable
            };

            if (seat.IsAvailable)
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(0x97, 0xC4, 0x59));
                btn.Foreground = new SolidColorBrush(Color.FromRgb(0x17, 0x34, 0x04));
                btn.Click += SeatButton_Click;
            }
            else
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(0xF0, 0x99, 0x95));
                btn.Foreground = new SolidColorBrush(Color.FromRgb(0x50, 0x13, 0x13));
            }

            return btn;
        }

        private void SeatButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = (Button)sender;
            var seat = (Seat)clickedButton.Tag;

            // Repõe a cor verde no lugar que estava selecionado antes (se houver)
            if (selectedSeatButton != null)
            {
                selectedSeatButton.Background = new SolidColorBrush(Color.FromRgb(0x97, 0xC4, 0x59));
                selectedSeatButton.Foreground = new SolidColorBrush(Color.FromRgb(0x17, 0x34, 0x04));
            }

            // Marca o novo lugar escolhido
            clickedButton.Background = new SolidColorBrush(Color.FromRgb(0x37, 0x8A, 0xDD));
            clickedButton.Foreground = Brushes.White;

            selectedSeatButton = clickedButton;
            selectedSeatId = seat.Id;
            LabelSelectedSeat.Text = "Lugar escolhido: " + seat.Code;
            BtnBuy.IsEnabled = true;
        }

        private async System.Threading.Tasks.Task LoadTicketsForFlight(int flightId)
        {
            var response = await apiService.GetTicketsByFlight("https://localhost:44332/", "api/tickets", flightId);

            if (!response.IsSucess)
            {
                DataGridFlightTickets.ItemsSource = null;
                return;
            }

            var tickets = (List<Ticket>)response.Result;
            var passengers = (List<Passenger>)CmbPassenger.ItemsSource;

            var rows = new List<TicketRow>();

            foreach (var ticket in tickets)
            {
                var passenger = passengers.FirstOrDefault(p => p.Id == ticket.PassengerId);
                var seat = currentSeats.FirstOrDefault(s => s.Id == ticket.SeatId);

                rows.Add(new TicketRow
                {
                    Id = ticket.Id,
                    PassengerName = passenger != null ? passenger.ToString() : "?",
                    SeatCode = seat != null ? seat.Code : "?"
                });
            }

            DataGridFlightTickets.ItemsSource = rows;
        }

        private async void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (CmbPassenger.SelectedValue == null)
            {
                LabelStatus.Text = "Escolhe o passageiro.";
                return;
            }

            if (selectedSeatId == null)
            {
                LabelStatus.Text = "Escolhe um lugar no mapa.";
                return;
            }

            BtnBuy.IsEnabled = false;

            var ticket = new Ticket
            {
                PassengerId = (int)CmbPassenger.SelectedValue,
                FlightId = (int)CmbFlight.SelectedValue,
                SeatId = selectedSeatId.Value
            };

            var response = await apiService.PostTicket("https://localhost:44332/", "api/tickets", ticket);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                BtnBuy.IsEnabled = true;
                return;
            }

            LabelStatus.Text = "Bilhete comprado com sucesso.";

            selectedSeatId = null;
            selectedSeatButton = null;
            LabelSelectedSeat.Text = "Nenhum lugar escolhido";

            int flightId = (int)CmbFlight.SelectedValue;
            PanelBusinessSeats.Children.Clear();
            PanelEconomySeats.Children.Clear();

            await LoadSeats(flightId);
            await LoadTicketsForFlight(flightId);
        }

        private async void BtnCancelTicket_Click(object sender, RoutedEventArgs e)
        {
            var selected = (TicketRow)DataGridFlightTickets.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("Seleciona um bilhete na tabela primeiro.");
                return;
            }

            var confirmacao = MessageBox.Show(
                $"Tens a certeza que queres cancelar o bilhete de \"{selected.PassengerName}\" (lugar {selected.SeatCode})?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            LabelStatus.Text = "A cancelar bilhete...";

            var response = await apiService.DeleteTicket("https://localhost:44332/", "api/tickets", selected.Id);

            if (!response.IsSucess)
            {
                LabelStatus.Text = "Erro: " + response.Message;
                return;
            }

            LabelStatus.Text = "Bilhete cancelado.";

            int flightId = (int)CmbFlight.SelectedValue;
            PanelBusinessSeats.Children.Clear();
            PanelEconomySeats.Children.Clear();

            await LoadSeats(flightId);
            await LoadTicketsForFlight(flightId);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
