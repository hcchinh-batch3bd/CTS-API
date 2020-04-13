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
        /// <summary>
        /// This method GetAll Type Mission 
        /// </summary>
        /// <returns> a json contraining result value(id_type, name_type_mission, id+employee, status, date)</returns>
        [Route("Type_Mission/GetAll")]
        [HttpGet]
        public IEnumerable<object> GetAll()
        {
            return db.TYPE_MISSIONs.Select(x => new {
                x.id_type,
                x.name_type_mission,
                x.id_employee,
                x.status,
                x.date
            });
        }
        /// <summary>
        /// This method GetAll Type Mission with status employee == true
        /// </summary>
        /// <returns>a json contraining result value(id_type, name_type_mission, id+employee, status, date)</returns>
        [Route("Type_Mission")]
        [HttpGet]
        // GET: Type_mission Lấy tất cả loại nhiệm vụ status = true
        public IEnumerable<object> Get()
        {
            return db.TYPE_MISSIONs.Where(x=>x.status == true).Select(x=> new {
                x.id_type,
                x.name_type_mission,
                x.id_employee,
                x.status,
                x.date
            });
        }
        /// <summary>
        /// This method getall type mission with id misson == id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a json contraining result value(id_type, name_type_mission, id+employee, status, date)</returns>
        [Route("Type_Mission/{id}")]
        [HttpGet]
        // GET: Type_mission?id=8 Lấy tất cả loại nhiệm vụ theo id
        public IEnumerable<object> Get(int id)
        {
            return db.TYPE_MISSIONs.Where(x => x.status == true && x.id_type == id).Select(x => new {
                x.id_type,
                x.name_type_mission,
                x.id_employee,
                x.status,
                x.date
            });
        }
        /// <summary>
        /// This method create one type mission
        /// </summary>
        /// <param name="type_mission"></param>
        /// <param name="apiKey"></param>
        /// <returns>a json contraining reslut message </returns>

        [Route("Type_Mission/Create")]
        [HttpPost]
        // POST: Type_Mission/apiKey Thêm loại nhiệm vụ
        public IHttpActionResult Post([FromBody] TYPE_MISSION type_mission,[FromUri] string apiKey)
        {
            if(type_mission != null && apiKey!=null)
            {
                var check = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status ==true).Select(x=>x.level_employee).FirstOrDefault();
                if (check)
                {
                    db.TYPE_MISSIONs.InsertOnSubmit(type_mission);
                    db.SubmitChanges();
                    return Ok(new { message = "Thêm loại nhiệm vụ thành công!" });
                }
                else
                {
                    return Ok(new { message = "Không có quyền thêm!" });
                }
            }
            else
            {
                return Ok(new { message = "Vui lòng nhập loại nhiệm vụ" });
            }
        }
        /// <summary>
        /// This method edit type mission
        /// </summary>
        /// <param name="type_mission"></param>
        /// <param name="apiKey"></param>
        /// <returns> a json contraining result a message </returns>
        [Route("Type_Mission/Edit")]
        [HttpPut]
        public IHttpActionResult Put([FromBody]TYPE_MISSION type_mission, string apiKey)
        {
            if (type_mission != null)
            {
                var check = db.EMPLOYEEs.Where(x => x.apiKey.Equals(apiKey)).Select(x=>x.level_employee).FirstOrDefault();
                if (check)
                {
                   
                    var update = db.TYPE_MISSIONs.Where(x => x.id_type == type_mission.id_type).ToList();
                    update.ForEach(x =>
                    {
                        x.name_type_mission = type_mission.name_type_mission;
                        x.status = type_mission.status;
                    });
                    db.SubmitChanges();
                    return Ok(new { massage = "Sửa loại nhiệm vụ thành công!" });
                }
                else
                {
                    return Ok(new { massage = "Không có quyền sửa!" });
                }
            }
            else
            {
                return Ok(new {massage = "Vui lòng nhập thông tin!" });
            }
        }
        /// <summary>
        /// This method delete type mission with id_name_mission == id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="apiKey"></param>
        /// <returns>a json contraining result a message</returns>
        [Route("Type_Mission/{id}/Remove")]
        [HttpPut]
        public IHttpActionResult Put(int id, [FromUri] string apiKey)
        {
            var check = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Select(x=>x.level_employee).FirstOrDefault();
            if (check)
            {
                var result = db.TYPE_MISSIONs.Where(x => x.id_type == id).ToList();
                result.ForEach(x => { x.status = false; });
                db.SubmitChanges();
                return Ok(new { message = "Xóa loại nhiệm vụ thành công!" });
            }
            else
            {
                return Ok(new { message = "Không có quyền xóa!"});
            }
        }

       
    }
}
