//using Admin_Dashboard.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace Admin_Dashboard.Controllers
//{
//    public class CourseController : Controller
//    {
//        // Simulated in-memory DB
//        private static List<Course> _courses = new();

//        public IActionResult Index()
//        {
//            return View(_courses);
//        }

//        public IActionResult Create()
//        {
//            ViewBag.Durations = Enumerable.Range(3, 16).Select(m => $"{m} Months").ToList();
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Create(Course course)
//        {
//            if (ModelState.IsValid)
//            {
//                course.Created_Date = DateTime.Now;
//                if (string.IsNullOrEmpty(course.Course_Id))
//                {
//                    course.Course_Id = $"CSD_{(_courses.Count + 1).ToString("000")}";
//                }
//                _courses.Add(course);
//                return RedirectToAction("Index");
//            }

//            ViewBag.Durations = Enumerable.Range(3, 16).Select(m => $"{m} Months").ToList();
//            return View(course);
//        }

//        public IActionResult Edit(string id)
//        {
//            var course = _courses.FirstOrDefault(c => c.Course_Id == id);
//            ViewBag.Durations = Enumerable.Range(3, 16).Select(m => $"{m} Months").ToList();
//            return View(course);
//        }

//        [HttpPost]
//        public IActionResult Edit(Course course)
//        {
//            var existing = _courses.FirstOrDefault(c => c.Course_Id == course.Course_Id);
//            if (existing != null)
//            {
//                existing.Course_Name = course.Course_Name;
//                existing.Duration_ = course.Duration_;
//                existing.Course_Fee = course.Course_Fee;
//                existing.Course_Details = course.Course_Details;
//            }
//            return RedirectToAction("Index");
//        }

//        public IActionResult Delete(string id)
//        {
//            var course = _courses.FirstOrDefault(c => c.Course_Id == id);
//            return View(course);
//        }

//        [HttpPost]
//        public IActionResult DeleteConfirmed(string Course_Id)
//        {
//            var course = _courses.FirstOrDefault(c => c.Course_Id == Course_Id);
//            if (course != null) _courses.Remove(course);
//            return RedirectToAction("Index");
//        }


//        public IActionResult Details(string id)
//        {
//            var course = _courses.FirstOrDefault(c => c.Course_Id == id);
//            return View(course);
//        }
//    }
//}
using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Admin_Dashboard.Controllers
{
    public class CourseController : Controller
    {
        private readonly IConfiguration _configuration;

        public CourseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public IActionResult Index()
        //{
        //    List<Course> courses = new List<Course>();
        //    using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM Course", conn);
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            courses.Add(new Course
        //            {
        //                Course_Id = reader["Course_Id"].ToString(),
        //                Course_Name = reader["Course_Name"].ToString(),
        //                Duration_ = reader["Duration_"].ToString(),
        //                Course_Fee = Convert.ToDecimal(reader["Course_Fee"]),
        //                Course_Details = reader["Course_Details"].ToString(),
        //                Created_Date = Convert.ToDateTime(reader["Created_Date"])
        //            });
        //        }
        //    }
        //    return View(courses);
        //}
        public IActionResult Index()
        {
            List<Course> courses = new List<Course>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Course", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            Course_Id = reader["Course_Id"].ToString(),
                            Course_Name = reader["Course_Name"].ToString(),
                            Duration_ = reader["Duration_"].ToString(),
                            Course_Fee = Convert.ToDecimal(reader["Course_Fee"]),
                            Course_Details = reader["Course_Details"].ToString(),
                            Created_Date = Convert.ToDateTime(reader["Created_Date"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "DB Error: " + ex.Message;
            }

            return View(courses);
        }

        public IActionResult Create()
        {
            ViewBag.Durations = Enumerable.Range(3, 16).Select(m => $"{m} Months").ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    // Auto-generate Course_Id like CSD_001
                    SqlCommand getMaxIdCmd = new SqlCommand("SELECT MAX(CAST(SUBSTRING(Course_Id, 5, LEN(Course_Id)-4) AS INT)) FROM Course", conn);
                    var maxId = getMaxIdCmd.ExecuteScalar() as int? ?? 0;
                    course.Course_Id = $"CSD_{(maxId + 1).ToString("000")}";

                    SqlCommand cmd = new SqlCommand("INSERT INTO Course (Course_Id, Course_Name, Duration_, Course_Fee, Course_Details, Created_Date) VALUES (@id, @name, @dur, @fee, @details, @date)", conn);
                    cmd.Parameters.AddWithValue("@id", course.Course_Id);
                    cmd.Parameters.AddWithValue("@name", course.Course_Name);
                    cmd.Parameters.AddWithValue("@dur", course.Duration_);
                    cmd.Parameters.AddWithValue("@fee", course.Course_Fee);
                    cmd.Parameters.AddWithValue("@details", course.Course_Details ?? "");
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            ViewBag.Durations = Enumerable.Range(3, 16).Select(m => $"{m} Months").ToList();
            return View(course);
        }

        public IActionResult Edit(string id)
        {
            Course course = new Course();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Course WHERE Course_Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    course.Course_Id = reader["Course_Id"].ToString();
                    course.Course_Name = reader["Course_Name"].ToString();
                    course.Duration_ = reader["Duration_"].ToString();
                    course.Course_Fee = Convert.ToDecimal(reader["Course_Fee"]);
                    course.Course_Details = reader["Course_Details"].ToString();
                }
            }
            ViewBag.Durations = Enumerable.Range(3, 16).Select(m => $"{m} Months").ToList();
            return View(course);
        }

        [HttpPost]
        public IActionResult Edit(Course course)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Course SET Course_Name = @name, Duration_ = @dur, Course_Fee = @fee, Course_Details = @details WHERE Course_Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", course.Course_Id);
                cmd.Parameters.AddWithValue("@name", course.Course_Name);
                cmd.Parameters.AddWithValue("@dur", course.Duration_);
                cmd.Parameters.AddWithValue("@fee", course.Course_Fee);
                cmd.Parameters.AddWithValue("@details", course.Course_Details ?? "");
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            Course course = new Course();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Course WHERE Course_Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    course.Course_Id = reader["Course_Id"].ToString();
                    course.Course_Name = reader["Course_Name"].ToString();
                    course.Duration_ = reader["Duration_"].ToString();
                    course.Course_Fee = Convert.ToDecimal(reader["Course_Fee"]);
                    course.Course_Details = reader["Course_Details"].ToString();
                }
            }
            return View(course);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(string Course_Id)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Course WHERE Course_Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", Course_Id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Details(string id)
        {
            Course course = new Course();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Course WHERE Course_Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    course.Course_Id = reader["Course_Id"].ToString();
                    course.Course_Name = reader["Course_Name"].ToString();
                    course.Duration_ = reader["Duration_"].ToString();
                    course.Course_Fee = Convert.ToDecimal(reader["Course_Fee"]);
                    course.Course_Details = reader["Course_Details"].ToString();
                    course.Created_Date = Convert.ToDateTime(reader["Created_Date"]);
                }
            }
            return View(course);
        }
    }
}
