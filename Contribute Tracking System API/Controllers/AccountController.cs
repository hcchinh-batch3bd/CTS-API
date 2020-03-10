using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Text;
using System.Security.Cryptography;

namespace Contribute_Tracking_System_API.Controllers
{
    public class AccountController : ApiController
    {

        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        [Route("Account/CheckLogin")]
        [HttpGet]
        public IHttpActionResult GetCheckLogin([FromUri] string id, [FromUri] string pw)
        {

            string message = "Đăng nhập thành công !!";
            bool status = true;
            var key = db.EMPLOYEEs.Where(x => x.id_employee == int.Parse(id) && x.password == CreateMD5Hash(pw)).Select(s => new { name = s.name_employee, point = s.point, apiKey = s.apiKey });
            if (!key.Any())
                message = "ID đăng nhập hoặc mật khẩu không hợp lê !!";
            return Ok(new { results = key, status = status, message = message });
        }
        // GET: Account
        public IEnumerable<EMPLOYEE> Get()
        {
            return db.EMPLOYEEs.ToList<EMPLOYEE>();
        }
        // GET: Account/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: Account
        public void Post([FromBody]string value)
        {
        }

        // PUT: Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: Account/5
        public void Delete(int id)
        {
        }
        public string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
