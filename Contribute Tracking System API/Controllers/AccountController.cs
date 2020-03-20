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
        /// <summary>
        /// This method Get check Login
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pw"></param>
        /// <returns> a json contraining result a message</returns>
        [Route("Account/CheckLogin")]
        [HttpGet]
        public IHttpActionResult GetCheckLogin([FromUri] string id, [FromUri] string pw)
        {
            string _message = "Bad request";
            bool _status = false;
            if (id != null && pw != null)
            {
                _message = "Đăng nhập thành công !!";
                _status = true;
                var key = db.EMPLOYEEs.Where(x => x.id_employee == int.Parse(id) && x.password == CreateMD5Hash(pw) && x.status==true).Select(s => new { s.name_employee, s.point, s.apiKey });
                if (!key.Any())
                    _message = "ID đăng nhập hoặc mật khẩu không hợp lệ !!";
                return Ok(new { results = key, status = _status, message = _message });
            }
            else return Ok(new { results = "", status = _status, message = _message });
        }
        /// <summary>
        /// This method Send a verification code (OTP) to retrieve your password
        /// </summary>
        /// <param name="mail"></param>
        /// <returns> a json contraining result a message</returns>
        //Gửi OTP
        [Route("Account/OTP")]
        [HttpGet]
        public IHttpActionResult SendOTP([FromUri] string mail)
        {
            string _message = "Bad request";
            if (mail != null)
            {
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
            return Ok(new {message = _message });

        }
        /// <summary>
        ///  This method Change the password for the account
        /// </summary>
        /// <param name="passnew"></param>
        /// <param name="apiKey"></param>
        /// <returns>a json contraining reslut a message</returns>
        //Đổi mật khẩu
        [Route("Account/Changepassword")]
        [HttpPut]
        public IHttpActionResult ChangePassword([FromUri] string passnew, [FromUri] string apiKey)
        {
            string _message = "";
            if (passnew != null && apiKey != null)
            {
                _message = "Bạn không có quyền đổi mật khẩu tài khoản này !!";
                var changpass = db.EMPLOYEEs.Where(x=>x.apiKey == apiKey && x.status==true).Select(x => x).SingleOrDefault();
                if (changpass != null)
                {
                    changpass.password = CreateMD5Hash(passnew);
                    db.SubmitChanges();
                    _message = " Đổi mật khẩu thành công";
                }
            }
            else _message = "Bạn chưa nhập dầy đủ thông tin !!";
            return Ok(new {message = _message });
        }
        /// <summary>
        ///  This method cancel a employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="apiKey"></param>
        /// <returns> a json contraining reslut a message</returns>
        //API xóa một nhân viên
        [Route("Account/{id}/DeleteEmployee")]
        [HttpPut]
        public IHttpActionResult DeleteEmployee(int id, [FromUri] string apiKey)
        {
            string _message = "Bad request";
            if (apiKey != null)
            {
                bool CheckApi = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status==true).Select(x => x.level_employee).FirstOrDefault();
                if (CheckApi)
                {
                    bool changeStatus = false;
                    int checkID = db.EMPLOYEEs.Where(x => x.id_employee == id).Count();
                    if (checkID > 0)
                    {
                        EMPLOYEE employee = db.EMPLOYEEs.Where(x => x.id_employee == id).SingleOrDefault();
                        if (employee.status == true)
                        {
                            employee.status = changeStatus;
                            db.SubmitChanges();
                            _message = "Xóa thành công";
                        }
                    }
                }
                else _message = "Không có quyền xóa";
            }
            return Ok(new {message = _message });
        }
        /// <summary>
        /// This method Rank employee by point
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns> a json contraining result a list rank with value ( id_employee, name_employee, point )</returns>
        //API xếp hạng nhân viên theo điểm
        [Route("Account/RankEmployee")]
        [HttpGet]
        public IHttpActionResult RankEmployee([FromUri] string apiKey)
        {
            object key = new object();
            bool _status = true;
            string _message = "Bad request";
            if (apiKey != null)
            {
                int CheckApi = db.EMPLOYEEs.Where(x => x.apiKey == apiKey).Select(x => x.apiKey).Count();
                if (CheckApi > 0)
                {
                    key = db.EMPLOYEEs.Where(x => x.status == true).OrderByDescending(x => x.point).Select(x => new { x.id_employee, x.name_employee, x.point });
                    _message = "Bảng xếp hạng nhân viên";
                }
                return Ok(new { results = key, status = _status, message = _message });
            }
            return Ok(new { results = "", status = _status, message = _message });
        }
        /// <summary>
        /// This method getall employee
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns>a json contraining result a list employee</returns>
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
                int checkApiKey = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status== true).Select(x => x.apiKey).Count();
                if (checkApiKey != 0)
                {
                    _message = "Danh sách nhân viên!!";
                    var checkLevel = db.EMPLOYEEs.Select(x => new { x.id_employee, x.level_employee, x.status });
                    foreach (var item in checkLevel)
                    {
                        if (item.level_employee == true && item.status == true)
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
        /// <summary>
        /// This method Loan data employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="status"></param>
        /// <returns> a json contraining result value(id.employee, name.employee, email, date, point, level, status)</returns>
        //Load Data
        private IEnumerable<object> LoadData(int id, string level, string status)
        {
            return db.EMPLOYEEs.Where(x => x.id_employee == id).Select(x => new {
                x.id_employee,
                x.name_employee,
                x.email,
                x.date,
                x.point,
                level,
                status
            });
        }
        //Create MD5 hash
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
        //Client sent mail OTP
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
