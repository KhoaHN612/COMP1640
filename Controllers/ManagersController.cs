using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using COMP1640.Models;
using Microsoft.AspNetCore.Identity;
using COMP1640.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace COMP1640.Controllers
{
    public class ManagersController : Controller
    {
        private readonly UserManager<COMP1640User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Comp1640Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ManagersController(Comp1640Context context, IWebHostEnvironment webHostEnvironment,
         UserManager<COMP1640User> UserManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = UserManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        //================================ ADMIN ================================//
        public async Task<IActionResult> CreateRole(){
            string[] roleNames = { "Guest", "Manager", "Coordinator", "Student", "Admin" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            return RedirectToAction("TableUser");
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            return View("admins/index");
        }
        public IActionResult TableFaculty()
        {
            ViewData["Title"] = "Faculty Table page";
            return View("admins/table_faculty");
        }
        public IActionResult FormCreateFaculty()
        {
            ViewData["Title"] = "Create Faculty Table page";
            return View("admins/form_create_faculty");
        }
        public IActionResult TableSubmissionDate()
        {
            ViewData["Title"] = "Submission Date Table page";
            return View("admins/table_submission_date");
        }
        public IActionResult FormCreateSubmissionDate()
        {
            ViewData["Title"] = "Create Submission Date page";
            return View("admins/form_create_submission_date");
        }
        public IActionResult TableUser ()
        {
            ViewData["Title"] = "User Table page";
            var users = _userManager.Users.ToList();
            return View("admins/table_user", users);
        }
        public IActionResult FormCreateUser()
        {
            ViewData["Title"] = "Create User page";
            ViewBag.Roles = _roleManager.Roles.ToList();
            var faculties = _context.Faculties.ToList();
            if (faculties != null && faculties.Any())
            {
                ViewBag.FacultyId = new SelectList(faculties, "FacultyId", "Name");
            }
            else
            {
                ViewBag.FacultyId = new SelectList(new List<Faculty>(), "FacultyId", "Name");
            }
            return View("admins/form_create_user");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormCreateUser(COMP1640User model, string Role)
        {
            if (ModelState.IsValid)
            {
                // Create a new IdentityUser instance
                var user = new COMP1640User 
                { 
                    UserName = model.Email, 
                    Email = model.Email,
                    FullName = model.FullName,
                    ProfileImagePath  = model.ProfileImagePath,
                    PhoneNumber = model.PhoneNumber,
                    DayOfBirth = model.DayOfBirth,
                    Address = model.Address,
                    FacultyId = model.FacultyId,
                };

                // Create the user in the database
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    // Find the role by its ID
                    var selectedRole = await _roleManager.FindByIdAsync(Role);

                    if (selectedRole != null)
                    {
                        // Assign the user to the selected role
                        await _userManager.AddToRoleAsync(user, selectedRole.Name);
                    }

                    // Redirect to a success page or return a success message
                    return RedirectToAction("TableUser");
                }
                else
                {
                    // If creation of user fails, add errors to model state
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return RedirectToAction("FormCreateUser");
            // // If ModelState is invalid, return to the create user form with errors
            // ViewData["Title"] = "Create User page";
            // ViewBag.Roles = _roleManager.Roles.ToList();
            // var faculties = _context.Faculties.ToList();
            // if (faculties != null && faculties.Any())
            // {
            //     ViewBag.FacultyId = new SelectList(faculties, "FacultyId", "Name");
            // }
            // else
            // {
            //     ViewBag.FacultyId = new SelectList(new List<Faculty>(), "FacultyId", "Name");
            // }
            // return View("admins/form_create_user", model);
        }
        //================================ COORINATORS ================================//
        public IActionResult IndexCooridinators()
        {
            ViewData["Title"] = "Dashboard Coordinators";
            return View("coordinators/index");
        }
        public IActionResult StudentSubmissionCoordinators()
        {
            List<Contribution> contributions = _context.Contributions.Include(c => c.AnnualMagazine).ToList();
            ViewData["Title"] = "List Student Submission";
            return View("coordinators/student_submission", contributions);
        }
        public IActionResult CoordinatorComment()
        {
            ViewData["Title"] = "Create Comment";
            return View("coordinators/create_comment");
        }
        //================================ MANAGERS ================================//
        public IActionResult IndexManagers()
        {
            ViewData["Title"] = "Dashboard Managers";
            return View("head_managers/index");
        }
        public IActionResult StudentSubmissionManagers()
        {
                List<Contribution> contributions = _context.Contributions
                                                .Include(c => c.AnnualMagazine)
                                                .Where(c => c.Status == "Approved")
                                                .ToList();
            ViewData["Title"] = "List Student Submission";
            return View("head_managers/student_submission", contributions);
        }
        // DOWNLOAD EACH FILES
        [HttpGet]
        public async Task<IActionResult> DownloadContributionFiles(int id)
        {
            var fileDetails = await _context.FileDetails
                .Where(fd => fd.ContributionId == id)
                .ToListAsync();

            if (!fileDetails.Any())
            {
                return NotFound("No files found for this contribution.");
            }

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileDetail in fileDetails)
                {
                    var completeFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileDetail.FilePath);
                    if (!System.IO.File.Exists(completeFilePath))
                    {
                        continue; // Ideally, add some error logging here
                    }

                    var entry = archive.CreateEntry(Path.GetFileName(completeFilePath));
                    using var entryStream = entry.Open();
                    using var fileStream = System.IO.File.OpenRead(completeFilePath);
                    await fileStream.CopyToAsync(entryStream);
                    entryStream.Flush(); // Make sure to flush the stream
                }
            }

            memoryStream.Position = 0; // Reset the memory stream position

            var zipFileName = $"ContributionFiles_{id}.zip";
            return File(memoryStream.ToArray(), "application/zip", zipFileName);
        }
        //DOWNLOAD ALL FILES

        [HttpGet]
        public async Task<IActionResult> DownloadAllApproved()
        {
            var contributions = await _context.Contributions
                .Where(c => c.Status == "Approved")
                .Include(c => c.FileDetails)
                .ToListAsync();

            var memoryStream = new MemoryStream();
            try
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var contribution in contributions)
                    {
                        foreach (var fileDetail in contribution.FileDetails)
                        {
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileDetail.FilePath);

                            if (System.IO.File.Exists(filePath))
                            {
                                var entry = archive.CreateEntry(Path.GetFileName(filePath));

                                using (var entryStream = entry.Open())
                                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                {
                                    await fileStream.CopyToAsync(entryStream);
                                }
                            }
                        }
                    }
                }
                memoryStream.Position = 0;
                return File(memoryStream, "application/zip", "ApprovedFiles.zip");
            }
            catch
            {
                memoryStream.Close();
                throw;
            }
        }
        //================================ PROFILES ================================//
        public IActionResult ShowProfile()
        {
            ViewData["Title"] = "Profiles";
            return View("profile_managers");
        }
    }
}