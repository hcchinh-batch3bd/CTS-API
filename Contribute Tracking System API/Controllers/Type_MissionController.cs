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

        [Route("Type_Mission")]
        [HttpPost]
        // POST: Type_Mission/apiKey
        public IHttpActionResult Post([FromBody] TYPE_MISSION type_mission, [FromUri]string apiKey)
        {
            if(type_mission != null)
            {
                var check = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Count();
                if (check > 0)
                {
                    db.TYPE_MISSIONs.InsertOnSubmit(type_mission);
                    db.SubmitChanges();
                    return Ok(new { message = "Thêm loại nhiệm vụ thành công!" });
                }
                else
                {
                    return Ok(new { message = "Thêm loại nhiệm vụ không thành công!" });
                }
            }
            else
            {
                return Ok(new { message = "Vui lòng nhập loại nhiệm vụ" });
            }
        }

        [Route("Type_Mission")]
        [HttpPut]
        // PUT: /Type_Mission?apiKey
        public IHttpActionResult Put([FromBody]TYPE_MISSION type_mission, [FromUri]string apiKey)
        {
            if (type_mission != null)
            {
                var check = db.EMPLOYEEs.Where(x => x.apiKey.Equals(apiKey)).Count();
                if (check > 0)
                {
                    var update = db.TYPE_MISSIONs.Where(x => x.id_type == type_mission.id_type).ToList();
                    update.ForEach(x =>
                    {
                        x.name_type_mission = type_mission.name_type_mission;
                        x.status = type_mission.status;
                        x.id_employee = type_mission.id_employee;
                        x.date = type_mission.date;
                    });
                    db.SubmitChanges();
                    return Ok(new { massage = "Sửa loại nhiệm vụ thành công!" });
                }
                else
                {
                    return Ok(new { massage = "Sửa loại nhiệm vụ không thành công!" });
                }
            }
            else
            {
                return Ok(new { massage = "Vui lòng nhập thông tin!" });
            }
        }

        // PUT:Type_Mission/5
        public void Put([FromUri]int id, [FromUri]string apiKey)
        {

        }

        // DELETE: Type_Mission/5
        public void Delete(int id)
        {
        }
    }
}
