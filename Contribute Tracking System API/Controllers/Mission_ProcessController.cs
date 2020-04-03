using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contribute_Tracking_System_API.Controllers
{
    public class Mission_ProcessController : ApiController
    {
        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        // GET: api/Mission_Process
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Mission_Process/5
        public string Get(int id)
        {
            return "value";
        }

        [Route("Mission_process")]
        [HttpPost]
        // POST: api/Mission_Process
        public IHttpActionResult Post([FromUri]int id_mission, [FromUri] string apiKey)
        {
            
            if(id_mission != 0 && apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status == true).Select(x => x).FirstOrDefault();
                var getMission = db.MISSIONs.Where(x => x.id_mission == id_mission && x.Count >= 0).Select(x => x).FirstOrDefault();
                if (check != null)
                {
                    if(getMission != null)
                    {
                        var create_MS = db.MISSION_PROCESSes.Where(x => x.id_employee == check.id_employee && x.id_mission == getMission.id_mission).SingleOrDefault();
                        if(create_MS==null)
                        {
                            MISSION_PROCESS mISSION_PROCESS = new MISSION_PROCESS();
                            mISSION_PROCESS.id_employee = check.id_employee;
                            mISSION_PROCESS.id_mission = getMission.id_mission;
                            db.MISSION_PROCESSes.InsertOnSubmit(mISSION_PROCESS);
                            db.SubmitChanges();
                            return Ok(new { message = "Nhận nhiệm vụ thành!" });
                        }
                        else
                        {
                            return Ok(new { message = "Bạn đã nhận nhiệm vụ này rồi" });
                        }
                    }
                    else
                    {
                        return Ok(new { message = "Rất tiếc! Nhiệm vụ này đã hết lượt nhận"});
                    }
                }
                else
                {
                    return Ok(new { message = "Nhân viên này không hoạt động" });
                }
            }
            else
            {
                return Ok(new { message = "Vui lòng chọn nhiệm vụ"});
            }

        }

        // PUT: api/Mission_Process/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Mission_Process/5
        public void Delete(int id)
        {
        }
    }
}
