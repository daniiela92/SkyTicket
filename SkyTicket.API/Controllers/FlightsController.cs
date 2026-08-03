using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;

namespace SkyTicket.API.Controllers
{
    public class FlightsController : ApiController
    {

        DataClasses1DataContext dc = new DataClasses1DataContext("workstation id = SkyTicketDatabase.mssql.somee.com; packet size = 4096; user id = daniielapaiis92_SQLLogin_1; pwd=mh1cyqg5pj;data source = SkyTicketDatabase.mssql.somee.com; persist security info=False;initial catalog = SkyTicketDatabase; TrustServerCertificate=True");


        // GET api/Flights
        // GET api/Flights
        /// <summary>
        /// Returns a list of all flights
        /// </summary>
        /// <returns>List of flights</returns>
        public List<Flight> Get()
        {
           var flightsList = from Flight in dc.Flights select Flight;

            return flightsList.ToList();    
        }

        // GET api/Flights/5
        /// <summary>
        /// Returns a specific flight by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Flight</returns>
        public IHttpActionResult Get(int id)
        {

            var flight = dc.Flights.SingleOrDefault(x => x.Id == id);

            if(flight != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, flight));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
        }

        //GET api/Flights/FlightNumber/{flightNumber}
        /// <summary>
        /// Finds flights by their flight number
        /// </summary>
        /// <param name="flightNumber"></param>
        /// <returns>List of flights</returns>
        [Route("api/Flights/FlightNumber/{flightNumber}")]
        public List<Flight> GetByFlightNumber(string flightNumber)
        {
            var flights = dc.Flights.Where(x => x.FlightNumber == flightNumber).ToList();

            return flights;

        }

        //GET api/flights/{id}/seats
        /// <summary>
        /// Returns the seats of a flight
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of seats</returns>
        [Route("api/Flights/{id}/Seats")]
        public List<Seat> GetSeats(int id)
        {
            var seats = dc.Seats.Where(s => s.FlightId == id).ToList();

            return seats;
        }


        // POST api/Flights
        /// <summary>
        /// Creates a new flight and generates its seats automatically
        /// </summary>
        /// <param name="newFlight"></param>
        /// <returns>201 Created</returns>
        public IHttpActionResult Post(Flight newFlight)
        {
            Airplane airplane = dc.Airplanes.SingleOrDefault(x => x.Id == newFlight.AirplaneId);

            if (airplane == null)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest));

            }

            if(newFlight.DepartureTime < DateTime.Now)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest));
            }

            if (!airplane.IsActive)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
            }

            dc.Flights.InsertOnSubmit(newFlight);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));

            }

            GenerateSeats(newFlight, airplane);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));

            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created));
        }



        // PUT api/Flights/5
        /// <summary>
        /// Updates a flight; regenerates seats if the airplane changes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newFlight"></param>
        /// <returns>200 OK, or 409 if changing airplane on a flight with tickets</returns>
        public IHttpActionResult Put(int id, Flight newFlight)
        {
            Flight flight = dc.Flights.FirstOrDefault(f => f.Id == id);

            if (flight == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            int oldAirplaneId = flight.AirplaneId;

            if(oldAirplaneId != newFlight.AirplaneId)
            {
                if(dc.Tickets.Any(t => t.FlightId == id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
                }

                var oldSeats = dc.Seats.Where(s => s.FlightId == id).ToList();
                dc.Seats.DeleteAllOnSubmit(oldSeats);
                dc.SubmitChanges();

                Airplane newAirplane = dc.Airplanes.SingleOrDefault(a => a.Id == newFlight.AirplaneId);

                if (newAirplane == null || !newAirplane.IsActive)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
                }

                GenerateSeats(flight, newAirplane);
            }

            flight.FlightNumber = newFlight.FlightNumber;
            flight.DepartureTime = newFlight.DepartureTime;
            flight.ArrivalTime = newFlight.ArrivalTime;
            flight.DepartureAirportId = newFlight.DepartureAirportId;
            flight.ArrivalAirportId = newFlight.ArrivalAirportId;
            flight.AirplaneId = newFlight.AirplaneId;
            flight.BasePrice = newFlight.BasePrice;

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

        // DELETE api/Flights/5
        /// <summary>
        /// Deletes a flight and its seats and tickets
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 OK, or 409 if the flight has already departed</returns>
        public IHttpActionResult Delete(int id)
        {
            Flight flight = dc.Flights.FirstOrDefault(f => f.Id == id);

            if(flight == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            if(flight.DepartureTime < DateTime.Now)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
            }

            var tickets = dc.Tickets.Where(t => t.FlightId == id);

            dc.Tickets.DeleteAllOnSubmit(tickets);

            var seats = dc.Seats.Where(s => s.FlightId == id);

            dc.Seats.DeleteAllOnSubmit(seats);

            dc.Flights.DeleteOnSubmit(flight);


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
        /// Generates the seats for a flight based on the airplane's capacity
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="airplane"></param>
        private void GenerateSeats(Flight flight, Airplane airplane)
        {

            List<Seat> seats = new List<Seat>();


            for (int i = 1; i <= airplane.BusinessSeats; i++)
            {
                Seat seat = new Seat
                {
                    FlightId = flight.Id,
                    Code = $"BUS{i}",
                    Class = 0,
                    IsAvailable = true
                };

                dc.Seats.InsertOnSubmit(seat);
                
            }

            for (int i = 1; i <= airplane.EconomySeats; i++)
            {
                Seat seat = new Seat
                {
                    FlightId= flight.Id,
                    Code = $"ECO{i}",
                    Class = 1,
                    IsAvailable = true
                };

                dc.Seats.InsertOnSubmit(seat);
                
            }

        }
    }
}