using System.Security.Claims;
using COMP1640.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Controllers
{
    public class StudentsController : Controller
    {
        private readonly Comp1640Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<StudentsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentsController(Comp1640Context context,
            IWebHostEnvironment webHostEnvironment,
            ILogger<StudentsController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
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
            var contributions = _context.Contributions.ToList();
            ViewBag.contributions = contributions;
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View(userId);
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
            var annualMagazines = _context.AnnualMagazines.ToList();
            ViewBag.annualMagazines = annualMagazines;
            return View("~/Views/managers/student/student_submission.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Title, SubmissionDate")] Contribution contribution, FileDetail fileDetail)
        {

            string uniqueFileName = GetUniqueFileName(fileDetail.ContributionFile.FileName);
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", uniqueFileName);
            string fileExtension = Path.GetExtension(uniqueFileName).ToLowerInvariant();
            if (fileExtension == ".docx")
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileDetail.ContributionFile.CopyToAsync(fileStream);
                }
                fileDetail.FilePath = uniqueFileName;
                fileDetail.Type = "Document";
            }
            else if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".webp")
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileDetail.ContributionFile.CopyToAsync(fileStream);
                }
                fileDetail.FilePath = uniqueFileName;
                fileDetail.Type = "Image";
            }
            else
            {
                return View("Error");
            }
            int maxId = await _context.FileDetails.MaxAsync(f => (int?)f.FileId) ?? 0;
            fileDetail.FileId = maxId + 1;

            maxId = await _context.Contributions.MaxAsync(c => (int?)c.ContributionId) ?? 0;
            contribution.ContributionId = maxId + 1;
            contribution.AnnualMagazineId = 1;

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            contribution.Comment = null;
            contribution.Status = "Pending";
            contribution.UserId = userId;
            fileDetail.ContributionId = contribution.ContributionId;
            _context.Add(contribution);
            _context.Add(fileDetail);
            var result = await _context.SaveChangesAsync();
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