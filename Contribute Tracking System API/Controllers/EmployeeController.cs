using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contribute_Tracking_System_API.Controllers
{
    public class EmployeeController : ApiController
    {
        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        // GET: api/Employee
        public IHttpActionResult Get()
        {
            string message = null;
            bool status = true;
            return Ok(new { results = db.EMPLOYEEs, status = status, message = message });
        }

        // GET: api/Employee/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Employee
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Employee/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Employee/5
        public void Delete(int id)
        {
        }
    }
}
