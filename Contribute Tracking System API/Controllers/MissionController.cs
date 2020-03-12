using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
<<<<<<< HEAD
using System.Data.Entity;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
=======
>>>>>>> master
using System.Web.Http.Cors;

namespace Contribute_Tracking_System_API.Controllers
{
<<<<<<< HEAD
=======

>>>>>>> master
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class MissionController : ApiController
    {
        private APIDataClassesDataContext db = new APIDataClassesDataContext();
<<<<<<< HEAD
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
=======
        [Route("Mission/ListMission")]
        [HttpGet]
        public IHttpActionResult GetListMission()
        {

            var key = new List<object>();
            string _message = "Danh sách nhiệm vụ !!";
            bool _status = true;
            {
                var checkstatus = db.MISSIONs.Select(x => new { id = x.id_mission, status = x.status });
                foreach (var item in checkstatus)
                {
                    var checksoluong = db.MISSIONs.Where(x => x.id_mission == item.id).Select(x => x.Count).FirstOrDefault();
                    int trangthai = Int32.Parse(item.status.ToString());
                    switch (trangthai)
                    {
                        case -1:
                            key.AddRange(LoadData(item.id, checksoluong, "Hủy"));
                            break;
                        case 0:
                            key.AddRange(LoadData(item.id, checksoluong, "Đang duyệt"));
                            break;
                        case 1:
                            key.AddRange(LoadData(item.id, checksoluong, "Đang chạy"));
                            break;
                        default:
                            key.AddRange(LoadData(item.id, checksoluong, "Lỗi"));
                            break;
                    }
                }
            }
            return Ok(new { results = key, status = _status, message = _message });

        }
        //Thay đổi dữ liệu status , count khi = 0. 
        private IEnumerable<object> LoadData(int id, int checksoluong, string status)
        {
            if (checksoluong == 0)
            {
                return  from a in db.MISSIONs
                      join b in db.TYPE_MISSIONs on a.id_type equals b.id_type
                      join c in db.EMPLOYEEs on a.id_employee equals c.id_employee
                      where a.id_mission == id
                      select new
                      {
                          a.id_mission,
                          a.name_mission,
                          a.Stardate,
                          a.point,
                          a.exprie,
                          a.describe,
                          status,
                          count = "Không giới hạn",
                          b.name_type_mission,
                          c.name_employee
                      };
            }
            else
            {
               return from a in db.MISSIONs
                      where a.id_mission == id
                      join b in db.TYPE_MISSIONs on a.id_type equals b.id_type
                      join c in db.EMPLOYEEs on a.id_employee equals c.id_employee
                      select new
                      {
                          a.id_mission,
                          a.name_mission,
                          a.Stardate,
                          a.point,
                          a.exprie,
                          a.describe,
                          status,
                          count = a.Count,
                          b.name_type_mission,
                          c.name_employee
                      };
            }
        }

>>>>>>> master
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
