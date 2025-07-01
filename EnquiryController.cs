using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Admin_Dashboard.Controllers
{
    public class EnquiryController : Controller
    {
        private readonly IConfiguration _configuration;

        public EnquiryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Enquiry enquiry)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Enquiries (Name, EmailId, MobileNumber, Qualification, InterestedCourse, Status) VALUES (@Name, @Email, @Mobile, @Qual, @Course, 'Pending')", conn);
                cmd.Parameters.AddWithValue("@Name", enquiry.Name);
                cmd.Parameters.AddWithValue("@Email", enquiry.EmailId);
                cmd.Parameters.AddWithValue("@Mobile", enquiry.MobileNumber);
                cmd.Parameters.AddWithValue("@Qual", enquiry.Qualification);
                cmd.Parameters.AddWithValue("@Course", enquiry.InterestedCourse);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("AdminView");
        }

        public IActionResult AdminView(string search = "")
        {
            List<Enquiry> enquiries = new();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                string query = "SELECT * FROM Enquiries WHERE Name LIKE @search OR InterestedCourse LIKE @search";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    enquiries.Add(new Enquiry
                    {
                        EnquiryId = (int)reader["EnquiryId"],
                        Name = reader["Name"].ToString(),
                        EmailId = reader["EmailId"].ToString(),
                        MobileNumber = reader["MobileNumber"].ToString(),
                        Qualification = reader["Qualification"].ToString(),
                        InterestedCourse = reader["InterestedCourse"].ToString(),
                        Status = reader["Status"].ToString(),
                    });
                }
            }
            return View(enquiries);
        }

        public IActionResult UpdateStatus(int id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Enquiries SET Status = @status WHERE EnquiryId = @id", conn);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("AdminView");
        }
    }
}
