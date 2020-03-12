using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Contribute_Tracking_System_API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class Type_MissionController : ApiController
    {
        private APIDataClassesDataContext db = new APIDataClassesDataContext();

        [Route("Type_Mission")]
        [HttpGet]
        // GET: Type_mission
        public IEnumerable<TYPE_MISSION> Get()
        {
            return db.TYPE_MISSIONs.ToList<TYPE_MISSION>();
        }

        // GET: Type_Mission/5
        public string Get(int id)
        {
            return "value";
        }

        
       // POST: Type_mission
        public void Post([FromBody]string value)
        {
        }


        // PUT: Type_Mission/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: Type_Mission/5
        public void Delete(int id)
        {
        }
    }
}
