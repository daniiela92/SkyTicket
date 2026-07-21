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
            //Lugar existe?
            Seat seat = dc.Seats.FirstOrDefault(s => s.Id == newTicket.SeatId);

            if(seat == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            } 

            //Lugar está disponível?

            if(!seat.IsAvailable)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
            }

            //Pertence a este voo?
            if(seat.FlightId != newTicket.FlightId)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest));
            }


            //Passageiro existe?
            Passenger passenger = dc.Passengers.FirstOrDefault(p => p.Id == newTicket.PassengerId);

            if(passenger == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            //Voo existe?
            Flight flight = dc.Flights.FirstOrDefault(f => f.Id == newTicket.FlightId);

            
            if(flight == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            if(flight.DepartureTime < DateTime.Now)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
            }

            newTicket.PurchaseDate = DateTime.Now;

            if(seat.Class == 0)
            {

                newTicket.Price = flight.BasePrice * 1.5m;

            }
            else
            {
                newTicket.Price = flight.BasePrice; 
            }

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
        public void Put(int id, [FromBody] string value)
        {
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
    }
}