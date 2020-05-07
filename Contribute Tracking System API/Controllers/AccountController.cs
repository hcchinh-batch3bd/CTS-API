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
using System.IO;
using System.Web;

namespace Contribute_Tracking_System_API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class AccountController : ApiController
    {
        const string badrequest = "Bad request";
        private APIDataClassesDataContext db = new APIDataClassesDataContext();
        /// <summary>This method check access login.</summary>
        /// <param name="id">id of account.</param>
        /// <param name="pw">password of account.</param>
        /// <returns>a json containing result value (name_employee, point, level_employee, apiKey) for id + pw specified, status query, message string/returns>
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
                var key = db.EMPLOYEEs.Where(x => x.id_employee == int.Parse(id) && x.password == Encrypt(pw) && x.status == true).Select(s => new { s.id_employee, s.name_employee, s.point, s.level_employee, s.apiKey });
                if (!key.Any())
                    _message = "ID đăng nhập hoặc mật khẩu không hợp lệ !!";
                return Ok(new { results = key, status = _status, message = _message });
            }
            else return Ok(new { results = "", status = _status, message = _message });
        }
        [Route("Account/")]
        [HttpGet]
        public IHttpActionResult GetInfo([FromUri] string apiKey)
        {
            var employee = db.EMPLOYEEs.Where(s => s.apiKey == apiKey).Select(s => new { s.id_employee, s.name_employee, s.point, s.level_employee }).FirstOrDefault();
            var totalProcess = db.MISSION_PROCESSes.Where(s => s.id_employee == employee.id_employee && s.status == 0 && DateTime.Compare(new DateTime(s.MISSION.Stardate.Year, s.MISSION.Stardate.Month, s.MISSION.Stardate.Day).AddDays(s.MISSION.exprie), DateTime.Now)>0).Count();
            var totalComplete = db.MISSION_PROCESSes.Where(s => s.id_employee == employee.id_employee && s.status == 1).Count();
            return Ok(new
            {
                id_employee = employee.id_employee,
                name_employee = employee.name_employee,
                level = employee.level_employee,
                point = employee.point,
                totalProcess = totalProcess,
                totalComplete = totalComplete
            });
        }
        [Route("Account/OTP")]
        [HttpGet]
        public IHttpActionResult SendOTP([FromUri] string OTP, [FromUri] string mail)
        {
            string _message = "Bad request";
            int _code = 0;
            string _secretcode = "";
            if (mail != null)
            {
                var checkmail = db.EMPLOYEEs.Where(x => x.email == mail).Count();
                if (checkmail != 0)
                {
                    string otp = Decrypt(OTP);
                    string msg = "OTP để xác nhận lấy lại mật khẩu của bạn : " + otp;
                    bool f = SendOTP(mail, "Xác thực mail OTP", msg);
                    if (f)
                    {
                        _message = "Đã gửi OTP đến mail !!";
                        _code = 1;
                        var secretcode = db.EMPLOYEEs.Where(x => x.email == mail).Select(x => x.apiKey).SingleOrDefault();
                        _secretcode = Encrypt(secretcode);
                    }
                    else
                    {
                        _message = "Gửi OTP thất bại, thử lại sau !! !!";
                        _code = 0;
                    }
                }
                else
                {
                    _message = "Không tìm thấy tài khoản nào với email này !!!";
                    _code = -1;
                }

            }
            return Ok(new { message = _message, code = _code, secretcode = _secretcode });

        }
        private const string mysecurityKey = "CTSOTP12";
        [Route("Account/Changepassword")]
        [HttpPut]
        public IHttpActionResult ChangePassword([FromUri] string passold, [FromUri] string passnew, [FromUri] string apiKey)
        {
            string _message = "";
            bool _status = false;
            if (passold != null && passnew != null && apiKey != null)
            {
                _message = "Bạn không có quyền đổi mật khẩu tài khoản này !!";
                var changpass = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status == true && x.password == Encrypt(passold)).Select(x => x).SingleOrDefault();
                if (changpass != null)
                {
                    changpass.password = Encrypt(passnew);
                    changpass.apiKey = RandomAPI(10, false);
                    db.SubmitChanges();
                    _message = " Đổi mật khẩu thành công !!";
                    _status = true;
                }
                else
                    _message = "Mật khẩu cũ không đúng !!";
            }
            else _message = "Bạn chưa nhập dầy đủ thông tin !!";
            return Ok(new { message = _message, status= _status });
        }
        ///
        [Route("Account/ChangepasswordOTP")]
        [HttpPut]
        public IHttpActionResult ChangePasswordWithOTP([FromUri] string passnew, [FromUri] string apiKey)
        {
            string _message = "";
            if (passnew != null && apiKey != null)
            {
                _message = "Bạn không có quyền đổi mật khẩu tài khoản này !!";
                var changpass = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status == true).Select(x => x).SingleOrDefault();
                if (changpass != null)
                {
                    changpass.password = Encrypt(passnew);
                    changpass.apiKey = RandomAPI(10, false);
                    db.SubmitChanges();
                    _message = " Đổi mật khẩu thành công !!";
                }
                else
                    _message = "Tài khoản không hợp lệ !!";
            }
            else _message = "Bạn chưa nhập dầy đủ thông tin !!";
            return Ok(new { message = _message });
        }
        [Route("Account/{id}/DeleteEmployee")]
        [HttpPut]
        public IHttpActionResult DeleteEmployee(int id, [FromUri] string apiKey)
        {
            string _message = "Bad request";
            if (apiKey != null)
            {
                bool CheckApi = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status == true).Select(x => x.level_employee).FirstOrDefault();
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
                        }else
                        _message = "Tài khoản này đã bị xoá từ trước";
                        
                    }
                }
                else _message = "Không có quyền xóa";
            }
            return Ok(new { message = _message });
        }
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
        [Route("Account/ListEmployee")]
        [HttpGet]
        public IHttpActionResult GetListEmployee([FromUri] string apiKey)
        {
            List<object> key = new List<object>();
            string _message = "Bad request!!";
            bool _status = true;
            if (apiKey != null)
            {
                int checkApiKey = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status == true).Select(x => x.apiKey).Count();
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
        private IEnumerable<object> LoadData(int id, string level, string status)
        {
            return db.EMPLOYEEs.Where(x => x.id_employee == id).Select(x => new
            {
                x.id_employee,
                x.name_employee,
                x.email,
                x.date,
                x.point,
                level,
                status
            });
        }
        [Route("Employee/Create")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] EMPLOYEE employee, [FromUri] string apiKey)
        {
            if (employee != null && apiKey != null)
            {
                var check = db.EMPLOYEEs.Where(x => x.apiKey == apiKey && x.status == true).Select(x => x.level_employee).FirstOrDefault();
                if (check)
                {
                    employee.password = Encrypt(employee.password);
                    employee.apiKey = RandomAPI(20, false);
                    employee.status = true;
                    var checkmail = db.EMPLOYEEs.Where(x => x.email == employee.email).Count();
                    if (checkmail == 0)
                    {
                        db.EMPLOYEEs.InsertOnSubmit(employee);
                        db.SubmitChanges();
                        return Ok(new { message = "Thêm nhân viên thành công!", status = true });
                    }
                    else
                        return Ok(new { message = "Email này đã tồn tại !!!", status = false });
                }
                else
                {
                    return Ok(new { message = "Không có quyền thêm!", status = false });
                }
            }
            else
            {
                return Ok(new { message = "Vui lòng nhập thông tin" });
            }
        }
        public static string Encrypt(string TextToEncrypt)
        {
            byte[] MyEncryptedArray = UTF8Encoding.UTF8
               .GetBytes(TextToEncrypt);

            MD5CryptoServiceProvider MyMD5CryptoService = new
               MD5CryptoServiceProvider();

            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash
               (UTF8Encoding.UTF8.GetBytes(mysecurityKey));

            MyMD5CryptoService.Clear();

            var MyTripleDESCryptoService = new
               TripleDESCryptoServiceProvider();

            MyTripleDESCryptoService.Key = MysecurityKeyArray;

            MyTripleDESCryptoService.Mode = CipherMode.ECB;

            MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var MyCrytpoTransform = MyTripleDESCryptoService
               .CreateEncryptor();

            byte[] MyresultArray = MyCrytpoTransform
               .TransformFinalBlock(MyEncryptedArray, 0,
               MyEncryptedArray.Length);

            MyTripleDESCryptoService.Clear();

            return Convert.ToBase64String(MyresultArray, 0,
               MyresultArray.Length);
        }
        public static string Decrypt(string TextToDecrypt)
        {
            TextToDecrypt = TextToDecrypt.Replace(" ", "+");
            byte[] MyDecryptArray = Convert.FromBase64String
               (TextToDecrypt);
            MD5CryptoServiceProvider MyMD5CryptoService = new
               MD5CryptoServiceProvider();
            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash
               (UTF8Encoding.UTF8.GetBytes(mysecurityKey));
            MyMD5CryptoService.Clear();
            var MyTripleDESCryptoService = new
               TripleDESCryptoServiceProvider();
            MyTripleDESCryptoService.Key = MysecurityKeyArray;
            MyTripleDESCryptoService.Mode = CipherMode.ECB;
            MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var MyCrytpoTransform = MyTripleDESCryptoService
               .CreateDecryptor();
            byte[] MyresultArray = MyCrytpoTransform
               .TransformFinalBlock(MyDecryptArray, 0,
               MyDecryptArray.Length);
            MyTripleDESCryptoService.Clear();

            return UTF8Encoding.UTF8.GetString(MyresultArray);
        }
        public bool SendOTP(string to, string subject, string body)
        {
            bool flags = false;
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                SmtpClient client = new SmtpClient();
                client.Send(mailMessage);
                flags = true;
            }
            catch (Exception ex)
            {
                flags = false;
                using (StreamWriter sw = new StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath("~/log.txt"), true))
                {
                    sw.WriteLine(DateTime.Now+" -send maiL: "+ex.Message);
                }
            }
            return flags;
        }
        /// <summary>
        /// Method random string API
        /// </summary>
        /// <param name="size"></param>
        /// <param name="lowerCase"></param>
        /// <returns></returns>
        public string RandomAPI(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}
