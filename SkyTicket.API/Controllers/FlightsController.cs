using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SkyTicket.API.Controllers
{
    public class FlightsController : ApiController
    {

        DataClasses1DataContext dc = new DataClasses1DataContext("workstation id = SkyTicketDatabase.mssql.somee.com; packet size = 4096; user id = daniielapaiis92_SQLLogin_1; pwd=mh1cyqg5pj;data source = SkyTicketDatabase.mssql.somee.com; persist security info=False;initial catalog = SkyTicketDatabase; TrustServerCertificate=True");


        // GET api/Flights
        public List<Flight> Get()
        {
           var flightsList = from Flight in dc.Flights select Flight;

            return flightsList.ToList();    
        }

        // GET api/Flights/5
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
        [Route("api/Flights/FlightNumber/{flightNumber}")]
        public List<Flight> GetByFlightNumber(string flightNumber)
        {
            var flights = dc.Flights.Where(x => x.FlightNumber == flightNumber).ToList();

            return flights;

        }


        // POST api/Flights
        public IHttpActionResult Post([FromBody] Flight newFlight)
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
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/Flights/5
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