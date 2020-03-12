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
    public class AccountController : ApiController
    {

        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        [Route("Account/CheckLogin")]
        [HttpGet]
        public IHttpActionResult GetCheckLogin([FromUri] string id, [FromUri] string pw)
        {
            string message = "Bad request";
            bool status = false;
            if (id!=null && pw!=null)
            {
                message = "Đăng nhập thành công !!";
                status = true;
                var key = db.EMPLOYEEs.Where(x => x.id_employee == int.Parse(id) && x.password == CreateMD5Hash(pw)).Select(s => new { s.name_employee, s.point, s.apiKey });
                if (!key.Any())
                    message = "ID đăng nhập hoặc mật khẩu không hợp lê !!";
                return Ok(new { results = key, status = status, message = message });
            }
            else return Ok(new { results = "", status = status, message = message });
        }
            
           
        [Route("Account/OTP")]
        [HttpGet]
        public IHttpActionResult SendOTP([FromUri] string mail)
        {
            string _message = "Bad request";
            bool _status = false;
            if (mail!=null)
            {
                _status = true;
                Random rnd = new Random();
                int otp = rnd.Next(000000, 999999);
                string msg = "OTP để xác nhận lấy lại mật khẩu của bạn : " + otp;
                bool f = SendOTP("lvx51523@outlook.com", mail, "Xác thực mail OTP", msg);
                if (f)
                {
                    _message = "Đã gửi OTP đến mail !!";
                }
                else { _message = "Gửi OTP thất bại, thử lại sau !! !!"; }
            }
            return Ok(new { results = "", status = _status, message = _message });

        }
        [Route("Account/Changepassword")]
        [HttpPut]
        public IHttpActionResult ChangePassword([FromUri] string passnew, [FromUri] string passold, [FromUri] string apiKey)
        {
            string _message = "";
            bool _status = true;
            if (passnew != null && passold != null && apiKey != null)
            {
                _message = "Mật khẩu cũ không đúng !!";
                _status = true;
                var changpass = db.EMPLOYEEs.Where(x => x.password == passold && x.apiKey == apiKey).Select(x => x).SingleOrDefault();
                if (changpass != null)
                {
                    changpass.password = passnew;
                    db.SubmitChanges();
                    _message = " Đổi mật khẩu thành công";
                }
            }
            else _message = "Ban chua nhap day du thong tin";
            return Ok(new { results = "", status = _status, message = _message });
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
        public bool SendOTP(string from, string to, string subject, string body)
        {
            bool f = false;
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(to);
                mailMessage.From = new MailAddress(from);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("lvx51523@outlook.com", "opencart123");
                client.Port = 25; // 25 587
                client.Host = "smtp.office365.com";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                
                client.Send(mailMessage);
                f = true;
            }
            catch (Exception ex)
            {
                f = false;
            }
            return f;
        }
    }
}
