using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SkyTicket.API.Controllers
{
    public class AirplanesController : ApiController
    {
        DataClasses1DataContext dc = new DataClasses1DataContext("workstation id = SkyTicketDatabase.mssql.somee.com; packet size = 4096; user id = daniielapaiis92_SQLLogin_1; pwd=mh1cyqg5pj;data source = SkyTicketDatabase.mssql.somee.com; persist security info=False;initial catalog = SkyTicketDatabase; TrustServerCertificate=True");

        // GET api/Airplanes
        /// <summary>
        /// Returns a list of all airplanes 
        /// </summary>
        /// <returns>List of airplanes</returns>
        public List<Airplane> Get()
        {
            var airplanesList = from Airplane in dc.Airplanes select Airplane;

            return airplanesList.ToList();
        }

        // GET api/Airplanes/5
        /// <summary>
        /// Returns a specific airplane by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Airplane</returns>
        public IHttpActionResult Get(int id)
        {
            var airplane = dc.Airplanes.SingleOrDefault(x => x.Id == id);

            if (airplane != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, airplane));

            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
        }

        // POST api/Airplanes
        /// <summary>
        /// Creates a new airplane
        /// </summary>
        /// <param name="newAirplane"></param>
        /// <returns>201 Created</returns>
        public IHttpActionResult Post([FromBody] Airplane newAirplane)
        {
            dc.Airplanes.InsertOnSubmit(newAirplane);

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

        // PUT api/Airplanes/5
        /// <summary>
        /// Update an existing airplane
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newAirplane"></param>
        /// <returns>200 OK</returns>
        public IHttpActionResult Put(int id, [FromBody] Airplane newAirplane)
        {

            Airplane airplane = dc.Airplanes.FirstOrDefault(a => a.Id == id);

            if(airplane == null) 
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));

            }

            airplane.Brand = newAirplane.Brand;
            airplane.Model = newAirplane.Model;
            airplane.EconomySeats = newAirplane.EconomySeats;
            airplane.BusinessSeats = newAirplane.BusinessSeats;
            airplane.IsActive = newAirplane.IsActive;

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

        // DELETE api/Airplanes/5
        /// <summary>
        /// Deletes an existing airplane
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 OK</returns>
        public IHttpActionResult Delete(int id)
        {

            Airplane airplane = dc.Airplanes.FirstOrDefault(a => a.Id == id);

            if (airplane != null)
            {

                if(dc.Flights.Any(f => f.AirplaneId == id))
                {

                    airplane.IsActive = false;

                }
                else 
                {
                    dc.Airplanes.DeleteOnSubmit(airplane);
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

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
        }

        // TODO - testar apagar avião que está em voo, deve desativar o avião e não apagar
    }
}