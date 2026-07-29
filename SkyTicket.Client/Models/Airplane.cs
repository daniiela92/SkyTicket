using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTicket.Client.Models
{
    public  class Airplane
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return $"{Brand} {Model}";
        }

    }
}
