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
            MessageBox.Show("Airports - not implemented yet");
        }

        private void BtnAirplanes_Click(object sender, RoutedEventArgs e)
        {
            new AirplanesWindow().Show();
        }

        private void BtnPassengers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Passengers - not implemented yet");
        }

        private void BtnFlights_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Flights - not implemented yet");
        }

        private void BtnTickets_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tickets - not implemented yet");
        }
    
    }
}
