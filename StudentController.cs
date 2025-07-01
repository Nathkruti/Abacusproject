//using Microsoft.AspNetCore.Mvc;
//using Admin_Dashboard.Models;
//using System.Data.SqlClient;
//using System.Data;
//using Microsoft.Extensions.Configuration;
//using Admin_Dashboard.Data;

//namespace Admin_Dashboard.Controllers
//{
//    public class StudentController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public StudentController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public IActionResult Index()
//        {
//            var students = _context.Students.ToList();
//            return View(students);
//        }

//        public IActionResult Create()
//        {
//            ViewBag.Courses = _context.Courses.ToList();
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Create(Student student)
//        {
//            if (ModelState.IsValid)
//            {
//                // Generate Student_Id like: John_001
//                var prefix = student.Student_Name.Split(' ')[0];
//                var count = _context.Students.Count() + 1;
//                student.Student_Id = $"{prefix}_{count.ToString("000")}";

//                // Get Course Name based on selected Course_Id
//                var selectedCourse = _context.Courses.FirstOrDefault(c => c.Course_Id == student.Course_Id);
//                student.Course_Name = selectedCourse?.Course_Name ?? "";
//                student.Join_Date = DateTime.Now;

//                _context.Students.Add(student);
//                _context.SaveChanges();

//                return RedirectToAction("Index");
//            }

//            ViewBag.Courses = _context.Courses.ToList();
//            return View(student);
//        }

//        public IActionResult Edit(string id)
//        {
//            var student = _context.Students.FirstOrDefault(s => s.Student_Id == id);
//            if (student == null)
//                return NotFound();

//            ViewBag.Courses = _context.Courses.ToList();
//            return View(student);
//        }

//        [HttpPost]
//        public IActionResult Edit(Student student)
//        {
//            if (ModelState.IsValid)
//            {
//                var existing = _context.Students.FirstOrDefault(s => s.Student_Id == student.Student_Id);
//                if (existing != null)
//                {
//                    existing.Student_Name = student.Student_Name;
//                    existing.Email_Id = student.Email_Id;
//                    existing.Mob_No = student.Mob_No;
//                    existing.Course_Id = student.Course_Id;

//                    // Update Course_Name
//                    var selectedCourse = _context.Courses.FirstOrDefault(c => c.Course_Id == student.Course_Id);
//                    existing.Course_Name = selectedCourse?.Course_Name ?? "";

//                    _context.SaveChanges();
//                    return RedirectToAction("Index");
//                }
//            }

//            ViewBag.Courses = _context.Courses.ToList();
//            return View(student);
//        }

//        public IActionResult Delete(string id)
//        {
//            var student = _context.Students.FirstOrDefault(s => s.Student_Id == id);
//            return View(student);
//        }

//        [HttpPost]
//        public IActionResult DeleteConfirmed(string Student_Id)
//        {
//            var student = _context.Students.FirstOrDefault(s => s.Student_Id == Student_Id);
//            if (student != null)
//            {
//                _context.Students.Remove(student);
//                _context.SaveChanges();
//            }
//            return RedirectToAction("Index");
//        }

//        public IActionResult Details(string id)
//        {
//            var student = _context.Students.FirstOrDefault(s => s.Student_Id == id);
//            return View(student);
//        }

//    }
//}
//using Admin_Dashboard.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Data.SqlClient;
//using System.Data;


//public class StudentController : Controller
//{
//    private readonly IConfiguration _configuration;

//    public StudentController(IConfiguration configuration)
//    {
//        _configuration = configuration;
//    }

//    public IActionResult Index()
//    {
//        List<Student> students = new List<Student>();
//        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//        {
//            conn.Open();
//            SqlCommand cmd = new SqlCommand("SELECT * FROM Student", conn);
//            SqlDataReader reader = cmd.ExecuteReader();
//            while (reader.Read())
//            {
//                students.Add(new Student
//                {
//                    Student_Id = reader["Student_Id"].ToString(),
//                    Student_Name = reader["Student_Name"].ToString(),
//                    Email_Id = reader["Email_Id"].ToString(),
//                    Mob_No = reader["Mob_No"].ToString(),
//                    Course_Name = reader["Course_Name"].ToString()
//                });
//            }
//        }
//        return View(students);
//    }

