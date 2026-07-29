using System.Windows;

namespace SkyTicket.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnAirports_Click(object sender, RoutedEventArgs e)
        {
            new AirportsWindow().Show();        }

        private void BtnAirplanes_Click(object sender, RoutedEventArgs e)
        {
            new AirplanesWindow().Show();
        }

        private void BtnPassengers_Click(object sender, RoutedEventArgs e)
        {
            new PassengersWindow().Show();
        }

        private void BtnFlights_Click(object sender, RoutedEventArgs e)
        {
            new FlightsWindow().Show();
        }

        private void BtnTickets_Click(object sender, RoutedEventArgs e)
        {
            new TicketsWindow().Show();
        }
    
    }
}
