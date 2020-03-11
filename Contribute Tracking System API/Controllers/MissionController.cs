using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contribute_Tracking_System_API.Controllers
{
    public class MissionController : ApiController
    {
        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        [Route("Mission")]
        [HttpGet]
        // GET: api/Mission
        public IEnumerable<MISSION> Get()
        {
            return db.MISSIONs.ToList<MISSION>();
        }
        // GET: api/Mission/5
        public string Get(int id)
        {
            return "value";
        }
        [Route("Mission")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]MISSION mission, [FromUri]string apiKey)
        {
            var check = db.EMPLOYEEs.Where(x => x.apiKey.Equals(apiKey)).Count();
            if(check > 0)
            {
                var checkID = db.MISSIONs.Where(x => x.id_mission != mission.id_mission).Count();
                if(checkID > 0)
                {
                    db.MISSIONs.InsertOnSubmit(mission);
                    db.SubmitChanges();
                    return Ok(new { message = "Thêm thành công!" });
                }
                else
                {
                    return Ok(new { message = "Nhiệm vụ này đã tồn tại  !" });
                }
                
            }
            else
            {
                return Ok(new { message = "Thêm không thành công!" });
            }
        }

        // PUT: api/Mission/5
        public IHttpActionResult Put(int id, [FromUri]string apiKey)
        {
            var check = db.EMPLOYEEs.Where(x => x.apiKey.Equals(apiKey)).Count();
            if(check > 0)
            {
                var delete = db.MISSIONs.Where(x => x.id_mission == id).ToList();
                delete.ForEach(x => { x.status = -1; });
                db.SubmitChanges();
                return Ok(new { id = id, message = "Xóa thành công!" });
            }
            else
            {
                return Ok(new { id = id, message = "Xóa không thành công!" });
            }
        }

       

        // DELETE: api/Mission/5
        public void Delete(int id)
        {
        }
    }
}