//    public IActionResult Create()
//    {
//        ViewBag.Courses = GetCourses();
//        return View();
//    }

//    [HttpPost]
//    public IActionResult Create(Student s)
//    {
//        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//        {
//            conn.Open();
//            SqlCommand cmd = new SqlCommand("InsertStudent", conn);
//            cmd.CommandType = CommandType.StoredProcedure;
//            cmd.Parameters.AddWithValue("@Student_Name", s.Student_Name);
//            cmd.Parameters.AddWithValue("@Email_Id", s.Email_Id);
//            cmd.Parameters.AddWithValue("@Mob_No", s.Mob_No);
//            cmd.Parameters.AddWithValue("@Course_Id", s.Course_Id);
//            cmd.ExecuteNonQuery();
//        }
//        return RedirectToAction("Index");
//    }

//    public IActionResult Details(string id)
//    {
//        Student s = new Student();
//        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//        {
//            conn.Open();
//            SqlCommand cmd = new SqlCommand("SELECT * FROM Student WHERE Student_Id = @id", conn);
//            cmd.Parameters.AddWithValue("@id", id);
//            SqlDataReader reader = cmd.ExecuteReader();
//            if (reader.Read())
//            {
//                s.Student_Id = reader["Student_Id"].ToString();
//                s.Student_Name = reader["Student_Name"].ToString();
//                s.Email_Id = reader["Email_Id"].ToString();
//                s.Mob_No = reader["Mob_No"].ToString();
//                s.Course_Name = reader["Course_Name"].ToString();
//            }
//        }
//        return View(s);
//    }

//    public IActionResult Edit(string id)
//    {
//        Student s = new Student();
//        ViewBag.Courses = GetCourses();
//        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//        {
//            conn.Open();
//            SqlCommand cmd = new SqlCommand("SELECT * FROM Student WHERE Student_Id = @id", conn);
//            cmd.Parameters.AddWithValue("@id", id);
//            SqlDataReader reader = cmd.ExecuteReader();
//            if (reader.Read())
//            {
//                s.Student_Id = reader["Student_Id"].ToString();
//                s.Student_Name = reader["Student_Name"].ToString();
//                s.Email_Id = reader["Email_Id"].ToString();
//                s.Mob_No = reader["Mob_No"].ToString();
//                s.Course_Id = reader["Course_Id"].ToString();
//            }
//        }
//        return View(s);
//    }

//    [HttpPost]
//    public IActionResult Edit(Student s)
//    {
//        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//        {
//            conn.Open();
//            SqlCommand cmd = new SqlCommand("UPDATE Student SET Student_Name = @name, Email_Id = @mail, Mob_No = @mob, Course_Id = @cid WHERE Student_Id = @id", conn);
//            cmd.Parameters.AddWithValue("@name", s.Student_Name);
//            cmd.Parameters.AddWithValue("@mail", s.Email_Id);
//            cmd.Parameters.AddWithValue("@mob", s.Mob_No);
//            cmd.Parameters.AddWithValue("@cid", s.Course_Id);
//            cmd.Parameters.AddWithValue("@id", s.Student_Id);
//            cmd.ExecuteNonQuery();
//        }
//        return RedirectToAction("Index");
//    }

//    public IActionResult Delete(string id)
//    {
//        Student s = new Student();
//        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//        {
//            conn.Open();
//            SqlCommand cmd = new SqlCommand("SELECT * FROM Student WHERE Student_Id = @id", conn);
//            cmd.Parameters.AddWithValue("@id", id);
//            SqlDataReader reader = cmd.ExecuteReader();
//            if (reader.Read())
//            {
//                s.Student_Id = reader["Student_Id"].ToString();
//                s.Student_Name = reader["Student_Name"].ToString();
//                s.Email_Id = reader["Email_Id"].ToString();
//                s.Mob_No = reader["Mob_No"].ToString();
//                s.Course_Name = reader["Course_Name"].ToString();
//            }
//        }
//        return View(s);
//    }

