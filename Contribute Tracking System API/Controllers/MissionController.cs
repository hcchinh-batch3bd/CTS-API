using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Web.Http.Cors;

namespace Contribute_Tracking_System_API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class MissionController : ApiController
    {
        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        [Route("Missison/Missionavaible")]
        [HttpGet]
        public IHttpActionResult Missonavaible()
        {

            string message = "";
            bool status = true;
            List<object> key =  new List<object>();
            foreach (var t in db.MISSIONs)
            {
                var id_mission = t.id_mission;
                var cou = db.MISSIONs.Where(a => a.id_mission == id_mission).Select(x => x.Count).FirstOrDefault();
                var idmiss = db.MISSION_PROCESSes.Where(b => b.id_mission == id_mission).Select(x => x.id_mission).Count();
                if (int.Parse(cou.ToString()) - idmiss > 0 || int.Parse(cou.ToString()) == 0)
                {
                    key.AddRange(db.MISSIONs.Where(x => x.id_mission == id_mission).Select(s => new { t.name_mission, t.id_mission }).ToList());
                }
            }
            return Ok(new { results = key, status = status, message = message });
        }
        [Route("Mission/Missionavaibleemp")]
        [HttpGet]
        public IHttpActionResult Missionavaibleemp([FromUri] string apiKey)
        {
            var miss = new object();
            string _message = "";
            bool _status = true;
            if (apiKey != null)
            {
                _message = "!";
                _status = true;
                var id = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Select(x => x.id_employee).SingleOrDefault();
                foreach (var t in db.MISSION_PROCESSes)
                {
                    if (t.id_employee == id)
                    {
                        miss = db.MISSIONs.Join(db.MISSION_PROCESSes.Where(x => x.status == 0 && x.id_employee==id), m => m.id_mission, mp => mp.id_mission, (m, mp) =>
                              new {m.id_mission , m.name_mission, m.Stardate, m.point, m.exprie, m.describe, m.id_type,m.id_employee});
                    }
                }
            }
            else _message = "Bạn chưa nhập apiKey";
            return Ok(new { results = miss, status = _status, message = _message });
        }
        // GET: api/Mission
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Mission/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Mission
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Mission/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Mission/5
        public void Delete(int id)
        {
        }
    }
}
