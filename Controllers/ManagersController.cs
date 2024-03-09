using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using COMP1640.Models;

namespace COMP1640.Controllers
{
    public class ManagersController : Controller
    {
        private readonly Comp1640Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ManagersController(Comp1640Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        //================================ ADMIN ================================//
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
            return View("admins/table_user");
        }
        public IActionResult FormCreateUser()
        {
            ViewData["Title"] = "Create User page";
            return View("admins/form_create_user");
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

        //================================ PROFILES ================================//
        public IActionResult ShowProfile()
        {
            ViewData["Title"] = "Profiles";
            return View("profile_managers");
        }
    }
}