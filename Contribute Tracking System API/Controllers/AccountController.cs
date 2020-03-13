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
            string _message = "Bad request";
            bool _status = false;
            if (id!=null && pw!=null)
            {
                _message = "Đăng nhập thành công !!";
                _status = true;
                var key = db.EMPLOYEEs.Where(x => x.id_employee == int.Parse(id) && x.password == CreateMD5Hash(pw)).Select(s => new { s.name_employee, s.point, s.apiKey });
                if (!key.Any())
                    _message = "ID đăng nhập hoặc mật khẩu không hợp lệ !!";
                return Ok(new { results = key, status = _status, message = _message });
            }
            else return Ok(new { results = "", status = _status, message = _message });
        }
        //Hủy Mission
        [Route("Mission/{id}/ClearMission")]
        [HttpPut]
        public IHttpActionResult ClearMission(int id, [FromUri] string apiKey)
        {
            if (id != null && apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(s => s.apiKey == apiKey).Select(a => a.id_employee).SingleOrDefault();
                if (check > 0)
                {
                    var level = db.EMPLOYEEs.Where(b => b.id_employee == check && b.level_employee == true).Select(b => b.id_employee).SingleOrDefault();
                    if (level > 0)
                    {
                        var check2 = db.MISSIONs.Where(x => x.id_mission == id).Select(x => x.id_mission).SingleOrDefault();
                        if (check2 > 0)
                        {
                            var clear = db.MISSIONs.Where(x => x.id_employee == level && x.id_mission == check2).ToList();
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
                    return Ok(new {  message = "Không có id employee này!" });

                }
            }
            else
            {
                return Ok(new { message = "Bạn chưa nhập id mission hoặc apiKey!" });

        }


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
            else _message = "Bạn chưa nhập dầy đủ thông tin !!";
            return Ok(new { results = "", status = _status, message = _message });
        }
        //Load List Employee
        [Route("Account/ListEmployee")]
        [HttpGet]
        public IHttpActionResult GetListEmployee([FromUri] string apiKey)
        {
            List<object> key = new List<object>();
            string _message = "Bad request!!";
            bool _status = true;
            if (apiKey != null)
            {
                int checkApiKey = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Select(x => x.apiKey).Count();
                if(checkApiKey != 0)
                {
                    _message = "Danh sách nhân viên!!";
                var checkLevel = db.EMPLOYEEs.Select(x => new { x.id_employee, x.level_employee, x.status });
                foreach (var item in checkLevel)
                {
                    if(item.level_employee == true && item.status == true )
                    {
                        key.AddRange(LoadData(item.id_employee, "Quản lý", "Hoạt động"));
                    }
                    else if (item.level_employee == true && item.status == false)
                        {
                        key.AddRange(LoadData(item.id_employee, "Quản lý", "Nghỉ việc"));
                        }
                    else if (item.level_employee == false && item.status == true)
                        {
                            key.AddRange(LoadData(item.id_employee, "Nhân viên", "Hoạt động"));
                        }
                    else
                        {
                            key.AddRange(LoadData(item.id_employee, "Nhân viên", "Nghỉ việc"));
                        } 
                        if (!key.Any())
                    {
                        _message = "Không có danh sách";
                    }
                }
                }
                return Ok(new { results = key, status = _status, message = _message });
            }
              
            else return Ok(new { results = "", status = _status, message = _message });
        }
        private IEnumerable<object> LoadData(int id, string level, string status)
        {
            return db.EMPLOYEEs.Where(x=> x.id_employee == id).Select(x => new {
                x.id_employee,
                x.name_employee,
                x.email,
                x.date,
                x.point,
                level,
                status
            });
        }
        [Route("Account/Information")]
        [HttpGet]
        public IHttpActionResult GetPoint([FromUri] string apiKey)
        {
            if (apiKey != null)
            {
                string mesage = "Thông tin nhân viên !";
                bool status = true;
                var infor = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Select(s => new { s.id_employee, s.name_employee, s.email, s.date, s.point, s.level_employee, s.status, s.apiKey });
                if (!infor.Any())
                    mesage = "Không có thông tin nhân viên này !";
                return Ok(new { results = infor, status = status, mesage = mesage });
            }
            else
                return Ok(new { results = "", status = "Flase", message = "Not Found apiKey" });
        }
        // GetET: Account
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
