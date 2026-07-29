using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTicket.Client.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public int AirplaneId { get; set; }
        public decimal BasePrice { get; set; }

        public override string ToString()
        {
            return $"{FlightNumber} ({DepartureTime:dd/MM/yyyy HH:mm})";
        }

    }
}
