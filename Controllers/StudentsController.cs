using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using COMP1640.Areas.Identity.Data;
using COMP1640.Migrations;
using COMP1640.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using COMP1640.Models.MultiModels;
using Humanizer;
using Microsoft.VisualBasic;

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

        public async Task<IActionResult> IndexGuest(string task, string year)
        {
            ViewData["Title"] = "Dashboard Guest";

            List<TotalContribution> TotalContribution = new List<TotalContribution>();
            List<TotalContribution> TotalContributionsPushlish = new List<TotalContribution>();
            List<TotalContribution> TotalContributionsRejected = new List<TotalContribution>();
            List<TotalContribution> TotalContributionsPending = new List<TotalContribution>();
            List<ContributionWithoutComment> contributions = new List<ContributionWithoutComment>();
            List<ContributionWithoutComment> contributionWithoutComments = new List<ContributionWithoutComment>();
            List<ContributionWithoutComment> contributionComments = new List<ContributionWithoutComment>();

            //GET ALL YEARS
            List<int> years = await _context.Contributions
                .Select(c => c.SubmissionDate.Year)
                .Distinct()
                .ToListAsync();

            //Total Contributions
            DateTime currentDate = DateTime.Now;
            if (task == "Contributions" && !string.IsNullOrEmpty(year))
            {
                currentDate = new DateTime(Convert.ToInt32(year), 1, 1);
            }

            DateTime date = DateTime.Now;
            if (task == "CommentContributions" && !string.IsNullOrEmpty(year))
            {
                date = new DateTime(Convert.ToInt32(year), 1, 1);
            }

            //get current faculty of current user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.FacultyId == null)
            {
                return null;
            }
            else
            {
                int currentFacultyId = currentUser.FacultyId ?? 0; ;

                TotalContribution = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributions");

                //Total Contributions Puslished
                TotalContributionsPushlish = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributionsPuslished");

                //Total Contributions Rejected
                TotalContributionsRejected = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributionsRejected");

                //Total Contributions Pending
                TotalContributionsPending = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributionsPending");

                //GET ALL CONTRIBUTIONS
                contributions = await GetComments(currentFacultyId, date.Year, "Contribution");

                //GET ALL CONTRIBUTIONS WITHOUT COMMENTS  
                contributionWithoutComments = await GetComments(currentFacultyId, date.Year, "ContributionWithoutComments");

                //GET ALL CONTRIBUTIONS HAVE COMMENT
                contributionComments = await GetComments(currentFacultyId, date.Year, "ContributionComments");
            }

            if (TotalContribution.Count == 0)
            {
                TotalContribution.Add(new TotalContribution { Year = currentDate.Year, Month = currentDate.Month, Total = 0 });
            }

            if (contributions.Count == 0)
            {
                contributions.Add(new ContributionWithoutComment { Date = date, Year = date.Year, Quantity = 0 });
            }

            ViewData["TotalContributionsAccepted"] = TotalContributionsPushlish;
            ViewData["TotalContributionsRejected"] = TotalContributionsRejected;
            ViewData["TotalContributionsPending"] = TotalContributionsPending;
            ViewData["TotalContribution"] = TotalContribution;
            ViewData["ContributionWithoutComments"] = contributionWithoutComments;
            ViewData["ContributionWithoutCommentsAfter14Days"] = contributionComments;
            ViewData["Contributions"] = contributions;
            ViewData["Years"] = years;

            return View("~/Views/managers/student/index.cshtml");
        }

        public async Task<List<ContributionWithoutComment>> GetComments(int facultyID, int year, string actions)
        {
            var query = from c in _context.Contributions
                        join u in _context.Users on c.UserId equals u.Id
                        select new { Contribution = c, User = u };

            if (actions == "ContributionWithoutComments")
            {
                query = query.Where(c => c.Contribution.Comment == null);
            }
            else if (actions == "ContributionComments")
            {
                query = query.Where(c => c.Contribution.Comment != null);
            }

            List<ContributionWithoutComment> contributions = await query
                .Where(c => c.Contribution.SubmissionDate.Year == year
                            && c.Contribution.Status == "Approved"
                            && c.User.FacultyId == facultyID)
                .GroupBy(c => new { Date = c.Contribution.SubmissionDate.Date })
                .Select(g => new ContributionWithoutComment
                {
                    Date = g.Key.Date,
                    Year = g.Key.Date.Year,
                    Quantity = g.Count()
                })
                .OrderBy(c => c.Date)
                .ToListAsync();
            return contributions;
        }

        public async Task<List<TotalContribution>> GetTotalContributions(int facultyID, int year, string action)
        {
            var query = from c in _context.Contributions
                        join u in _context.Users on c.UserId equals u.Id
                        where c.SubmissionDate.Year == year && u.FacultyId == facultyID
                        select new { Contribution = c, User = u };

            switch (action)
            {
                case "TotalContributionsPuslished":
                    query = query.Where(c => c.Contribution.IsPublished == true);
                    break;
                case "TotalContributionsRejected":
                    query = query.Where(c => c.Contribution.Status == "Rejected");
                    break;
                case "TotalContributionsPending":
                    query = query.Where(c => c.Contribution.Status == "Pending");
                    break;
                default:
                    break;
            }

            List<TotalContribution> contributions = await query
            .GroupBy(c => new { c.Contribution.SubmissionDate.Year, c.Contribution.SubmissionDate.Month })
            .Select(g => new TotalContribution
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Total = g.Count()
            })
            .OrderBy(c => c.Year)
            .ThenBy(c => c.Month)
            .ToListAsync();

            return contributions;
        }

        [Authorize]
        public async Task<IActionResult> SubmissionList()
        {
            ViewData["Title"] = "Submission List";
            var publishedContributions = await _context.Contributions
                .Where(c => c.IsPublished)
                .ToListAsync();
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            return View("submissionList", publishedContributions);
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
        [Authorize(Roles = "Student, Guest")]
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
            var fileTypes = new Dictionary<int, List<string>>();
            foreach (var contribution in contributions)
            {
                var fileDetails = _context.FileDetails.Where(fd => fd.ContributionId == contribution.ContributionId).ToList();

                var types = new List<string>();
                if (fileDetails.Count != 0)
                {
                    foreach (var fileDetail in fileDetails)
                    {
                        types.Add(fileDetail.Type);
                    }
                }
                else
                {
                    types.Add("Unknown");
                }
                fileTypes[contribution.ContributionId] = types;
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

        [Authorize(Roles = "Student")]
        public IActionResult FromCreateSubmission()
        {
            ViewData["Title"] = "From Submission";
            var annualMagazines = _context.AnnualMagazines
                            .Where(m => m.IsActive == true)
                            .ToList();
            ViewBag.annualMagazines = annualMagazines;
            return View("~/Views/managers/student/student_submission.cshtml");
        }

        [Authorize(Roles = "Student")]
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
            var uploadedFiles = await _context.FileDetails.Where(f => f.ContributionId == id).ToListAsync();
            return View("~/Views/managers/student/student_edit_submission.cshtml", uploadedFiles);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int AnnualMagazineId, [Bind("Title")] Contribution contribution, FileDetail fileDetails)
        {
            ViewBag.wrongFileMessage = "";
            var currentContributionId = await _context.Contributions.MaxAsync(c => (int?)c.ContributionId) ?? 0;
            contribution.ContributionId = currentContributionId + 1;
            int maxId = 0;
            maxId = await _context.FileDetails.MaxAsync(f => (int?)f.FileId) ?? 0;

            foreach (var file in fileDetails.ContributionFile)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                fileDetails.FileId = maxId + 1;
                maxId++;

                string uniqueFileName = GetUniqueFileName(file.FileName);
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "contributionUpload", uniqueFileName);
                string fileExtension = Path.GetExtension(uniqueFileName).ToLowerInvariant();


                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                fileDetails.FilePath = uniqueFileName;

                var documentExtensions = new List<string> { ".doc", ".docx" };
                var imageExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
                fileDetails.Type = documentExtensions.Any(e => e == fileExtension) ? "Document" :
                   imageExtensions.Any(e => e == fileExtension) ? "Image" : "Unknown";

                fileDetails.ContributionId = contribution.ContributionId;
                _context.Add(fileDetails);
                await _context.SaveChangesAsync();
            }

            maxId = await _context.Contributions.MaxAsync(c => (int?)c.ContributionId) ?? 0;
            contribution.AnnualMagazineId = AnnualMagazineId;

            var userId = _userManager.GetUserId(User);
            contribution.SubmissionDate = DateTime.Now;
            contribution.Comment = null;
            contribution.Status = "Pending";
            contribution.UserId = userId ?? "Unknown";
            contribution.CommentDeadline = contribution.SubmissionDate.AddDays(14).AddHours(23).AddMinutes(59).AddSeconds(59);
            _context.Add(contribution);
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
                var coordinatorRole = await _roleManager.FindByNameAsync("Coordinator");

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
        public async Task<ActionResult> EditSubmission(int id, IFormFile newFile)
        {
            var currentFile = await _context.FileDetails
                .FirstOrDefaultAsync(fd => fd.FileId == id);

            if (currentFile != null)
            {
                string uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "contributionUpload");
                string currentFilePath = Path.Combine(uploadsFolderPath, currentFile.FilePath);
                System.IO.File.Delete(currentFilePath);
            }

            string uniqueFileName = GetUniqueFileName(newFile.FileName);
            string newFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "contributionUpload", uniqueFileName);
            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                await newFile.CopyToAsync(fileStream);
            }

            currentFile.FilePath = uniqueFileName;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Student")]
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

        [Authorize]
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

        [Authorize(Roles = "Student")]
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

            ViewBag.Deadline = contribution.CommentDeadline;
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

        [Authorize(Roles = "Student")]
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

        [Authorize(Roles = "Student")]
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