using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Admin_Dashboard.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public IActionResult Register(AdminModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("InsertAdmin", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Admin_Name", model.Admin_Name);
                    cmd.Parameters.AddWithValue("@Email_Id", model.Email_Id);
                    cmd.Parameters.AddWithValue("@Mob_No", model.Mob_No);
                    cmd.Parameters.AddWithValue("@Photo_", model.Photo_ ?? "default.jpg");
                    cmd.Parameters.AddWithValue("@Role_", model.Role_);
                    cmd.Parameters.AddWithValue("@Password_", model.Password_);
                    cmd.Parameters.AddWithValue("@CreatedBy", model.Admin_Name);
                    cmd.Parameters.AddWithValue("@ModifiedBy", model.Admin_Name);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["Msg"] = "Registration Successful!";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public IActionResult Login(AdminModel model)
        {
            AdminModel admin = null;

            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM Admin_ WHERE Email_Id = @Email_Id AND Password_ = @Password_";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email_Id", model.Email_Id);
                cmd.Parameters.AddWithValue("@Password_", model.Password_);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    admin = new AdminModel
                    {
                        Admin_Id = dr["Admin_Id"].ToString(),
                        Admin_Name = dr["Admin_Name"].ToString(),
                        Email_Id = dr["Email_Id"].ToString(),
                        Mob_No = dr["Mob_No"].ToString(),
                        Photo_ = dr["Photo_"].ToString(),
                        Role_ = dr["Role_"].ToString()
                    };

                    HttpContext.Session.SetString("AdminName", admin.Admin_Name);
                    HttpContext.Session.SetString("AdminRole", admin.Role_);
                    HttpContext.Session.SetString("AdminPhoto", admin.Photo_);
                }
            }

            if (admin != null)
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials!";
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewBag.Name = HttpContext.Session.GetString("AdminName");
            ViewBag.Role = HttpContext.Session.GetString("AdminRole");
            ViewBag.Photo = HttpContext.Session.GetString("AdminPhoto");
            return View();
        }
    }
}
