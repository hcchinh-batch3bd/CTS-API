using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contribute_Tracking_System_API.Controllers
{
    public class Type_MissionController : ApiController
    {
        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        [Route("Type_Mission")]
        [HttpGet]
        // GET: api/Type_Mission
        public IEnumerable<TYPE_MISSION> Get()
        {
            return db.TYPE_MISSIONs.ToList<TYPE_MISSION>();
        }

        // GET: api/Type_Mission/5
        public string Get(int id)
        {
            return "value";
        }
        [Route("Type_Mission")]
        [HttpPost]
        // POST: api/Type_Mission
        public IHttpActionResult Post([FromBody] TYPE_MISSION type_mission ,[FromUri]string apiKey)
        {
            var check = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Count();
            if(check > 0)
            {
                var checkID = db.TYPE_MISSIONs.Where(x => x.id_type == type_mission.id_type).Count();
                if(checkID > 0)
                {
                    return Ok(new { message = "Loại nhiệm vụ đã tồn tại!" });
                }
                else
                {
                    db.TYPE_MISSIONs.InsertOnSubmit(type_mission);
                    db.SubmitChanges();
                    return Ok(new { message = "Thêm loại thành công!" });
                }
            }
            else
            {
                return Ok(new { message = "Thêm loại không thành công!" });
            }
            
        }

        [Route("Type_Mission")]
        [HttpPut]
        // PUT: api/Type_Mission/5
        public IHttpActionResult Put([FromBody]TYPE_MISSION type_mission, [FromUri]string apiKey)
        {
            var check = db.EMPLOYEEs.Where(x => x.apiKey.Equals(apiKey)).Count();
            if(check > 0)
            {
                var update = db.TYPE_MISSIONs.Where(x=>x.id_type == type_mission.id_type).ToList();
                update.ForEach(x =>
                {
                    x.name_type_mission = type_mission.name_type_mission;
                });
                db.SubmitChanges();
                return Ok(new { massage = "Sửa thành công" });
            }
            else
            {
                return Ok(new { massage = "Sửa không thành công" });
            }
            
        }

        // DELETE: api/Type_Mission/5
        public void Delete(int id)
        {
        }
    }
}