//    [HttpPost, ActionName("Delete")]
//    public IActionResult DeleteConfirmed(string id)
//    {
//        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//        {
//            conn.Open();
//            SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE Student_Id = @id", conn);
//            cmd.Parameters.AddWithValue("@id", id);
//            cmd.ExecuteNonQuery();
//        }
//        return RedirectToAction("Index");
//    }

//    private List<SelectListItem> GetCourses()
//    {
//        List<SelectListItem> courses = new List<SelectListItem>();
//        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//        {
//            conn.Open();
//            SqlCommand cmd = new SqlCommand("SELECT Course_Id, Course_Name FROM Course", conn);
//            SqlDataReader reader = cmd.ExecuteReader();
//            while (reader.Read())
//            {
//                courses.Add(new SelectListItem
//                {
//                    Value = reader["Course_Id"].ToString(),
//                    Text = reader["Course_Name"].ToString()
//                });
//            }
//        }
//        return courses;
//    }
//}
using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

public class StudentController : Controller
{
    private readonly IConfiguration _configuration;

    public StudentController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        List<Student> students = new List<Student>();
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT s.Student_Id, s.Student_Name, s.Email_Id, s.Mob_No, c.Course_Name FROM Student s JOIN Course c ON s.Course_Id = c.Course_Id", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new Student
                {
                    Student_Id = reader["Student_Id"].ToString(),
                    Student_Name = reader["Student_Name"].ToString(),
                    Email_Id = reader["Email_Id"].ToString(),
                    Mob_No = reader["Mob_No"].ToString(),
                    Course_Name = reader["Course_Name"].ToString()
                });
            }
        }
        return View(students); // View: Index.cshtml
    }

    public IActionResult Create()
    {
        ViewBag.Courses = GetCourses();
        return View();
    }

    [HttpPost]
    public IActionResult Create(Student s)
    {
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("InsertStudent", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Student_Name", s.Student_Name);
            cmd.Parameters.AddWithValue("@Email_Id", s.Email_Id);
            cmd.Parameters.AddWithValue("@Mob_No", s.Mob_No);
            cmd.Parameters.AddWithValue("@Course_Id", s.Course_Id);
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }

    public IActionResult Details(string id)
    {
        Student s = GetStudentById(id);
        return View(s);
    }

    public IActionResult Edit(string id)
    {
        ViewBag.Courses = GetCourses();
        Student s = GetStudentById(id);
        return View(s);
    }

    [HttpPost]
    public IActionResult Edit(Student s)
    {
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE Student SET Student_Name = @name, Email_Id = @mail, Mob_No = @mob, Course_Id = @cid WHERE Student_Id = @id", conn);
            cmd.Parameters.AddWithValue("@name", s.Student_Name);
            cmd.Parameters.AddWithValue("@mail", s.Email_Id);
            cmd.Parameters.AddWithValue("@mob", s.Mob_No);
            cmd.Parameters.AddWithValue("@cid", s.Course_Id);
            cmd.Parameters.AddWithValue("@id", s.Student_Id);
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }

    public IActionResult Delete(string id)
    {
        Student s = GetStudentById(id);
        return View(s);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(string id)
    {
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE Student_Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }

    private Student GetStudentById(string id)
    {
        Student s = new Student();
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT s.Student_Id, s.Student_Name, s.Email_Id, s.Mob_No, c.Course_Name, s.Course_Id FROM Student s JOIN Course c ON s.Course_Id = c.Course_Id WHERE s.Student_Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                s.Student_Id = reader["Student_Id"].ToString();
                s.Student_Name = reader["Student_Name"].ToString();
                s.Email_Id = reader["Email_Id"].ToString();
                s.Mob_No = reader["Mob_No"].ToString();
                s.Course_Name = reader["Course_Name"].ToString();
                s.Course_Id = reader["Course_Id"].ToString();
            }
        }
        return s;
    }

    private List<SelectListItem> GetCourses()
    {
        List<SelectListItem> courses = new List<SelectListItem>();
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT Course_Id, Course_Name FROM Course", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                courses.Add(new SelectListItem
                {
                    Value = reader["Course_Id"].ToString(),
                    Text = reader["Course_Name"].ToString()
                });
            }
        }
        return courses;
    }
}
