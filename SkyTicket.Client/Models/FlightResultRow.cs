using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTicket.Client.Models
{
    public class FlightResultRow
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public string Duration { get; set; }
        public string Fare { get; set; }
    }
}
