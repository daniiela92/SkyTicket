using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTicket.Client.Models
{
    public class TicketRow
    {
        public int Id { get; set; }
        public string PassengerName { get; set; }
        public string SeatCode { get; set; }
    }
}
