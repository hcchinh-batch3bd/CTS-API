﻿using System;
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
        public IEnumerable<TYPE_MISSION> Get()
        {
            return db.TYPE_MISSIONs.ToList<TYPE_MISSION>();
        }

        // GET: Type_Mission/5
        public string Get(int id)
        {
            return "value";
        }


        [Route("Type_Mission")]
        [HttpPost]
        // POST: Type_Mission?apiKey=MjE0MTQyNDI0MQ==
        public IHttpActionResult Post([FromBody] TYPE_MISSION type_mission, [FromUri]string apiKey)
        {
            var check = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Count();
            if (check > 0)
            {
                var checkID = db.TYPE_MISSIONs.Where(x => x.id_type == type_mission.id_type).Count();
                if (checkID > 0)
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


        // PUT:Type_Mission/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: Type_Mission/5
        public void Delete(int id)
        {
        }
    }
}
