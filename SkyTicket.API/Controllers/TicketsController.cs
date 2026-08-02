using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;

namespace SkyTicket.API.Controllers
{
    public class TicketsController : ApiController
    {
        DataClasses1DataContext dc = new DataClasses1DataContext("workstation id = SkyTicketDatabase.mssql.somee.com; packet size = 4096; user id = daniielapaiis92_SQLLogin_1; pwd=mh1cyqg5pj;data source = SkyTicketDatabase.mssql.somee.com; persist security info=False;initial catalog = SkyTicketDatabase; TrustServerCertificate=True");


        // GET api/tickets
        public List<Ticket> Get()
        {
            var ticketsList = from Ticket in dc.Tickets select Ticket;

            return ticketsList.ToList();
        }

        // GET api/tickets/5
        public IHttpActionResult Get(int id)
        {
            var ticket = dc.Tickets.SingleOrDefault(x => x.Id == id);

            if (ticket != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, ticket));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
        }

        //GET api/tickets/FlightId/{flightId}
        [Route("api/tickets/FlightId/{flightId}")]
        public List<Ticket> GetByFlightId(int flightId)
        {
            var tickets = dc.Tickets.Where(t => t.FlightId == flightId).ToList();
            return tickets;
        }

        //GET api/tickets/PassengerId/{passengerId}
        [Route("api/Tickets/passenger/{passengerId}")]
        public List<Ticket> GetByPassenger(int passengerId)
        {
            var tickets = dc.Tickets.Where(t => t.PassengerId == passengerId).ToList();

            return tickets;
        } 

        // POST api/tickets
        public IHttpActionResult Post(Ticket newTicket)
        {
            Seat seat = dc.Seats.FirstOrDefault(s => s.Id == newTicket.SeatId);
            Flight flight = dc.Flights.FirstOrDefault(f => f.Id == newTicket.FlightId);

            IHttpActionResult error = ValidateTicket(newTicket, seat, flight);

            if (error != null)
            {
                return error;
            }

            newTicket.PurchaseDate = DateTime.Now;
            newTicket.Price = CalculatePrice(seat, flight);


            seat.IsAvailable = false;
            dc.Tickets.InsertOnSubmit(newTicket);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, newTicket));

        }

        // PUT api/tickets/5
        public IHttpActionResult Put(int id, Ticket newTicket)
        {
            Ticket ticket = dc.Tickets.FirstOrDefault(t => t.Id == id);

            if(ticket == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            if (ticket.Flight.DepartureTime < DateTime.Now)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
            }

            if (ticket.SeatId != newTicket.SeatId)
            {
                Seat newSeat = dc.Seats.FirstOrDefault(s => s.Id == newTicket.SeatId);

                if (newSeat == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));

                }

                if (!newSeat.IsAvailable)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));

                }
                

                if (newSeat.FlightId != ticket.FlightId)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest));

                }

                ticket.Seat.IsAvailable = true;    
                newSeat.IsAvailable = false;       
                ticket.SeatId = newTicket.SeatId;  
                ticket.Price = CalculatePrice(newSeat, ticket.Flight);


            }

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }


            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));




        }

        // DELETE api/tickets/5
        public IHttpActionResult Delete(int id)
        {
            Ticket ticket = dc.Tickets.FirstOrDefault(t  => t.Id == id);

            if(ticket == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            }


            if (ticket.Flight.DepartureTime < DateTime.Now)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
            }

            ticket.Seat.IsAvailable = true;

            dc.Tickets.DeleteOnSubmit(ticket);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }


            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));

        }

        /// <summary>
        /// Valida os dados de um bilhete antes da venda.
        /// </summary>
        /// <param name="ticket">Bilhete a validar.</param>
        /// <param name="seat">Lugar do bilhete (pode ser null se não existir).</param>
        /// <param name="flight">Voo do bilhete (pode ser null se não existir).</param>
        /// <returns>Resposta de erro, ou null se estiver tudo válido.</returns>
        private IHttpActionResult ValidateTicket(Ticket ticket, Seat seat, Flight flight)
        {
            
            if (seat == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            
            if (!seat.IsAvailable)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
            }

            
            if (seat.FlightId != ticket.FlightId)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest));
            }

            
            if (flight == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            
            if (flight.DepartureTime < DateTime.Now)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
            }

            return null;   
        }

        /// <summary>
        /// Calcula o preço do bilhete conforme a classe do lugar.
        /// </summary>
        /// <param name="seat">Lugar do bilhete.</param>
        /// <param name="flight">Voo do bilhete.</param>
        /// <returns>Preço final do bilhete.</returns>
        private decimal CalculatePrice(Seat seat, Flight flight)
        {
            if (seat.Class == 0)   
            {
                return flight.BasePrice * 1.5m;
            }

            return flight.BasePrice;
        }
    }
}