using System.Security.Claims;
using COMP1640.Areas.Identity.Data;
using COMP1640.Models;
using Microsoft.AspNetCore.Identity;
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

        private readonly UserManager<COMP1640User> _userManager;

        public StudentsController(Comp1640Context context,
            IWebHostEnvironment webHostEnvironment,
            ILogger<StudentsController> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<COMP1640User> userManager
           )
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
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
        public async Task<IActionResult> MyAccount()
        {
            ViewData["Title"] = "My Account";
            var contributions = _context.Contributions.ToList();
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var userFullName = user.FullName;
            var userAddress = user.Address;
            var facultyName = await _context.Faculties.FirstOrDefaultAsync(f => f.FacultyId == user.FacultyId);
            var userFaculty = facultyName != null ? facultyName.Name : null;
            var userEmail = user.Email;
            var userProfileImagePath = user.ProfileImagePath;

            ViewBag.userEmail = userEmail;
            ViewBag.contributions = contributions;
            ViewBag.userFaculty = userFaculty;
            ViewBag.userId = userId;
            ViewBag.userFullName = userFullName;
            ViewBag.userAddress = userAddress;
            ViewBag.userProfileImagePath = userProfileImagePath;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(string id, [Bind("UserName, Address, Email")] COMP1640User user)
        {
            Console.WriteLine(id);
            if (user != null)
            {
                string uniqueFileName = GetUniqueFileName(user.ProfileImage.FileName);
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await user.ProfileImage.CopyToAsync(fileStream);
                }
                user.ProfileImagePath = uniqueFileName;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }else{
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }


        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }

        private COMP1640User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<COMP1640User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(COMP1640User)}'. " +
                    $"Ensure that '{nameof(COMP1640User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}