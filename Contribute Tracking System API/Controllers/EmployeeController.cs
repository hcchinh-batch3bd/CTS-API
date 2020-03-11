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
        [Route("Employee")]
        [HttpGet]
        // GET: api/Employee
        public IEnumerable<EMPLOYEE> Get()
        {
            return db.EMPLOYEEs.ToList<EMPLOYEE>();
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
        public IHttpActionResult Put(int id, [FromUri] string apiKey)
        {
            var quyen = db.EMPLOYEEs.Where(x => x.apiKey.Equals(apiKey) && x.level_employee == true).Count();
            if(quyen > 0)
            {
                var update = db.EMPLOYEEs.Where(x => x.id_employee == id).ToList();
                update.ForEach(x => { x.status = false; });
                db.SubmitChanges();
                return Ok(new { results = id, message = "Cập nhật thành công!" });
            }
            else
            {
                return Ok(new { results = id, message = "Cập nhật không thành công!" });
            }
            
        }

        // DELETE: api/Employee/5
        public void Delete(int id)
        {
            
        }
    }
}
