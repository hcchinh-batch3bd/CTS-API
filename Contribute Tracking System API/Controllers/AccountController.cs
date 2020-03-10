using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;


namespace Contribute_Tracking_System_API.Controllers
{
    public class AccountController : ApiController
    {

        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        [Route("Account/CheckLogin")]
        [HttpGet]
        public IHttpActionResult GetCheckLogin([FromUri] string id, [FromUri] string pw) 
        {

            string message = "Đăng nhập thành công !";
            bool status = true;
            var key = db.EMPLOYEEs.Where(x=>x.id_employee == int.Parse(id) && x.password==pw).Select(s => new  {name = s.name_employee, point= s.point, apiKey = s.apiKey});
            if (!key.Any())
                message = "Not Found ID or Password";
            return Ok(new { results = key, status = status, message = message });
        }
        // GET: api/Account
        public IEnumerable<EMPLOYEE> Get()
        {
            return db.EMPLOYEEs.ToList<EMPLOYEE>();
        }
        // GET: api/Account/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Account
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }

    }
}
