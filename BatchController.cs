using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Admin_Dashboard.Controllers
{
    public class BatchController : Controller
    {
        private readonly IConfiguration _configuration;

        public BatchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Batch> batches = new List<Batch>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Sp_Batch_CRUD", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "SELECTALL");
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    batches.Add(new Batch
                    {
                        Batch_Id = Convert.ToInt32(reader["Batch_Id"]),
                        Batch_Name = reader["Batch_Name"].ToString(),
                        Course_Id = reader["Course_Id"].ToString(),
                        Course_Name = reader["Course_Name"].ToString(),
                        Student_Id = reader["Student_Id"].ToString(),
                        Student_Name = reader["Student_Name"].ToString(),
                        Batch_Description = reader["Batch_Description"].ToString(),
                        Class_Start_Time = Convert.ToDateTime(reader["Class_Start_Time"]),
                        Class_Duration = (TimeSpan)reader["Class_Duration"],
                        Faculty_Name = reader["Faculty_Name"].ToString(),
                        Created_Date = Convert.ToDateTime(reader["Created_Date"])
                    });
                }
            }

            return View("Index", batches);
        }

        public IActionResult Create()
        {
            ViewBag.Courses = GetCourses();
            ViewBag.Students = GetStudents();
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Batch batch)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Sp_Batch_CRUD", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@Batch_Name", batch.Batch_Name);
                    cmd.Parameters.AddWithValue("@Course_Id", batch.Course_Id);
                    cmd.Parameters.AddWithValue("@Course_Name", batch.Course_Name);
                    cmd.Parameters.AddWithValue("@Student_Id", batch.Student_Id);
                    cmd.Parameters.AddWithValue("@Student_Name", batch.Student_Name);
                    cmd.Parameters.AddWithValue("@Batch_Description", batch.Batch_Description);
                    cmd.Parameters.AddWithValue("@Class_Start_Time", batch.Class_Start_Time);
                    cmd.Parameters.AddWithValue("@Class_Duration", batch.Class_Duration);
                    cmd.Parameters.AddWithValue("@Faculty_Name", batch.Faculty_Name);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            ViewBag.Courses = GetCourses();
            ViewBag.Students = GetStudents();
            return View("Create", batch);
        }

        public IActionResult Edit(int id)
        {
            Batch batch = GetBatchById(id);
            ViewBag.Courses = GetCourses();
            ViewBag.Students = GetStudents();
            return View("Edit", batch);
        }

        [HttpPost]
        public IActionResult Edit(Batch batch)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Sp_Batch_CRUD", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@Batch_Id", batch.Batch_Id);
                cmd.Parameters.AddWithValue("@Batch_Name", batch.Batch_Name);
                cmd.Parameters.AddWithValue("@Course_Id", batch.Course_Id);
                cmd.Parameters.AddWithValue("@Course_Name", batch.Course_Name);
                cmd.Parameters.AddWithValue("@Student_Id", batch.Student_Id);
                cmd.Parameters.AddWithValue("@Student_Name", batch.Student_Name);
                cmd.Parameters.AddWithValue("@Batch_Description", batch.Batch_Description);
                cmd.Parameters.AddWithValue("@Class_Start_Time", batch.Class_Start_Time);
                cmd.Parameters.AddWithValue("@Class_Duration", batch.Class_Duration);
                cmd.Parameters.AddWithValue("@Faculty_Name", batch.Faculty_Name);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Batch batch = GetBatchById(id);
            return View("Delete", batch);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Sp_Batch_CRUD", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@Batch_Id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            Batch batch = GetBatchById(id);
            return View("Details", batch);
        }

        private Batch GetBatchById(int id)
        {
            Batch batch = new Batch();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Sp_Batch_CRUD", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "SELECT");
                cmd.Parameters.AddWithValue("@Batch_Id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    batch.Batch_Id = Convert.ToInt32(reader["Batch_Id"]);
                    batch.Batch_Name = reader["Batch_Name"].ToString();
                    batch.Course_Id = reader["Course_Id"].ToString();
                    batch.Course_Name = reader["Course_Name"].ToString();
                    batch.Student_Id = reader["Student_Id"].ToString();
                    batch.Student_Name = reader["Student_Name"].ToString();
                    batch.Batch_Description = reader["Batch_Description"].ToString();
                    batch.Class_Start_Time = Convert.ToDateTime(reader["Class_Start_Time"]);
                    batch.Class_Duration = (TimeSpan)reader["Class_Duration"];
                    batch.Faculty_Name = reader["Faculty_Name"].ToString();
                    batch.Created_Date = Convert.ToDateTime(reader["Created_Date"]);
                }
            }
            return batch;
        }

        private List<SelectListItem> GetCourses()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Course_Id, Course_Name FROM Course", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["Course_Id"].ToString(),
                        Text = reader["Course_Name"].ToString()
                    });
                }
            }
            return list;
        }

        private List<SelectListItem> GetStudents()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Student_Id, Student_Name FROM Student", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["Student_Id"].ToString(),
                        Text = reader["Student_Name"].ToString()
                    });
                }
            }
            return list;
        }
    }
}
