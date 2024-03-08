using System.Security.Claims;
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
        private readonly ILogger<StudentsController> _logger;
        public StudentsController(Comp1640Context context,
            IWebHostEnvironment webHostEnvironment,
            ILogger<StudentsController> logger)
        {
            _logger = logger;
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
            var annualMagazines = _context.AnnualMagazines.ToList();
            ViewBag.annualMagazines = annualMagazines;
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
        public async Task<ActionResult> Create([Bind("UserId, Title, SubmissionDate")] Contribution contribution, FileDetail fileDetail)
        {
            // Kiểm tra xác thực
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra file không hợp lệ
            string fileExtension = Path.GetExtension(fileDetail.ContributionFile.FileName).ToLowerInvariant();
            if (fileExtension != ".docx" &&
                fileExtension != ".jpg" &&
                fileExtension != ".jpeg" &&
                fileExtension != ".png" &&
                fileExtension != ".webp")
            {
                return BadRequest("Invalid file format");
            }

            // Tạo đường dẫn và ghi file
            string uniqueFileName = GetUniqueFileName(fileDetail.ContributionFile.FileName);
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileDetail.ContributionFile.CopyToAsync(fileStream);
            }

            // Gán thông tin file và loại file
            fileDetail.FilePath = uniqueFileName;
            if (fileExtension == ".docx")
            {
                fileDetail.Type = "Document";
            }
            else
            {
                fileDetail.Type = "Image";
            }

            // Tạo ID mới cho contribution và fileDetail
            int maxContributionId = await _context.Contributions.MaxAsync(c => (int?)c.ContributionId) ?? 0;
            contribution.ContributionId = maxContributionId + 1;
            fileDetail.ContributionId = contribution.ContributionId;
            int maxFileId = await _context.FileDetails.MaxAsync(f => (int?)f.FileId) ?? 0;
            fileDetail.FileId = maxFileId + 1;

            // Thêm contribution và fileDetail vào cơ sở dữ liệu trong một giao dịch
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Add(contribution);
                    _context.Add(fileDetail);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

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