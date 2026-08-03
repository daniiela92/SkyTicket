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
        /// <summary>
        /// Returns a list of all tickets
        /// </summary>
        /// <returns>List of tickets</returns>
        public List<Ticket> Get()
        {
            var ticketsList = from Ticket in dc.Tickets select Ticket;

            return ticketsList.ToList();
        }

        // GET api/tickets/5
        /// <summary>
        /// Returns a specific ticket by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Ticket</returns>
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
        /// <summary>
        /// Returns the tickets of a flight
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns>List of tickets</returns>
        [Route("api/tickets/FlightId/{flightId}")]
        public List<Ticket> GetByFlightId(int flightId)
        {
            var tickets = dc.Tickets.Where(t => t.FlightId == flightId).ToList();
            return tickets;
        }

        //GET api/tickets/PassengerId/{passengerId}
        /// <summary>
        /// Returns the tickets of a passenger
        /// </summary>
        /// <param name="passengerId"></param>
        /// <returns>List of tickets</returns>
        [Route("api/Tickets/passenger/{passengerId}")]
        public List<Ticket> GetByPassenger(int passengerId)
        {
            var tickets = dc.Tickets.Where(t => t.PassengerId == passengerId).ToList();

            return tickets;
        }

        // POST api/tickets
        /// <summary>
        /// Buys a ticket for a passenger on a specific seat, marking it as unavailable
        /// </summary>
        /// <param name="newTicket"></param>
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
        /// <summary>
        /// Changes a ticket's seat, freeing the old one and taking the new one
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newTicket"></param>
        /// <returns>200 OK; 409 if the seat is taken; 400 if it belongs to another flight</returns>
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

                Seat oldSeat = dc.Seats.FirstOrDefault(s => s.Id == ticket.SeatId);

                ticket.SeatId = newSeat.Id;
                ticket.Price = CalculatePrice(newSeat, ticket.Flight);

                oldSeat.IsAvailable = true;
                newSeat.IsAvailable = false;


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
        /// <summary>
        /// Cancels a ticket and frees its seat
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 OK, or 409 if the flight has already departed</returns>
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
        /// Validates a ticket's data before the sale.
        /// </summary>
        /// <param name="ticket">Ticket to validate.</param>
        /// <param name="seat">Ticket's seat (may be null if it does not exist).</param>
        /// <param name="flight">Ticket's flight (may be null if it does not exist).</param>
        /// <returns>An error response, or null if everything is valid.</returns>
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
        /// Calculates the ticket price according to the seat class.
        /// </summary>
        /// <param name="seat">Ticket's seat.</param>
        /// <param name="flight">Ticket's flight.</param>
        /// <returns>Final ticket price.</returns>
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