using Microsoft.AspNetCore.Mvc;
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


            //GET CONTRIBUTIONS BY USER
            List<ContributionUser> ContributionUser = new List<ContributionUser>();
            int selectedYearUser = DateTime.Now.Year;

            if (task == "ContributionUser" && !string.IsNullOrEmpty(year)) { selectedYearUser = Convert.ToInt32(year); }
            var userResult = await GetContributionByUser(selectedYearUser);

            if (userResult is JsonResult jsonUserResult)
            {
                ContributionUser = jsonUserResult.Value as List<ContributionUser>;
            }     


            //GET ROLE STATISTICS
            List<RoleStatistics> roleStatistics = new List<RoleStatistics>();
            var roleResult = await GetRoleStatistics();

            if (roleResult is JsonResult jsonRoleResult)
            {
                roleStatistics = jsonRoleResult.Value as List<RoleStatistics>;
            }      

            ViewData["ContributionFaculty"] = contributionFaculty;
            ViewData["Years"] = years;
            ViewData["ContributionYear"] = ContributionDate;
            ViewData["ContributionUser"] = ContributionUser;
            ViewData["RoleStatistics"] = roleStatistics;
            
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

        // Quantity of contributions by user
        public async Task<IActionResult> GetContributionByUser(int year)
        {
            /*
            SELECT 
                u.Id,
                u.FullName,
                f.name AS 'Faculty',
                COUNT(c.status) AS 'Total contributions',
                SUM(CASE WHEN c.status = 'Accepted' THEN 1 ELSE 0 END) AS 'Accepted',
                SUM(CASE WHEN c.status = 'Rejected' THEN 1 ELSE 0 END) AS 'Rejected',
                SUM(CASE WHEN c.status = 'Pending' THEN 1 ELSE 0 END) AS 'Pending'
            FROM 
                AspNetUsers u
            JOIN 
                Faculties f ON u.FacultyId = f.facultyID
            JOIN 
                Contributions c ON u.Id = c.userId
            WHERE 
                YEAR(c.submissionDate) = 2023
            GROUP BY 
                u.FullName, f.name;
            */

            List<ContributionUser> contributions = await _context.Users
                .Join(_context.Faculties, u => u.FacultyId, f => f.FacultyId, (u, f) => new { User = u, Faculty = f })
                .Join(_context.Contributions, uf => uf.User.Id, c => c.UserId, (uf, c) => new { UserFaculty = uf, Contributions = c })
                .Where(uc => uc.Contributions.SubmissionDate.Year == year)
                .GroupBy(uc => new { uc.UserFaculty.User.Id, uc.UserFaculty.User.FullName, uc.UserFaculty.Faculty.Name })
                .Select(g => new ContributionUser
                {
                    Id = g.Key.Id,
                    FullName = g.Key.FullName,
                    Faculty = g.Key.Name,
                    TotalContribution = g.Count(),
                    TotalAccept = g.Where(c => c.Contributions.Status == "Accepted").Count(),
                    TotalReject = g.Where(c => c.Contributions.Status == "Rejected").Count(),
                    TotalPending = g.Where(c => c.Contributions.Status == "Pending").Count(),
                    Year = year
                })
                .ToListAsync();
            
            return Json(contributions);
        }

        //Thống kê mỗi role có bao nhiêu người
        public async Task<IActionResult> GetRoleStatistics()
        {
            /*
            SELECT 
                r.Name AS 'Role',
                COUNT(u.Id) AS 'Total'
            FROM 
                AspNetUsers u
            JOIN 
                AspNetUserRoles ur ON u.Id = ur.UserId
            JOIN 
                AspNetRoles r ON ur.RoleId = r.Id
            GROUP BY 
                r.Name;
            */

            List<RoleStatistics> roleStatistics = await _context.Users
                .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles, ur => ur.UserRole.RoleId, r => r.Id, (ur, r) => new { UserRole = ur, Role = r })
                .GroupBy(ur => ur.Role.Name)
                .Select(g => new RoleStatistics
                {
                    Role = g.Key,
                    Total = g.Count()
                })
                .ToListAsync();

            return Json(roleStatistics);
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
        public async Task<IActionResult> IndexCooridinators(string task, string year)
        {
            ViewData["Title"] = "Dashboard Coordinators";
            Console.WriteLine($"Action: {task}task, Year: {year}");

            //Total Contributions
            DateTime currentDate = DateTime.Now;
            if (task == "TotalContribution" && !string.IsNullOrEmpty(year))
            {
                currentDate = new DateTime(Convert.ToInt32(year), 1, 1);
            }
            List<TotalContribution> TotalContribution = await GetTotalContributions(currentDate.Year, "TotalContributions");

            //Total Contributions Accepted
            List<TotalContribution> TotalContributionsAccepted = await GetTotalContributions(currentDate.Year, "TotalContributionsAccepted");

            //Total Contributions Rejected
            List<TotalContribution> TotalContributionsRejected = await GetTotalContributions(currentDate.Year, "TotalContributionsRejected");

            //Total Contributions Pending
            List<TotalContribution> TotalContributionsPending = await GetTotalContributions(currentDate.Year, "TotalContributionsPending");

            //Get contribution without comment and withou comment after 14 days 
            /*
            SELECT SubmissionDate AS Date,
                COUNT(*) AS ContributionsWithoutComments
            FROM Contributions
            WHERE YEAR(SubmissionDate) = 2024
                AND Comment IS NULL
            GROUP BY MONTH(SubmissionDate);
            */

            List<ContributionWithoutComment> contributionWithoutComments = await _context.Contributions
                .Where(c => c.SubmissionDate.Year == currentDate.Year && c.Comment == null)
                .GroupBy(c => new { c.SubmissionDate.Year, c.SubmissionDate.Month })
                .Select(g => new ContributionWithoutComment
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    ContributionsWithoutComments = g.Count()
                })
                .ToListAsync();

            foreach (var item in contributionWithoutComments)
            {
                Console.WriteLine($"Date: {item.Date}, ContributionsWithoutComments: {item.ContributionsWithoutComments}");
            }

            Console.WriteLine("==============================================");
            
            //Check after 14 days from contribution date
            if(year != null)
            {
                foreach (var item in contributionWithoutComments)
                {
                    item.ContributionsWithoutCommentsAfter14Days = item.ContributionsWithoutComments;
                }
            }else{
                //Kiểm tra quá 14 ngày từ ngày nộp bài hay không
                foreach (var item in contributionWithoutComments)
                {
                    var date = item.Date;
                    var dateAfter14Days = date.AddDays(14);
                    var contributions = await _context.Contributions
                        .Where(c => c.SubmissionDate.Year == date.Year && c.SubmissionDate.Month == date.Month && c.Comment == null)
                        .ToListAsync();
                    foreach (var contribution in contributions)
                    {
                        if (contribution.SubmissionDate.Day < DateTime.Now.Day)
                        {
                            item.ContributionsWithoutCommentsAfter14Days++;
                        }
                    }
                }
            }   
            
            foreach (var item in contributionWithoutComments)
            {
                Console.WriteLine($"Date: {item.Date}, ContributionsWithoutComments: {item.ContributionsWithoutComments}, ContributionsWithoutCommentsAfter14Days: {item.ContributionsWithoutCommentsAfter14Days}");
            }

            //GET ALL YEARS
            List<int> years = await _context.Contributions
                .Select(c => c.SubmissionDate.Year)
                .Distinct()
                .ToListAsync();

            ViewData["TotalContributionsAccepted"] = TotalContributionsAccepted;
            ViewData["TotalContributionsRejected"] = TotalContributionsRejected;
            ViewData["TotalContributionsPending"] = TotalContributionsPending;
            ViewData["TotalContribution"] = TotalContribution;
            ViewData["ContributionWithoutComments"] = contributionWithoutComments;
            ViewData["Years"] = years;

            return View("coordinators/index");
        }

        public async Task<List<TotalContribution>> GetTotalContributions(int year, string action)
        {
            IQueryable<Contribution> query = _context.Contributions.Where(c => c.SubmissionDate.Year == year);

            switch (action)
            {
                case "TotalContributionsAccepted":
                    query = query.Where(c => c.Status == "Accepted");
                    break;
                case "TotalContributionsRejected":
                    query = query.Where(c => c.Status == "Rejected");
                    break;
                case "TotalContributionsPending":
                    query = query.Where(c => c.Status == "Pending");
                    break;
            }

            List<TotalContribution> contributions = await query
                .GroupBy(c => new { c.SubmissionDate.Year, c.SubmissionDate.Month })
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