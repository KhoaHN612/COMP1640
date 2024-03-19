using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using COMP1640.Areas.Identity.Data;
using COMP1640.Migrations;
using COMP1640.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSenderCustom _emailSender;
        public StudentsController(Comp1640Context context,
            IWebHostEnvironment webHostEnvironment,
            ILogger<StudentsController> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<COMP1640User> userManager,
            RoleManager<IdentityRole> RoleManager,
            IEmailSenderCustom EmailSender
           )
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = RoleManager;
            _emailSender = EmailSender;
        }
        private async Task<string> GetUserFullName()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            return user?.FullName; // This will return null if user is null.
        }

        // GET: StudentsController
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Home Page";
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            return View();
        }
        public async Task<IActionResult> SubmissionList()
        {
            ViewData["Title"] = "Submission List";
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            return View();
        }

        // Action for the About Us page
        public async Task<IActionResult> AboutUs()
        {
            ViewData["Title"] = "About Us";
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            return View();
        }

        // Action for the Contact Us page
        public async Task<IActionResult> ContactUs()
        {
            ViewData["Title"] = "Contact Us";
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            return View();
        }

        // Action for the My Account page
        public async Task<IActionResult> MyAccount()
        {
            ViewData["Title"] = "My Account";
            var contributions = _context.Contributions.ToList();
            var userId = _userManager.GetUserId(User);
            var anotherUserId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            var userFullName = user.FullName;
            var userAddress = user.Address;
            var facultyName = await _context.Faculties.FirstOrDefaultAsync(f => f.FacultyId == user.FacultyId);
            var userFaculty = facultyName != null ? facultyName.Name : null;
            var userEmail = user.Email;
            var userProfileImagePath = user.ProfileImagePath;

            var fileTypes = new Dictionary<int, string>();
            foreach (var contribution in contributions)
            {
                var fileDetail = _context.FileDetails.FirstOrDefault(fd => fd.ContributionId == contribution.ContributionId);
                if (fileDetail != null)
                {
                    fileTypes[contribution.ContributionId] = fileDetail.Type;
                }
                else
                {
                    fileTypes[contribution.ContributionId] = "Unknown";
                }
            }
            ViewBag.FileTypes = fileTypes;
            ViewBag.userEmail = userEmail;
            ViewBag.contributions = contributions;
            ViewBag.userFaculty = userFaculty;
            ViewBag.userId = anotherUserId;
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
            var annualMagazines = _context.AnnualMagazines
                             .Where(m => m.IsActive == true)
                             .ToList();
            ViewBag.annualMagazines = annualMagazines;
            return View("~/Views/managers/student/student_submission.cshtml");
        }
        public async Task<IActionResult> FromEditSubmission(int id)
        {
            ViewData["Title"] = "From Submission";
            var contribution = await _context.Contributions.FirstOrDefaultAsync(c => c.ContributionId == id);
            var academicYear = await _context.Contributions
                .Where(c => c.ContributionId == id)
                .Select(c => c.AnnualMagazine.AcademicYear)
                .FirstOrDefaultAsync();
            if (academicYear != null)
            {
                ViewBag.academicYear = academicYear;
            }   
            return View("~/Views/managers/student/student_edit_submission.cshtml", contribution);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int AnnualMagazineId, [Bind("Title, SubmissionDate")] Contribution contribution, FileDetail fileDetail)
        {

            string uniqueFileName = GetUniqueFileName(fileDetail.ContributionFile.FileName);
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "contributionUpload", uniqueFileName);
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
            contribution.AnnualMagazineId = AnnualMagazineId;

            var userId = _userManager.GetUserId(User);
            contribution.Comment = null;
            contribution.Status = "Pending";
            contribution.UserId = userId;
            fileDetail.ContributionId = contribution.ContributionId;
            _context.Add(contribution);
            _context.Add(fileDetail);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    return NotFound("User not found.");
                }

                // Get the Faculty ID of the current user
                var facultyId = currentUser.FacultyId;

                // Get the role ID for "Coordinator"
                var coordinatorRole = (await _roleManager.FindByNameAsync("Coordinator"));

                if (facultyId != null && coordinatorRole != null)
                {
                    // // Retrieve users with the same Faculty and "Coordinator" role
                    // var coordinators = await _userManager.Users
                    //     .Include(u => u.Faculty) // Eager load the Faculty navigation property
                    //     .Where(u => u.FacultyId == currentUser.FacultyId) // Match faculty ID
                    //     .Where(u => _userManager.IsInRoleAsync(u, coordinatorRole.Name).Result) // Check if user has the "Coordinator" role
                    //     .ToListAsync();

                    var sameFacultyUsers = await _userManager.Users
                        .Include(u => u.Faculty) // Eager load the Faculty navigation property
                        .Where(u => u.FacultyId == currentUser.FacultyId) // Match faculty ID
                        .ToListAsync();

                    // Filter users who have the "Coordinator" role
                    var coordinators = sameFacultyUsers
                        .Where(u => _userManager.IsInRoleAsync(u, coordinatorRole.Name).Result)
                        .ToList();

                    var coordinatorEmails = coordinators.Select(u => u.Email).ToArray();
                    var annualMagazine = await _context.AnnualMagazines.FindAsync(AnnualMagazineId);

                    string body = "Title: New Contribution\n" +
                    "Dear sir/madam, \n" +
                    "There are new contribution(s) for the annual magazine.\n" +
                    "- Name Contribution: " + contribution.Title + "\n" +
                    "- Annual Magazine name:" + annualMagazine.Title + "\n" +
                    "- Academic Year: " + annualMagazine.AcademicYear + "\n\n" +
                    "Sincerely, \n" +
                    "Developer team";
                    var message = new Message(coordinatorEmails, "New Contribution", body);
                    await _emailSender.SendEmailAsync(message);
                }
            }
            return RedirectToAction(nameof(MyAccount));
        }

        // POST: StudentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSubmission(int id, FileDetail newContribution)
        {
            // var contribution = await _context.Contributions.FirstOrDefaultAsync(c => c.ContributionId == id);
            var currentFile = await _context.FileDetails
                .FirstOrDefaultAsync(fd => fd.ContributionId == id);
            if (currentFile != null)
            {
                string uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "contributionUpload");
                string currentFilePath = Path.Combine(uploadsFolderPath, currentFile.FilePath);
                System.IO.File.Delete(currentFilePath);
            }

            string uniqueFileName = GetUniqueFileName(newContribution.ContributionFile.FileName);
            string newFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "contributionUpload", uniqueFileName);
            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                await newContribution.ContributionFile.CopyToAsync(fileStream);
            }

            currentFile.FilePath = uniqueFileName;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        public async Task<ActionResult> UpdateProfile(IFormFile ProfileImageFile, COMP1640User user)
        {
            var userToUpdate = await _context.FindAsync<COMP1640User>(user.Id);
            var profileImageFile = ProfileImageFile;
            if (user.ProfileImageFile == null)
            {
                ModelState.Remove("ProfileImageFile");
            }
            else
            {
                if (userToUpdate.ProfileImagePath != null)
                {
                    string uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "profileImageUpload");
                    string imageFilePath = Path.Combine(uploadsFolderPath, userToUpdate.ProfileImagePath);
                    System.IO.File.Delete(imageFilePath);
                }
                string uniqueFileName = GetUniqueFileName(user.ProfileImageFile.FileName);
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "profileImageUpload", uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await user.ProfileImageFile.CopyToAsync(fileStream);
                }
                user.ProfileImagePath = uniqueFileName;
                userToUpdate.ProfileImagePath = user.ProfileImagePath;
            }
            userToUpdate.FullName = user.FullName;
            userToUpdate.Address = user.Address;
            userToUpdate.Email = user.Email;
            _context.Update(userToUpdate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyAccount));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePassword(string id, string inputOldPassword, string newPassword)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var status = await _userManager.ChangePasswordAsync(user, inputOldPassword, newPassword);
            }

            return RedirectToAction(nameof(MyAccount));
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }

        public async Task<IActionResult> SubmissionDetail(int id)
        {
            ViewData["Title"] = "Submission Detail";
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            var contribution = await _context.Contributions.FindAsync(id);
            var comments = await _context.Comments
                                .Where(c => c.ContributionId == id)
                                .ToListAsync();

            var contributions = _context.Contributions.ToList();
            var userId = _userManager.GetUserId(User);
            var anotherUserId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            var userAddress = user.Address;
            var facultyName = await _context.Faculties.FirstOrDefaultAsync(f => f.FacultyId == user.FacultyId);
            var userFaculty = facultyName != null ? facultyName.Name : null;
            var userEmail = user.Email;
            var userProfileImagePath = user.ProfileImagePath;
            if (contribution != null)
            {
                var submissionDate = contribution.SubmissionDate;
                var deadline = submissionDate.AddDays(14);
                if (deadline >= submissionDate)
                {
                    ViewBag.Deadline = deadline;
                }
            }
            ViewBag.userEmail = userEmail;
            ViewBag.contributions = contributions;
            ViewBag.userFaculty = userFaculty;
            ViewBag.userId = anotherUserId;
            ViewBag.contributionUserId = contribution.UserId;
            ViewBag.contributionsTile = contribution.Title;
            ViewBag.userFullName = userFullName;
            ViewBag.userAddress = userAddress;
            ViewBag.userProfileImagePath = userProfileImagePath;
            ViewBag.Comments = comments;
            return View(contribution);

        }

        public async Task<IActionResult> PostLists()
        {
            ViewData["Title"] = "Post Lists";
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            return View();
        }

        public async Task<IActionResult> PostDetail()
        {
            ViewData["Title"] = "Post Detail";
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComment(Contribution contribution)
        {
            var existingContribution = await _context.Contributions.FindAsync(contribution.ContributionId);
            if (existingContribution != null)
            {
                var user = await _context.Users.FindAsync(existingContribution.UserId);
                var annualMagazine = await _context.AnnualMagazines.FindAsync(existingContribution.AnnualMagazineId);
                var newComment = new Comment
                {
                    UserId = user.Id,
                    ContributionId = contribution.ContributionId,
                    CommentField = contribution.Comment, 
                    CommentDate = DateTime.Now 
                };
                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();
                var comments = await _context.Comments
                    .Where(c => c.ContributionId == contribution.ContributionId)
                    .ToListAsync();
                ViewBag.Comments = comments;
                return RedirectToAction("SubmissionDetail", "Students", new { id = contribution.ContributionId });
            }
            return View(contribution);
        }

    }
}