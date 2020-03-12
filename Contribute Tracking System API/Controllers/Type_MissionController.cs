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
        public IEnumerable<object> Get()
        {
            return db.TYPE_MISSIONs.Where(x=>x.status == true).Select(x=> new {
                x.id_type, x.name_type_mission, x.status, x.date, x.id_employee
            });
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


        // PUT:Type_Mission/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: Type_Mission/5
        public void Delete(int id)
        {
        }
    }
}
