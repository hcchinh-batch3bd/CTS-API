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
        //Hủy Mission
        [Route("Mission/{id}/ClearMission")]
        [HttpPut]
        public IHttpActionResult ClearMission(int id, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(s => s.apiKey == apiKey && s.status== true).Select(a => a.id_employee).SingleOrDefault();
                if (check > 0)
                {
                    var level = db.EMPLOYEEs.Where(b => b.id_employee == check && b.level_employee == true).Select(b => b.id_employee).SingleOrDefault();
                    if (level > 0)
                    {
                        var check2 = db.MISSIONs.Where(x => x.id_mission == id).Select(x => x.id_mission).SingleOrDefault();
                        if (check2 > 0)
                        {
                            var clear = db.MISSIONs.Where(x=>x.id_mission == check2).ToList();
                            clear.ForEach(x => { x.status = -1; });
                            db.SubmitChanges();
                            return Ok(new { message = "Tạm hủy thành công !" });
                        }
                        else
                        {
                            return Ok(new { message = "Không có id misson này!" });
                        }
                    }
                    else
                    {
                        return Ok(new { message = "Bạn không đủ quyền hạn này" });
                    }
                }
                else
                {
                    return Ok(new { message = "Không có id employee này!" });

                }
            }
            else
            {
                return Ok(new { message = "Bad request" });

            }


        }
         
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
                           where (a.apiKey == apiKey && a.status==true)
                           join b in db.MISSION_PROCESSes on a.id_employee equals b.id_employee
                           join c in db.MISSIONs on b.id_mission equals c.id_mission
                           where (b.status == 1)
                           select new {b.id_mission,c.name_mission, c.TYPE_MISSION.name_type_mission, c.point, b.date}).ToList();

                if (!key.Any())
                    _message = "Nhân viên chưa hoàn thành nhiệm vụ nào hết !";

                return Ok(new { results = key, status = _status, message = _message });
            }
            else
                return Ok(new { results = "", status = "false", message = "Not Found apiKey" });

        }
        //Get list mission 

        //Get Describe Mission
        [Route("Mission/{id}/Describe")]
        [HttpGet]
        public IHttpActionResult GetDescribeMission(int id)
        {
                string message = "Chi tiết của một nhiệm vụ";
                bool status = true;
                var key = db.MISSIONs.Where(s => s.id_mission == id).Select(m=> new { m.id_mission, m.name_mission, m.Stardate, m.point, m.exprie, m.describe, m.id_type, m.id_employee });
                if (!key.Any())
                    message = "Không có chi tiết nhiệm vụ nào hết !";
                return Ok(new { results = key, status = status, message = message });

        }
        //Create Mission
        [Route("Mission/Create")]
        [HttpPost]
        public IHttpActionResult PostMission(MISSION mission, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(s => s.apiKey.Equals(apiKey) && s.status== true).Select(x=> new {x.id_employee, x.level_employee}).SingleOrDefault();
                if(check!=null)
                {
                    string messeage = "Tạo nhiệm vụ thành công !";
                    if (check.level_employee)
                    {
                        mission.status = 1;
                    }
                    else
                        mission.status = 0;
                    mission.id_employee = check.id_employee;
                    db.MISSIONs.InsertOnSubmit(mission);
                    db.SubmitChanges();
                    return Ok(new { message = messeage });
                }
               else
                    return Ok(new { message = "Tài khoản này không tồn tại" });
            }
            else
                return Ok(new { message = "Not Found apiKey" });
        }
        //Complete Mission
        [Route("Mission/{id}/CompleteMission")]
        [HttpPut]
        public IHttpActionResult CompleteMission(int id, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(s => s.apiKey.Equals(apiKey)).Select(x => x.id_employee).SingleOrDefault();
                if (check > 0)
                {
                    var complete = db.MISSION_PROCESSes.Where(x => x.id_employee == check && x.id_mission == id).Select(x => x).SingleOrDefault();
                    complete.status = 1;
                    var pointmission = db.MISSIONs.Where(a => a.id_mission == id).Select(a => a.point).SingleOrDefault();
                    var check2 = db.EMPLOYEEs.Where(s => s.id_employee== check).Select(x => x).SingleOrDefault();
                    check2.point = check2.point + pointmission;
                    db.SubmitChanges();
                    return Ok(new { message = "Xác nhận nhiệm vụ thành công!" });
                }
                else
                {
                    return Ok(new { message = "Bạn không thể xác nhận nhiệm vụ của người khác!" });
                }
            }
            else
                return Ok(new { message = "Not Found apiKey" });

        }
        //Confim Mission of Admin
        [Route("Mission/{id}/Confirm")]
        [HttpPut]
        public IHttpActionResult ConfirmMission(int id, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(s => s.apiKey.Equals(apiKey) && s.status==true && s.level_employee == true).Select(x => x.id_employee).Count();
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
        public IHttpActionResult GetSearchMission([FromUri] string key)
        {
            if (key != null)
            {
                string _messeage = "Tìm kiếm thành công !";
                bool _status = true;
                var search = db.MISSIONs.Where(s => s.name_mission.Contains(key)).Select(a=> new { a.id_mission, a.name_mission, a.Stardate, a.point, a.exprie, a.describe, a.id_type, a.id_employee } );
                if (!search.Any())

                    _messeage = "Không tìm thấy nhiệm vụ nào !";
                return Ok(new { result = search, status = _status, messeage = _messeage });
            }
            else
                return Ok(new { results = "", status = "false", message = "Chưa nhập từ khóa cần tìm kiếm !!" });

        }
        //Get list mission available
        [Route("Missison/Missionavailable")]
        [HttpGet]
        public IHttpActionResult Missonavailable()
        {

            string _message = "Danh sách nhiệm vụ đang còn";
            bool _status = true;
            List<object> key =  new List<object>();
            foreach (var t in db.MISSIONs)
            {
                var id_mission = t.id_mission;
                var starday = t.Stardate;
                DateTime finish = starday.AddDays(t.exprie);
                var cou = db.MISSIONs.Where(a => a.id_mission == id_mission).Select(x => x.Count).FirstOrDefault();
                var idmiss = db.MISSION_PROCESSes.Where(b => b.id_mission == id_mission).Select(x => x.id_mission).Count();
                if ((int.Parse(cou.ToString()) - idmiss > 0 || int.Parse(cou.ToString()) == 0) && finish>=DateTime.Now)
                {
                    key.AddRange( from a in db.MISSIONs
                    
                    join c in db.EMPLOYEEs on a.id_employee equals c.id_employee
                    where a.id_mission == id_mission
                    select new
                    {
                        a.id_mission,
                        a.name_mission,
                        a.Stardate,
                        a.point,
                        a.exprie,
                        a.describe,
                        a.id_type,
                        c.id_employee,
                        c.name_employee
                    }); 
                }
            }
            return Ok(new { results = key, status = _status, message = _message });
        }
        //Get list mission process of employee
        [Route("Mission/Missionavailableemp")]
        [HttpGet]
        public IHttpActionResult Missionavailableemp([FromUri] string apiKey)
        {
            List<object> miss = new List<object>();
            string _message = "";
            bool _status = true;
            if (apiKey != null)
            {
                _message = "Danh sách nhiệm vụ đang làm của 1 nhân viên";
                _status = true;
                var id = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Select(x => x.id_employee).SingleOrDefault();
                foreach (var t in db.MISSION_PROCESSes.Where(x=>x.id_employee==id))
                {
                    if (t.id_employee == id)
                    {
                                miss.AddRange(from a in db.MISSIONs 
                                join c in db.EMPLOYEEs on a.id_employee equals c.id_employee 
                                where t.status == 0 && t.id_employee == id && a.id_mission== t.id_mission
                                select new
                                {
                                    a.id_mission,
                                    a.name_mission,
                                    a.Stardate,
                                    a.point,
                                    a.exprie,
                                    a.describe,
                                    a.id_type,
                                    c.id_employee,
                                    c.name_employee
                                });
                    }
                }
            }
            else _message = "Bạn chưa nhập apiKey";
            return Ok(new { results = miss, status = _status, message = _message });
        }
       
        //Get list mission
        [Route("Mission/ListMission")]
        [HttpGet]
        public IHttpActionResult GetListMission()
        {
            string _message = "Danh sách nhiệm vụ !!";
            bool _status = true;
            var key = db.MISSIONs.Select(x => new { x.id_mission, x.name_mission, x.Stardate, x.point, x.exprie, x.describe, x.status,  x.Count,  x.id_type, x.id_employee });
            if(!key.Any())
            {
                _message = "Không có danh sách nhiệm vụ";
            }
            return Ok(new { results = key, status = _status, message = _message });

        }
    }
}
