﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using COMP1640.Models;
using COMP1640.Models.MultiModels;

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
        public async Task<IActionResult> Index(string task, string year)
        {
            Console.WriteLine($"Action: {task}task, Year: {year}");
            ViewData["Title"] = "Dashboard";

            // GET CONTRIBUTION BY FACULTY
            List<ContributionFaculty> contributionFaculty = new List<ContributionFaculty>();
            DateTime currentDate = DateTime.Now; 

            if (task == "ContributionFaculty" && !string.IsNullOrEmpty(year)) { currentDate = new DateTime(Convert.ToInt32(year), 1, 1); }
            var result = await GetContributionByFaculty(currentDate);
            
            if (result is JsonResult jsonResult)
            {
                contributionFaculty = jsonResult.Value as List<ContributionFaculty>;
            }


            //GET ALL YEARS
            List<int> years = await _context.Contributions
                .Select(c => c.SubmissionDate.Year)
                .Distinct()
                .ToListAsync();


            //GET CONTRIBUTIONS BY YEAR
            List<ContributionDate> ContributionDate = new List<ContributionDate>();
            int selectedYear = DateTime.Now.Year; 

            if (task == "ContributionYear" && !string.IsNullOrEmpty(year)) { selectedYear = Convert.ToInt32(year); }
            var yearResult = await GetContributionByYear(selectedYear);

            if (yearResult is JsonResult jsonYearResult)
            {
                ContributionDate = jsonYearResult.Value as List<ContributionDate>;
            }
            

            ViewData["ContributionFaculty"] = contributionFaculty;
            ViewData["Years"] = years;
            ViewData["ContributionYear"] = ContributionDate;
            
            //print ContributionYear
            foreach (var item in ContributionDate)
            {
                Console.WriteLine($"Year: {item.Year}, Month: {item.Month}, TotalByMonth: {item.TotalByMonth}");
            }
            
            return View("admins/index");
        }

        //ContributionFaculty
        public async Task<IActionResult> GetContributionByFaculty(DateTime date)
        {
            
            /*SELECT COUNT(a.Id) AS 'Total', 
            f.name AS 'Faculties', 
            CONVERT(date, GETDATE()) AS 'Date' 
            FROM AspNetUsers a
            JOIN Faculties f ON a.FacultyId = f.facultyID
            JOIN Contributions c ON c.userId = a.Id
            WHERE YEAR(c.submissionDate) = YEAR(GETDATE())
            GROUP BY f.name;*/

            List<ContributionFaculty> contributions = await _context.Users
                .Join(_context.Faculties, u => u.FacultyId, f => f.FacultyId, (u, f) => new { User = u, Faculty = f })
                .Join(_context.Contributions, uf => uf.User.Id, c => c.UserId, (uf, c) => new { UserFaculty = uf, Contributions = c })
                .Where(uc => uc.Contributions.SubmissionDate.Year == date.Year && uc.Contributions.Status == "Accepted")
                .GroupBy(uc => uc.UserFaculty.Faculty.Name)
                .Select(g => new ContributionFaculty
                {
                    Total = g.Count(),
                    Faculty = g.Key,
                    SubmissionDate = date
                })
                .ToListAsync();

            return Json(contributions);
        }

        // ContributionYear
        public async Task<IActionResult> GetContributionByYear(int year)
        {
            /*
            SELECT
            YEAR(SubmissionDate) AS Year,
            MONTH(SubmissionDate) AS Month,
            COUNT(*) AS TotalAcceptedContributions
            FROM
                Contributions
            WHERE
                Status = 'Accepted' 
            GROUP BY
                YEAR(SubmissionDate), MONTH(SubmissionDate)
            ORDER BY
                YEAR(SubmissionDate), MONTH(SubmissionDate)
            */

            List<ContributionDate> contributions = await _context.Contributions
                .Where(c => c.Status == "Accepted" && c.SubmissionDate.Year == year)
                .GroupBy(c => new { c.SubmissionDate.Year, c.SubmissionDate.Month })
                .Select(g => new ContributionDate
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalByMonth = g.Count()
                })
                .OrderBy(c => c.Year)
                .ThenBy(c => c.Month)
                .ToListAsync();

            return Json(contributions);
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
        public IActionResult TableUser()
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
        public async Task<IActionResult> CoordinatorComment(int? id)
        {
            ViewData["Title"] = "Create Comment";
            var contribution = await _context.Contributions.FindAsync(id);
            return View("coordinators/create_comment", contribution);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var contribution = await _context.Contributions.FindAsync(id);
            if (contribution == null)
            {
                return NotFound();
            }

            if (contribution.Status == null)
            {
                contribution.Status = "Pending";
            }
            else
            {
                if (status == "Rejected" && contribution.Status != "Rejected")
                {
                    contribution.Status = status;
                }
                if (status == "Approved" && contribution.Status != "Approved")
                {
                    contribution.Status = status;
                }
            }
            _context.Update(contribution);
            await _context.SaveChangesAsync();

            return RedirectToAction("StudentSubmissionCoordinators", "Managers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComment(Contribution contribution)
        {
            
                var existingContribution = await _context.Contributions.FindAsync(contribution.ContributionId);
                if (existingContribution != null)
                {
                    
                    existingContribution.Comment = contribution.Comment;
                    
                    _context.Update(existingContribution);
                    await _context.SaveChangesAsync();
                return RedirectToAction("StudentSubmissionCoordinators");
            }
          
            return RedirectToAction("StudentSubmissionCoordinators", "Managers");
        }




    }
}