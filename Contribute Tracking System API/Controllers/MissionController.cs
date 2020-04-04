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
        [Route("MissionProcess")]
        [HttpGet]
        public IHttpActionResult MissionProcess([FromUri] string apiKey)
        {
            string _message = "Danh sách chờ duyệt thực hiện nhiệm vụ tất cả nhân viên nhân viên";
            bool _status = true;
            if (apiKey != null)
            {
                var checkAPI = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status == true && x.level_employee == true).Count();
                if(checkAPI!=0)
                {
                    var listMission = db.MISSION_PROCESSes.Where(x => x.status == -1).Select(x => new
                    {
                        x.id,
                        x.MISSION.name_mission,
                        x.id_mission,
                        x.EMPLOYEE.name_employee

                    }).ToList();
                    return Ok(new { results = listMission, status = _status, message = _message });
                }
                else
                    return Ok(new { results = "", status = "false", message = "Không đủ quyền hạn" });


            }
            else
                return Ok(new { results = "", status = "false", message = "Not Found apiKey" });

        }
        #region Search List mission complete
        #endregion
        [Route("Mission/ListMissionComplete/Search")]
        [HttpGet]
        public IHttpActionResult GetListMissonCompleteSearch([FromUri] string key, [FromUri] string apiKey)
        {
            if (apiKey != null && key!=null)
            {
                string _message = "Danh sách nhiệm vụ hoàn thành của một nhân viên";
                bool _status = true;

                var result = db.MISSION_PROCESSes.Where(x => x.EMPLOYEE.apiKey == apiKey && x.status == 1 && (x.MISSION.name_mission.Contains(key)  || x.MISSION.describe.Contains(key))).Select(x => new
                {
                    x.id_mission, x.MISSION.name_mission, x.MISSION.TYPE_MISSION.name_type_mission, x.MISSION.point,x.date
                }).ToList();
                if (!key.Any())
                    _message = "Không tìm thấy nhiệm vụ nào hoàn thành với tên đó !";

                return Ok(new { results = result, status = _status, message = _message });
            }
            else
                return Ok(new { results = "", status = "false", message = "Bad request" });

        }
        //Get Describe Mission
        [Route("Mission/{id}/Describe")]
        [HttpGet]
        public IHttpActionResult GetDescribeMission(int id)
        {
                string message = "Chi tiết của một nhiệm vụ";
                bool status = true;
                var getMission = db.MISSIONs.Where(x => x.id_mission == id).Select(x => x).FirstOrDefault();
            if(getMission.Count==0)
            {
                int countProcess = db.MISSION_PROCESSes.Where(x => x.id_mission == getMission.id_mission).Count();
                var key = db.MISSIONs.Where(s => s.id_mission == id).Select(m => new { m.id_mission, m.name_mission, m.Stardate, m.point, m.exprie, m.describe, m.id_type, m.id_employee, m.TYPE_MISSION.name_type_mission, m.EMPLOYEE.name_employee, m.Count, countProcess });
                if (!key.Any())
                    message = "Không có chi tiết nhiệm vụ nào hết !";
                return Ok(new { results = key, status = status, message = message });
            }
            else
            {
                int count = db.MISSION_PROCESSes.Where(x => x.id_mission == getMission.id_mission).Count();
                int countProcess = getMission.Count - count;
                var key = db.MISSIONs.Where(s => s.id_mission == id).Select(m => new { m.id_mission, m.name_mission, m.Stardate, m.point, m.exprie, m.describe, m.id_type, m.id_employee, m.TYPE_MISSION.name_type_mission, m.EMPLOYEE.name_employee, countProcess, m.Count });
                if (!key.Any())
                    message = "Không có chi tiết nhiệm vụ nào hết !";
                return Ok(new { results = key, status = status, message = message });
            }

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
                    var complete = db.MISSION_PROCESSes.Where(x => x.id_employee == check && x.id_mission == id && x.status==0).Select(x => x).SingleOrDefault();
                    complete.status = 1;
                    complete.date = DateTime.Now;
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
        [Route("MissionOrder/{id}/Confirm")]
        [HttpPut]
        public IHttpActionResult ConfirmOrderMission(int id, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                var checkAPI = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.level_employee == true && x.status == true).Count();
                if(checkAPI!=0)
                {
                    var listOrder = db.MISSION_PROCESSes.Where(x => x.id == id).Select(x=>x).FirstOrDefault();
                    listOrder.status = 0;
                    db.SubmitChanges();
                    return Ok(new { message = "Xét duyệt nhận nhiệm vụ thành công !!" });
                }
                else
                    return Ok(new { message = "Bạn không đủ quyền hạn !!" });
            }
            else
                return Ok(new { message = "Not Found apiKey" });
        }
        [Route("MissionOrder/{id}/Delete")]
        [HttpPut]
        public IHttpActionResult DeleteOrderMission(int id, [FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                var checkAPI = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.level_employee == true && x.status == true).Count();
                if (checkAPI != 0)
                {
                    var listOrder = db.MISSION_PROCESSes.Where(x => x.id == id).Select(x => x).FirstOrDefault();
                    db.MISSION_PROCESSes.DeleteOnSubmit(listOrder);
                    db.SubmitChanges();
                    return Ok(new { message = "Từ chối nhận nhiệm vụ thành công !!" });
                }
                else
                    return Ok(new { message = "Bạn không đủ quyền hạn !!" });
            }
            else
                return Ok(new { message = "Not Found apiKey" });
        }
        //Get Search Mission available
        [Route("Mission/Search")]
        [HttpGet]
        public IHttpActionResult GetSearchMission([FromUri] string key)
        {
            string _message = "Danh sách nhiệm vụ đang còn";
            bool _status = true;
            List<object> result = new List<object>();
            foreach (var t in db.MISSIONs.Where(x => x.status == 1))
            {
                var id_mission = t.id_mission;
                var starday = t.Stardate;
                DateTime finish = starday.AddDays(t.exprie);
                var cou = db.MISSIONs.Where(a => a.id_mission == id_mission).Select(x => x.Count).FirstOrDefault();
                var idmiss = db.MISSION_PROCESSes.Where(b => b.id_mission == id_mission).Select(x => x.id_mission).Count();
                if ((int.Parse(cou.ToString()) - idmiss > 0 || int.Parse(cou.ToString()) == 0) && finish >= DateTime.Now)
                {
                    result.AddRange(from a in db.MISSIONs

                                 join c in db.EMPLOYEEs on a.id_employee equals c.id_employee
                                 where a.id_mission == id_mission && (a.name_mission.Contains(key) || a.describe.Contains(key))
                                 select new
                                 {
                                     a.id_mission,
                                     a.name_mission,
                                     a.Stardate,
                                     a.point,
                                     a.exprie,
                                     a.describe,
                                     a.TYPE_MISSION.name_type_mission,
                                     c.id_employee,
                                     c.name_employee
                                 });
                }
            }
            return Ok(new { results = result, status = _status, message = _message });

        }
        //Get list mission available
        [Route("Missison/Missionavailable")]
        [HttpGet]
        public IHttpActionResult Missonavailable()
        {

            string _message = "Danh sách nhiệm vụ đang còn";
            bool _status = true;
            List<object> key =  new List<object>();
            foreach (var t in db.MISSIONs.Where(x=>x.status==1))
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
                        a.TYPE_MISSION.name_type_mission,
                        c.id_employee,
                        c.name_employee
                    }); 
                }
            }
            return Ok(new { results = key, status = _status, message = _message });
        }
        //Search list mission process
        [Route("Mission/Missionavailableemp/Search")]
        [HttpGet]
        public IHttpActionResult MissionavailableempSearch([FromUri] string key, [FromUri] string apiKey)
        {
            var miss= new object();
            string _message = "";
            bool _status = true;
            if (apiKey != null && key!=null)
            {
                _message = "Danh sách nhiệm vụ đang làm của 1 nhân viên";
                _status = true;
                var id = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Select(x => x.id_employee).SingleOrDefault();
                var result = db.MISSION_PROCESSes.Where(x => x.id_employee== id && DateTime.Compare(new DateTime(x.MISSION.Stardate.Year, x.MISSION.Stardate.Month, x.MISSION.Stardate.Day).AddDays(x.MISSION.exprie), DateTime.Now) >0 && x.status == 0 && (x.MISSION.name_mission.Contains(key) || x.MISSION.describe.Contains(key))).Select(a => new
                {
                    a.id_mission,
                    a.MISSION.name_mission,
                    a.MISSION.Stardate,
                    a.MISSION.point,
                    a.MISSION.exprie,
                    a.MISSION.describe,
                    a.MISSION.id_type,
                    a.MISSION.TYPE_MISSION.name_type_mission,
                    a.id_employee,
                    a.EMPLOYEE.name_employee
                }).ToList();
                 miss = result;
            }
            else _message = "Bạn chưa nhập apiKey";
            return Ok(new { results = miss, status = _status, message = _message });
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
                foreach (var t in db.MISSION_PROCESSes.Where(x=>x.id_employee==id && x.MISSION.status==1))
                {
                    if (t.id_employee == id)
                    {
                                miss.AddRange(from a in db.MISSIONs 
                                join c in db.EMPLOYEEs on a.id_employee equals c.id_employee 
                                where t.status == 0 && t.id_employee == id && a.id_mission== t.id_mission && DateTime.Compare(new DateTime(a.Stardate.Year, a.Stardate.Month, a.Stardate.Day).AddDays(a.exprie), DateTime.Now) > 0
                                              select new
                                {
                                    a.id_mission,
                                    a.name_mission,
                                    a.Stardate,
                                    a.point,
                                    a.exprie,
                                    a.describe,
                                    a.id_type,
                                    a.TYPE_MISSION.name_type_mission,
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
            var key = db.MISSIONs.Select(x => new { x.id_mission, x.name_mission, x.Stardate, x.point, x.exprie, x.describe, x.status,  x.Count,  x.id_type, x.id_employee,x.EMPLOYEE.name_employee, x.TYPE_MISSION.name_type_mission });
            if(!key.Any())
            {
                _message = "Không có danh sách nhiệm vụ";
            }
            return Ok(new { results = key, status = _status, message = _message });

        }
        [Route("Mission/{id}/Order")]
        [HttpPost]
        public IHttpActionResult Order(int id, [FromUri] string apiKey)
        {

            if (id != 0 && apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status == true).Select(x => x).FirstOrDefault();
                
                var getMission = db.MISSIONs.Where(x => x.id_mission == id).Select(x => x).FirstOrDefault();
                var getCount = db.MISSION_PROCESSes.Where(x => x.id_mission == getMission.id_mission).Count();
                if (check != null)
                {
                    if (getMission != null && (getMission.Count- getCount>=0 || getMission.Count==0))
                    {
                        var create_MS = db.MISSION_PROCESSes.Where(x => x.id_employee == check.id_employee && x.id_mission == getMission.id_mission).Select(x=>x).Count();
                        var checkProcess = db.MISSION_PROCESSes.Where(x => x.id_mission == getMission.id_mission && x.id_employee == check.id_employee && (x.status == 0 || x.status == -1)).Count();
                        if (create_MS ==0 || checkProcess==0)
                        {
                            string _message = "Nhận nhiệm vụ thành công !!";
                            var checkpoint = db.MISSION_PROCESSes.Where(x => x.id_mission == getMission.id_mission && x.id_employee == check.id_employee && x.status == 0 && DateTime.Compare(new DateTime(x.MISSION.Stardate.Year, x.MISSION.Stardate.Month, x.MISSION.Stardate.Day).AddDays(x.MISSION.exprie), DateTime.Now) < 0).Count();
                            MISSION_PROCESS mISSION_PROCESS = new MISSION_PROCESS();
                            if (checkpoint!=0)
                            {
                                mISSION_PROCESS.status = -1;
                                _message = "Vì bạn không hoàn thành nhiệm vụ này đúng thời hạn trước đó nên lần nhận nhiệm vụ này phải chờ duyệt !!";
                            }
                            mISSION_PROCESS.id_employee = check.id_employee;
                            mISSION_PROCESS.id_mission = getMission.id_mission;
                            db.MISSION_PROCESSes.InsertOnSubmit(mISSION_PROCESS);
                            db.SubmitChanges();
                            return Ok(new { message = "Nhận nhiệm vụ thành công !!" });
                        }
                        else
                        {
                            return Ok(new { message = "Nhiệm vụ này đã nhận rồi hãy đến hoàn thành nó. Nếu bạn không thấy không danh sách đang làm có thể nó đang được chờ duyệt !!" });
                        }
                    }
                    else
                    {
                        return Ok(new { message = "Rất tiếc! Nhiệm vụ này đã hết lượt nhận" });
                    }
                }
                else
                {
                    return Ok(new { message = "Nhân viên này không hoạt động" });
                }
            }
            else
            {
                return Ok(new { message = "Vui lòng chọn nhiệm vụ" });
            }

        }
        [Route("Mission/Edit")]
        [HttpPut]
        public IHttpActionResult Put([FromBody] MISSION mission, [FromUri] string apiKey)
        {
            if (mission != null && apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(x => x.apiKey.Equals(apiKey) && x.level_employee == true).Select(x => x).FirstOrDefault();
                if (check!= null)
                {
                    var update = db.MISSIONs.Where(x => x.id_mission == mission.id_mission).ToList();
                    update.ForEach(x =>
                    {
                        x.name_mission = mission.name_mission;
                        x.id_type = mission.id_type;
                        x.describe = mission.describe;
                        x.Count = mission.Count;
                        x.point = mission.point;
                        x.id_employee = x.id_employee;
                    });
                    db.SubmitChanges();
                    return Ok(new { massage = "Sửa nhiệm vụ thành công!" });
                }
                else
                {
                    return Ok(new { massage = "Không có quyền sửa!" });
                }
            }
            else
            {
                return Ok(new { massage = "Vui lòng nhập thông tin!" });
            }
        }
    }
}
