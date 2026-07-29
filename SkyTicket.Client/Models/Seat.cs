using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTicket.Client.Models
{
    public enum SeatClass
    {
        Business = 0,
        Economy = 1
    }

    public  class Seat
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string Code { get; set; }
        public SeatClass Class { get; set; }
        public bool IsAvailable { get; set; }

        public override string ToString()
        {
            return $"{Code} ({Class})";
        }

    }
}
