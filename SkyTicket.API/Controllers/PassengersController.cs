using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SkyTicket.API.Controllers
{
    public class PassengersController : ApiController
    {
        DataClasses1DataContext dc = new DataClasses1DataContext("workstation id = SkyTicketDatabase.mssql.somee.com; packet size = 4096; user id = daniielapaiis92_SQLLogin_1; pwd=mh1cyqg5pj;data source = SkyTicketDatabase.mssql.somee.com; persist security info=False;initial catalog = SkyTicketDatabase; TrustServerCertificate=True");

        // GET api/Passengers
        public List<Passenger> Get()
        {
            var passengersList = from Passenger in dc.Passengers select Passenger;

            return passengersList.ToList();
        }

        // GET api/Passengers/5
        public IHttpActionResult Get(int id)
        {
            var passengersList = dc.Passengers.SingleOrDefault(x => x.Id == id);

            if (passengersList != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, passengersList));

            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
        }

        // GET api/Passengers/passport
        [Route("api/Passengers/{passport}")]
        public IHttpActionResult Get(string passport)
        {
            var passengersList = dc.Passengers.SingleOrDefault(x => x.Passport == passport);

            if (passengersList != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, passengersList));
            }


            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));


        }

        // POST api/Passengers
        public IHttpActionResult Post([FromBody] Passenger newPassenger)
        {
            Passenger passenger = dc.Passengers.FirstOrDefault(a => a.Id == newPassenger.Id);

            if (passenger != null)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));

            }

            dc.Passengers.InsertOnSubmit(newPassenger);

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

        // PUT api/Passengers/5
        public IHttpActionResult Put(int id, [FromBody] Passenger newPassenger)
        {

            Passenger passenger = dc.Passengers.FirstOrDefault(a => a.Id == id);

            if (passenger == null)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));

            }

            passenger.FirstName = newPassenger.FirstName;
            passenger.LastName = newPassenger.LastName;
            passenger.Phone = newPassenger.Phone;
            passenger.Email = newPassenger.Email;
            passenger.Passport = newPassenger.Passport;

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

        // DELETE api/Passengers/5
        [Route("api/Passengers/{passport}")]
        public IHttpActionResult Delete(string passport)
        {

            Passenger passenger = dc.Passengers.FirstOrDefault(a => a.Passport == passport);

            if (passenger != null)
            {
                dc.Passengers.DeleteOnSubmit(passenger);

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