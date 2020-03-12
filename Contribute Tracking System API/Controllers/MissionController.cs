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
        //Get List Mission
        [Route("Mission/ListMissionComplete")]
        [HttpGet]
        public IHttpActionResult GetListMissonComplete([FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                string _message = "Danh sách nhiệm vụ hoàn thành của một nhân viên";
                bool _status = true;

                var key = (from a in db.EMPLOYEEs
                           where (a.apiKey == apiKey)
                           join b in db.MISSION_PROCESSes on a.id_employee equals b.id_employee
                           join c in db.MISSIONs on b.id_mission equals c.id_mission
                           where (b.status == 1)
                           select new {b.id_mission,c.name_mission, c.point, b.date}).ToList();

                if (!key.Any())
                    _message = "Nhân viên chưa hoàn thành nhiệm vụ nào hết !";

                return Ok(new { results = key, status = _status, message = _message });
            }
            else
                return Ok(new { results = "", status = "false", message = "Not Found apiKey" });

        }
        //Get Describe Mission
        [Route("Mission/{id}/Describe")]
        [HttpGet]
        public IHttpActionResult GetDescribeMission(int id, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                string message = "Chi tiết của một nhiệm vụ";
                bool status = true;
                var key = db.MISSIONs.Where(s => s.id_mission == id).Select(m=> new { m.id_mission, m.name_mission, m.Stardate, m.point, m.exprie, m.describe, m.id_type, m.id_employee });
                if (!key.Any())
                    message = "Không có chi tiết nhiệm vụ nào hết !";
                return Ok(new { results = key, status = status, message = message });
            }
            else
                return Ok(new { results = "", status = "Flase", message = "Not Found apiKey" });

        }
        //Post Mission
        [Route("Mission/Post")]
        [HttpPost]
        public IHttpActionResult PostMission(MISSION mission, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(s => s.apiKey.Equals(apiKey) && s.level_employee==true).Select(x => x.id_employee).Count();
                string messeage = "Tạo nhiệm vụ thành công !";
                var key = db.EMPLOYEEs.Where(s => s.apiKey == apiKey);
                db.MISSIONs.InsertOnSubmit(mission);
                db.SubmitChanges();
                return Ok(new { message = messeage });
            }
            else
                return Ok(new { message = "Not Found apiKey" });
        }

        //Confim Mission of Admin
        [Route("Mission/{id}/Confim")]
        [HttpPut]
        public IHttpActionResult ConfimMission(int id, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(s => s.apiKey.Equals(apiKey) && s.level_employee == true).Select(x => x.id_employee).Count();
                if (check > 0)
                {
                    foreach (var i in db.MISSIONs)
                    {
                        var cm = db.MISSIONs.Where(x => x.id_mission == id).Select(x => x).SingleOrDefault();
                        cm.status = 1;
                        db.SubmitChanges();
                        break;
                    }
                    return Ok(new { message = "Xác nhận xét duyệt thành công !!!" });
                }
                else
                    return Ok(new { message = "Không đủ quyền hạn !!!" });
            }
            else
                return Ok(new { message = "Not Found apiKey" });
        }
        //Get Search Mission
        [Route("Mission/Search")]
        [HttpGet]
        public IHttpActionResult GetSearchMission([FromUri] string key, [FromUri] string apiKey)
        {
            if (apiKey != null && key != null)
            {
                string _messeage = "Tìm kiếm thành công !";
                bool _status = true;
                var search = db.MISSIONs.Where(s => s.name_mission.Contains(key)).Select(a=> new { a.id_mission, a.name_mission, a.Stardate, a.point, a.exprie, a.describe, a.id_type, a.id_employee } );
                if (!search.Any())

                    _messeage = "Không tìm thấy nhiệm vụ nào !";
                return Ok(new { result = search, status = _status, messeage = _messeage });
            }
            else
                return Ok(new { results = "", status = "false", message = "Not Found apiKey" });

        }
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
