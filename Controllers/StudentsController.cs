using COMP1640.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Controllers
{
    public class StudentsController : Controller
    {
        private int contributionId;
        private readonly Comp1640Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public StudentsController(Comp1640Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: StudentsController
        public ActionResult Index()
        {
            ViewData["Title"] = "Home Page";
            return View();
        }
        public IActionResult SubmissionList()
        {
            ViewData["Title"] = "Submission List";
            return View();
        }

        // Action for the About Us page
        public IActionResult AboutUs()
        {
            ViewData["Title"] = "About Us";
            return View();
        }

        // Action for the Contact Us page
        public IActionResult ContactUs()
        {
            ViewData["Title"] = "Contact Us";
            return View();
        }

        // Action for the My Account page
        public IActionResult MyAccount()
        {
            ViewData["Title"] = "My Account";
            return View();
        }

        // Action for the Login/Register page
        public IActionResult LoginRegister()
        {
            ViewData["Title"] = "Login Or Register";
            return View();
        }

        public IActionResult FromCreateSubmission()
        {
            ViewData["Title"] = "From Submission";
            return View("~/Views/managers/student/student_submission.cshtml");
        }

        //// GET: StudentsController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: StudentsController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: StudentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("UserId, Title, SubmissionDate")] Contribution contribution)
        {
            contribution.ContributionId = 1;
            contribution.AnnualMagazineId = 1;
            contribution.Comment = null;
            contribution.Status = "Pending";
            _context.Add(contribution);
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: StudentsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StudentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: StudentsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StudentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }
    }
}