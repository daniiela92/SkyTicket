using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SkyTicket.API.Controllers
{
    public class AirportsController : ApiController
    {
        DataClasses1DataContext dc = new DataClasses1DataContext("workstation id = SkyTicketDatabase.mssql.somee.com; packet size = 4096; user id = daniielapaiis92_SQLLogin_1; pwd=mh1cyqg5pj;data source = SkyTicketDatabase.mssql.somee.com; persist security info=False;initial catalog = SkyTicketDatabase; TrustServerCertificate=True");


        // GET api/Airports
        /// <summary>
        /// Returns a list of all airports
        /// </summary>
        /// <returns>List of airports</returns>
        public List<Airport> Get()
        {

            var airportsList = from Airport in dc.Airports select Airport;

            return airportsList.ToList();


        }

        // GET api/Airports/5
        /// <summary>
        /// Returns a specific airport by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Airport</returns>
        public IHttpActionResult Get(int id)
        {
            var airport = dc.Airports.SingleOrDefault(x => x.Id == id);

            if (airport != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, airport));

            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));

        }

        // GET api/Airports/IATACode/{iataCode}
        /// <summary>
        /// Finds an airport by its IATA code
        /// </summary>
        /// <param name="iataCode"></param>
        /// <returns>Airport</returns>
        [Route("api/Airports/IATACode/{iataCode}")]
        public IHttpActionResult GetByIATACode(string iataCode)
        {
            var airport = dc.Airports.SingleOrDefault(x => x.IataCode == iataCode);

            if (airport != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, airport));
            }


            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));

        }


        // POST api/Airports
        /// <summary>
        /// Creates a new airport
        /// </summary>
        /// <param name="newAirport"></param>
        /// <returns>201 Created</returns>
        public IHttpActionResult Post([FromBody] Airport newAirport)
        {
            Airport airport = dc.Airports.FirstOrDefault(a => a.IataCode == newAirport.IataCode);

            if (airport != null)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));

            }

            dc.Airports.InsertOnSubmit(newAirport);

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

        // PUT api/Airports/5
        /// <summary>
        /// Updates an existing airport
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newAirport"></param>
        /// <returns>200 OK</returns>
        public IHttpActionResult Put(int id, [FromBody] Airport newAirport)
        {

            Airport airport = dc.Airports.FirstOrDefault(a => a.Id == id);



            if (airport == null)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));

            }

            airport.Name = newAirport.Name;
            airport.City = newAirport.City;
            airport.Country = newAirport.Country;
            airport.IataCode = newAirport.IataCode;
            

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

        // DELETE api/Airports/5
        /// <summary>
        /// Deletes an airport
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 OK</returns>
        public IHttpActionResult Delete(int id)
        {
            Airport airport = dc.Airports.FirstOrDefault(a => a.Id == id);

            if (airport != null)
            {

                if (dc.Flights.Any(f => f.DepartureAirportId == id || f.ArrivalAirportId == id))
                {

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
                }


                dc.Airports.DeleteOnSubmit(airport);

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

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
        }
    }
}