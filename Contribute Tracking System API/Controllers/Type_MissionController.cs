using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contribute_Tracking_System_API.Controllers
{
    public class Type_MissionController : ApiController
    {
        private APIDataClassesDataContext db = new APIDataClassesDataContext();

        [Route("Type_Mission")]
        [HttpGet]
        // GET: api/Type_Mission
        public IEnumerable<TYPE_MISSION> Get()
        {
            return db.TYPE_MISSIONs.ToList<TYPE_MISSION>();
        }

        // GET: api/Type_Mission/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Type_Mission
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Type_Mission/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Type_Mission/5
        public void Delete(int id)
        {
        }
    }
}
