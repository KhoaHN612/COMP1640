﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using COMP1640.Models;
using COMP1640.Models.MultiModels;
using Microsoft.AspNetCore.Identity;
using COMP1640.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;


namespace COMP1640.Controllers
{
    public class ManagersController : Controller
    {
        private readonly UserManager<COMP1640User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSenderCustom _emailSender;
        private readonly Comp1640Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ManagersController(Comp1640Context context, IWebHostEnvironment webHostEnvironment,
         UserManager<COMP1640User> UserManager, RoleManager<IdentityRole> roleManager, IEmailSenderCustom EmailSender)
        {
            _roleManager = roleManager;
            _userManager = UserManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = EmailSender;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string task, string year)
        {
            ViewData["Title"] = "Dashboard";

            // GET CONTRIBUTION BY FACULTY
            DateTime currentDate = DateTime.Now;

            if (task == "ContributionFaculty" && !string.IsNullOrEmpty(year)) { currentDate = new DateTime(Convert.ToInt32(year), 1, 1); }
            List<ContributionFaculty> contributionFaculty = await GetContributionByFaculty(currentDate);
            if (contributionFaculty.Count == 0) { contributionFaculty.Add(new ContributionFaculty { SubmissionDate = currentDate }); }

            //GET ALL YEARS
            List<int> years = await _context.Contributions
                .Select(c => c.SubmissionDate.Year)
                .Distinct()
                .ToListAsync();

            // GET ALL YEARS IN BROWERS
            List<int> YearBrower = await _context.WebAccessLogs
                .Select(a => a.AccessDate.Year)
                .Distinct()
                .ToListAsync();

            //GET CONTRIBUTIONS BY YEAR
            int selectedYear = DateTime.Now.Year;

            if (task == "ContributionYear" && !string.IsNullOrEmpty(year)) { selectedYear = Convert.ToInt32(year); }
            List<ContributionDate> contributionByYear = await GetContributionByYear(selectedYear);

            //GET CONTRIBUTIONS BY USER
            int selectedYearUser = DateTime.Now.Year;

            if (task == "ContributionUser" && !string.IsNullOrEmpty(year)) { selectedYearUser = Convert.ToInt32(year); }
            List<ContributionUser> contributiors = await GetContributionByUser(selectedYearUser);

            Console.WriteLine("============ContributionUser============");
            //print contributiors
            foreach (var contributior in contributiors)
            {
                Console.WriteLine("Id: " + contributior.Id + " FullName: " + contributior.FullName + " Faculty: " + contributior.Faculty + " TotalContribution: " + contributior.TotalContribution + " TotalAccept: " + contributior.TotalAccept + " TotalReject: " + contributior.TotalReject + " TotalPending: " + contributior.TotalPending + " Year: " + contributior.Year);
            }
            
            //GET ROLE STATISTICS
            List<RoleStatistics> roleStatistics = await GetRoleStatistics();
            ViewData["Years"] = years;
            ViewData["YearBrower"] = YearBrower;
            ViewData["ContributionFaculty"] = contributionFaculty;
            ViewData["ContributionByYear"] = contributionByYear;
            ViewData["Contributiors"] = contributiors;
            ViewData["RoleStatistics"] = roleStatistics;

            return View("admins/index");
        }

        //ContributionFaculty
        public async Task<List<ContributionFaculty>> GetContributionByFaculty(DateTime date)
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
                .Where(uc => uc.Contributions.SubmissionDate.Year == date.Year)
                .GroupBy(uc => uc.UserFaculty.Faculty.Name)
                .Select(g => new ContributionFaculty
                {
                    Total = g.Count(),
                    Faculty = g.Key,
                    SubmissionDate = date
                })
                .ToListAsync();

            if (contributions.Count <= 0)
            {
                contributions.Add(new ContributionFaculty { SubmissionDate = date });
            }

            return contributions;
        }

        // ContributionYear
        public async Task<List<ContributionDate>> GetContributionByYear(int year)
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
                .Where(c => c.Status == "Approved" && c.SubmissionDate.Year == year)
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

            if (contributions.Count <= 0)
            {
                contributions.Add(new ContributionDate { Year = year });
            }

            return contributions;
        }

        // Quantity of contributions by user
        public async Task<List<ContributionUser>> GetContributionByUser(int year)
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
                .Join(_context.Faculties,
                    u => u.FacultyId,
                    f => f.FacultyId,
                    (u, f) => new { User = u, Faculty = f })
                .Join(_context.Contributions,
                    uf => uf.User.Id,
                    c => c.UserId,
                    (uf, c) => new { UserFaculty = uf, Contribution = c })
                .Where(uc => uc.Contribution.SubmissionDate.Year == year)
                .GroupBy(uc => new { uc.UserFaculty.Faculty.FacultyId, uc.UserFaculty.Faculty.Name })
                .Select(g => new ContributionUser
                {
                    Faculty = g.Key.Name,
                    TotalContribution = g.Select(x => x.UserFaculty.User.Id).Distinct().Count(),
                    Year = year
                }).ToListAsync();

            if (contributions.Count <= 0)
            {
                contributions.Add(new ContributionUser { Year = year });
            }

            return contributions;
        }

        //Thống kê mỗi role có bao nhiêu người
        public async Task<List<RoleStatistics>> GetRoleStatistics()
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

            return roleStatistics;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TableFaculty()
        {
            var pageName = ControllerContext.ActionDescriptor.ActionName;
            var pageVisit = await _context.PageVisits.FirstOrDefaultAsync(p => p.PageName == pageName);

            if (pageVisit == null)
            {
                pageVisit = new PageVisit { PageName = pageName };
                _context.PageVisits.Add(pageVisit);
            }

            pageVisit.VisitCount++;

            await _context.SaveChangesAsync();
            List<Faculty> faculty = _context.Faculties.OrderBy(f => f.FacultyId).ToList();
            ViewData["Title"] = "Faculty Table page";
            return View("admins/table_faculty", faculty);
        }
        public IActionResult FormCreateFaculty(int? id)
        {
            if (id != null)
            {
                ViewData["Title"] = "Edit Faculty";
                ViewData["ButtonLabel"] = "Update";
            }
            else
            {
                ViewData["Title"] = "Create Faculty";
                ViewData["ButtonLabel"] = "Submit";
            }
            Faculty faculty = id != null ? _context.Faculties.Find(id) : new Faculty();
            return View("admins/form_create_faculty", faculty);
        }

        [HttpPost]
        public IActionResult CreateFaculty(string name, string deanName, string description, Faculty faculty)
        {
                int? maxFacultyId = _context.Faculties.Max(f => (int?)f.FacultyId);
                int newFacultyId = (maxFacultyId ?? 0) + 1;
                faculty.FacultyId = newFacultyId;
                faculty.Name = name;
                faculty.Description = description;
                faculty.DeanName = deanName;
                _context.Faculties.Add(faculty);
                _context.SaveChanges();
                return RedirectToAction("TableFaculty");
        }
        [HttpPost]
        public IActionResult UpdateFaculty(Faculty faculty)
        {
            var existingFaculty = _context.Faculties.Find(faculty.FacultyId);
            if (existingFaculty == null)
            {
                return RedirectToAction("TableFaculty");
            }

            if (ModelState.IsValid)
            {
                _context.Entry(existingFaculty).CurrentValues.SetValues(faculty);
                _context.SaveChanges();
                return RedirectToAction("TableFaculty");
            }
            return View("admins/form_create_faculty", faculty);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TableSubmissionDate()
        {
            var pageName = ControllerContext.ActionDescriptor.ActionName;
            var pageVisit = await _context.PageVisits.FirstOrDefaultAsync(p => p.PageName == pageName);

            if (pageVisit == null)
            {
                pageVisit = new PageVisit { PageName = pageName };
                _context.PageVisits.Add(pageVisit);
            }

            pageVisit.VisitCount++;

            await _context.SaveChangesAsync();
            List<AnnualMagazine> annualMagazine = _context.AnnualMagazines.OrderBy(f => f.AnnualMagazineId).ToList();
            ViewData["Title"] = "Submission Date Table page";
            return View("admins/table_submission_date", annualMagazine);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult FormCreateSubmissionDate(int? id)
        {
            if (id != null)
            {
                ViewData["Title"] = "Edit Annual Magazine";
                ViewData["ButtonLabel"] = "Update";
            }
            else
            {
                ViewData["Title"] = "Create Annual Magazine";
                ViewData["ButtonLabel"] = "Submit";
            }
            AnnualMagazine annualMagazine = id != null ? _context.AnnualMagazines.Find(id) : new AnnualMagazine();
            return View("admins/form_create_submission_date", annualMagazine);
        }

        [HttpPost]
        public IActionResult CreateAnnualMagazine(AnnualMagazine annualMagazine)
        {
            if (ModelState.IsValid)
            {
                int? maxAnnualMagazineId = _context.AnnualMagazines.Max(f => (int?)f.AnnualMagazineId);
                int newFacultyId = (maxAnnualMagazineId ?? 0) + 1;
                annualMagazine.AnnualMagazineId = newFacultyId;
                annualMagazine.IsActive = true;
                _context.AnnualMagazines.Add(annualMagazine);
                _context.SaveChanges();
                return RedirectToAction("TableSubmissionDate");
            }
            return View("admins/form_create_submission_date", annualMagazine);
        }

        [HttpPost]
        public IActionResult UpdateAnnualMagazine(AnnualMagazine annualMagazine)
        {
            Console.WriteLine("============UpdateMagazine============");
            //print annualMagazine
            foreach (var prop in annualMagazine.GetType().GetProperties())
            {
                Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(annualMagazine, null));
            }

            var existingAnnualMagazine = _context.AnnualMagazines.Find(annualMagazine.AnnualMagazineId);
            if (existingAnnualMagazine == null)
            {
                return RedirectToAction("TableSubmissionDate");
            }

            if (ModelState.IsValid)
            {
                _context.Entry(existingAnnualMagazine).CurrentValues.SetValues(annualMagazine);
                _context.SaveChanges();
                return RedirectToAction("TableSubmissionDate");
            }
            return View("admins/form_create_submission_date", annualMagazine);
        }

        [HttpDelete]
        public IActionResult DeleteAnnualMagazine(int id)
        {
            var annualManazineToDelete = _context.AnnualMagazines.Find(id);
            if (annualManazineToDelete != null)
            {
                var contributions = _context.Contributions.Where(c => c.AnnualMagazineId == id);
                foreach (var c in contributions)
                {
                    _context.Contributions.Remove(c);
                }
                _context.AnnualMagazines.Remove(annualManazineToDelete);

                _context.SaveChanges();
            }

            return Ok();
        }

        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> IndexCoordinators(string task, string year)
        {
            ViewData["Title"] = "Dashboard Coordinators";
            List<TotalContribution> TotalContribution = new List<TotalContribution>();
            List<TotalContribution> TotalContributionsApproved = new List<TotalContribution>();
            List<TotalContribution> TotalContributionsPublished = new List<TotalContribution>();
            List<TotalContribution> TotalContributionsRejected = new List<TotalContribution>();
            List<TotalContribution> TotalContributionsPending = new List<TotalContribution>();
            List<ContributionWithoutComment> contributions = new List<ContributionWithoutComment>();
            List<ContributionWithoutComment> contributionWithoutComments = new List<ContributionWithoutComment>();
            List<ContributionWithoutComment> contributionWithoutCommentsAfter14Days = new List<ContributionWithoutComment>();
            List<ContributionUser> contributionUser = new List<ContributionUser>();
            //Total Contributions
            DateTime currentDate = DateTime.Now;
            if (task == "TotalContribution" && !string.IsNullOrEmpty(year)) { currentDate = new DateTime(Convert.ToInt32(year), 1, 1); }
            //get current faculty of current user
            var currentUser = await _userManager.GetUserAsync(User);
            //get faculty id of current user
            if (currentUser == null || currentUser.FacultyId == null)
            {
                return null;
            }
            else
            {
                int currentFacultyId = currentUser.FacultyId ?? 0; ;

                TotalContribution = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributions");

                //Total Contributions Published
                TotalContributionsPublished = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributionsPuslished");

                //Total Contributions Accepted
                TotalContributionsApproved = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributionsApproved");

                //Total Contributions Rejected
                TotalContributionsRejected = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributionsRejected");

                //Total Contributions Pending
                TotalContributionsPending = await GetTotalContributions(currentFacultyId, currentDate.Year, "TotalContributionsPending");

                 contributionWithoutComments = (from c in _context.Contributions
                    join u in _context.Users on c.UserId equals u.Id
                    join cm in _context.Comments on c.ContributionId equals cm.ContributionId into cmGroup
                    from cm in cmGroup.DefaultIfEmpty()
                    where cm.CommentId == null
                            && DateTime.Now <= c.CommentDeadline
                            && c.Status == "Approved"
                            && u.FacultyId == currentFacultyId
                            && c.SubmissionDate.Year == DateTime.Now.Year
                    group c by c.SubmissionDate.Year into g
                    select new ContributionWithoutComment
                    {
                        Year = g.Key,
                        Quantity = g.Count()
                    }).ToList();

                contributionWithoutCommentsAfter14Days = (from c in _context.Contributions
                    join u in _context.Users on c.UserId equals u.Id
                    join cm in _context.Comments on c.ContributionId equals cm.ContributionId into cmGroup
                    from cm in cmGroup.DefaultIfEmpty()
                    where cm.CommentId == null
                            && DateTime.Now > c.CommentDeadline
                            && c.Status == "Approved"
                            && u.FacultyId == currentFacultyId
                            && c.SubmissionDate.Year == DateTime.Now.Year
                    group c by c.SubmissionDate.Year into g
                    select new ContributionWithoutComment
                    {
                        Year = g.Key,
                        Quantity = g.Count()
                    }).ToList();

                //GET ALL CONTRIBUTIONS
                contributions = (from c in _context.Contributions
                    join u in _context.Users on c.UserId equals u.Id
                    join cm in _context.Comments on c.ContributionId equals cm.ContributionId into cmGroup
                    from cm in cmGroup.DefaultIfEmpty()
                    where cm.CommentId == null
                            && c.Status == "Approved"
                            && u.FacultyId == currentFacultyId
                            && c.SubmissionDate.Year == DateTime.Now.Year
                    group c by c.SubmissionDate.Year into g
                    select new ContributionWithoutComment
                    {
                        Year = g.Key,
                        Quantity = g.Count()
                    }).ToList();
                if(contributions.Count == 0)
                {
                    contributions.Add(new ContributionWithoutComment { Year = currentDate.Year });
                }
                
                //GET CONTRIBUTIONS BY USER
                int selectedYearUser = DateTime.Now.Year;

                if (task == "ContributionUser" && !string.IsNullOrEmpty(year)) { selectedYearUser = Convert.ToInt32(year); }
                contributionUser = await GetContributors(currentFacultyId, selectedYearUser);
            }


            //GET ALL YEARS
            List<int> years = await _context.Contributions
                .Select(c => c.SubmissionDate.Year)
                .Distinct()
                .ToListAsync();


            ViewData["TotalContributionsApproved"] = TotalContributionsApproved;
            ViewData["TotalContributionsPublished"] = TotalContributionsPublished;
            ViewData["TotalContributionsRejected"] = TotalContributionsRejected;
            ViewData["TotalContributionsPending"] = TotalContributionsPending;
            ViewData["TotalContribution"] = TotalContribution;
            ViewData["ContributionUser"] = contributionUser;
            ViewData["ContributionWithoutComments"] = contributionWithoutComments;
            ViewData["ContributionWithoutCommentsAfter14Days"] = contributionWithoutCommentsAfter14Days;
            ViewData["Contributions"] = contributions;
            ViewData["Years"] = years;

            return View("coordinators/index");
        }

        public async Task<List<ContributionUser>> GetContributors(int currentFacultyId, int year)
        {
            List<ContributionUser> contributions = await _context.Users
                .Join(_context.Faculties, u => u.FacultyId, f => f.FacultyId, (u, f) => new { User = u, Faculty = f })
                .Join(_context.Contributions, uf => uf.User.Id, c => c.UserId, (uf, c) => new { UserFaculty = uf, Contributions = c })
                .Where(uc => uc.Contributions.SubmissionDate.Year == year && uc.UserFaculty.User.FacultyId == currentFacultyId)
                .GroupBy(uc => new { uc.UserFaculty.User.Id, uc.UserFaculty.User.FullName, uc.UserFaculty.Faculty.Name })
                .Select(g => new ContributionUser
                {
                    Id = g.Key.Id,
                    FullName = g.Key.FullName,
                    Faculty = g.Key.Name,
                    TotalContribution = g.Count(),
                    TotalAccept = g.Where(c => c.Contributions.Status == "Approved").Count(),
                    TotalReject = g.Where(c => c.Contributions.Status == "Rejected").Count(),
                    TotalPending = g.Where(c => c.Contributions.Status == "Pending").Count(),
                    Year = year
                })
                .ToListAsync();

            if (contributions.Count == 0)
            {
                var facultyName = await _context.Faculties.FirstOrDefaultAsync(f => f.FacultyId == currentFacultyId);
                var faculty = facultyName != null ? facultyName.Name : null;

                contributions.Add(new ContributionUser { Faculty = faculty, Year = year });
            }

            return contributions;
        }

        public async Task<List<TotalContribution>> GetTotalContributions(int facultyID, int year, string action)
        {
            List<TotalContribution> contributions = new List<TotalContribution>();
            var query = from c in _context.Contributions
                        join u in _context.Users on c.UserId equals u.Id
                        where c.SubmissionDate.Year == year
                        select new { Contribution = c, User = u };

            switch (action)
            {
                case "TotalContributionsPuslished":
                    query = query.Where(c => c.Contribution.IsPublished == true);
                    break;
                case "TotalContributionsApproved":
                    query = query.Where(c => c.Contribution.Status == "Approved");
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

            if (action == "TotalContributionsPuslished" || action == "TotalContributionsApproved" || action == "TotalContributionsRejected" || action == "TotalContributionsPending")
            {
                contributions = await query
                    .Where(uc => uc.User.FacultyId == facultyID)
                    .GroupBy(c => new { c.Contribution.SubmissionDate.Year })
                    .Select(g => new TotalContribution
                    {
                        Year = g.Key.Year,
                        Total = g.Count()
                    })
                    .OrderBy(c => c.Year)
                    .ToListAsync();
            }
            else
            {
                contributions = await query
                    .Where(uc => uc.User.FacultyId == facultyID)
                    .GroupBy(c => new { c.Contribution.SubmissionDate.Year })
                    .Select(g => new TotalContribution
                    {
                        Year = g.Key.Year,
                        Total = g.Count()
                    })
                    .OrderBy(c => c.Year)
                    .ToListAsync();
            
                //print contributions
                foreach (var contribution in contributions)
                {
                    Console.WriteLine("Year: " + contribution.Year + " Total: " + contribution.Total);
                }
            }       

            if (contributions.Count == 0)
            {
                contributions.Add(new TotalContribution { Year = year, Total = 0 });
            }

            return contributions;
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> SubmitListManager()
        {

            ViewData["Title"] = "List Submission";

            var pageName = ControllerContext.ActionDescriptor.ActionName;
            var pageVisit = await _context.PageVisits.FirstOrDefaultAsync(p => p.PageName == pageName);

            if (pageVisit == null)
            {
                pageVisit = new PageVisit { PageName = pageName };
                _context.PageVisits.Add(pageVisit);
            }

            pageVisit.VisitCount++;

            await _context.SaveChangesAsync();


            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.FacultyId == null)
            {
                return NotFound();
            }
            var currentFacultyId = currentUser.FacultyId;

            var userIdsInSameFaculty = await _context.Users
                .Where(u => u.FacultyId == currentFacultyId)
                .Select(u => u.Id)
                .ToListAsync();

            var contributions = await _context.Contributions
                .Where(c => userIdsInSameFaculty.Contains(c.UserId))
                .ToListAsync();

            return View("head_managers/SubmitList", contributions);
        }

        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> StudentSubmissionCoordinators(int? id)
        {

            ViewData["Title"] = "List Submission";

            var pageName = ControllerContext.ActionDescriptor.ActionName;
            var pageVisit = await _context.PageVisits.FirstOrDefaultAsync(p => p.PageName == pageName);

            if (pageVisit == null)
            {
                pageVisit = new PageVisit { PageName = pageName };
                _context.PageVisits.Add(pageVisit);
            }

            pageVisit.VisitCount++;

            await _context.SaveChangesAsync();


            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.FacultyId == null)
            {
                return NotFound();
            }
            var currentFacultyId = currentUser.FacultyId;

            var userIdsInSameFaculty = await _context.Users
                .Where(u => u.FacultyId == currentFacultyId)
                .Select(u => u.Id)
                .ToListAsync();

            var contributions = await _context.Contributions
                .Where(c => userIdsInSameFaculty.Contains(c.UserId))
                .ToListAsync();

            return View("coordinators/student_submission", contributions);
        }


        [Authorize(Roles = "Coordinator, Student, Manager")]
        public async Task<IActionResult> CoordinatorComment(int? id)
        {
            ViewData["Title"] = "Create Comment";
            var contribution = await _context.Contributions.FindAsync(id);
            var comments = await _context.Comments
                                .Where(c => c.ContributionId == id)
                                .ToListAsync();
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

            var commentDeadline = contribution.CommentDeadline;
            var contributionUser = await _userManager.FindByIdAsync(contribution.UserId);
            var contributionUserFullName = contributionUser.FullName;
            bool isCommentDeadlineOver = contribution.CommentDeadline < DateTime.Now;
            ViewBag.isCommentDeadlineOver = isCommentDeadlineOver;
            ViewBag.userEmail = userEmail;
            ViewBag.contributions = contributions;
            ViewBag.userFaculty = userFaculty;
            ViewBag.userId = anotherUserId;
            ViewBag.contributionUserId = contribution.UserId;
            ViewBag.contributionUserName = contributionUserFullName;
            ViewBag.contributionsTile = contribution.Title;
            ViewBag.userFullName = userFullName;
            ViewBag.userAddress = userAddress;
            ViewBag.userProfileImagePath = userProfileImagePath;
            ViewBag.Comments = comments;
            ViewBag.Deadline = contribution.CommentDeadline;
            return View("coordinators/create_comment", contribution);
        }

        //================================ MANAGERS ================================//
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> IndexManagers(string task, string year)
        {
            ViewData["Title"] = "Dashboard Managers";

            //GET CONTRIBUTIONS BY YEAR
            int selectedYearAll = DateTime.Now.Year;
            int selectedYearApproved = DateTime.Now.Year;
            int selectedYearRejected = DateTime.Now.Year;
            int selectedYearPending = DateTime.Now.Year;

            int yearConvert = Convert.ToInt32(year);

            //GET ALL YEARS
            List<int> years = await _context.Contributions
                .Select(c => c.SubmissionDate.Year)
                .Distinct()
                .ToListAsync();

            //GET CONTRIBUTIONS BY YEAR
            List<ContributionDate> pendingResults = new List<ContributionDate>();

            if (task == "TotalContribution" && !string.IsNullOrEmpty(year)) { selectedYearAll = Convert.ToInt32(year); }
            List<ContributionDate> allResults = await GetContributionsByStatus(selectedYearAll, "All");

            if (task == "ApprovedContribution" && !string.IsNullOrEmpty(year)) { selectedYearApproved = yearConvert; }
            List<ContributionDate> approvedResults = await GetContributionsByStatus(selectedYearApproved, "Approved");

            if (task == "RejectedContribution" && !string.IsNullOrEmpty(year)) { selectedYearRejected = yearConvert; }
            List<ContributionDate> rejectedResults = await GetContributionsByStatus(selectedYearRejected, "Rejected");

            if (task == "PendingContribution" && !string.IsNullOrEmpty(year)) { selectedYearPending = Convert.ToInt32(year); }
            pendingResults = await GetContributionsByStatus(selectedYearPending, "Pending");
            if (year == null)
            {
                year = DateTime.Now.Year.ToString();
            }

            if (allResults.Count == 0) { allResults.Add(new ContributionDate { Year = int.Parse(year) }); }
            if (approvedResults.Count == 0) { approvedResults.Add(new ContributionDate { Year = Convert.ToInt32(year) }); }
            if (rejectedResults.Count == 0) { rejectedResults.Add(new ContributionDate { Year = Convert.ToInt32(year) }); }
            if (pendingResults.Count == 0) { pendingResults.Add(new ContributionDate { Year = Convert.ToInt32(year) }); }

            ViewData["Years"] = years;
            ViewData["Contributions"] = allResults;
            ViewData["ApprovedContribution"] = approvedResults;
            ViewData["RejectedContribution"] = rejectedResults;
            ViewData["PendingContribution"] = pendingResults;

            return View("head_managers/index");
        }

        public async Task<List<ContributionDate>> GetContributionsByStatus(int year, string status)
        {
            /*
            SELECT 
                YEAR(C.submissionDate) AS SubmissionYear, 
                MONTH(C.submissionDate) AS SubmissionMonth, 
                COUNT(*) AS TotalContributions, 
                F.name AS Faculty
            FROM 
                AspNetUsers U
            JOIN 
                Faculties F ON U.FacultyId = F.facultyID
            JOIN 
                Contributions C ON C.userId = U.Id
            WHERE 
                YEAR(C.submissionDate) = 2024
            GROUP BY 
                MONTH(C.submissionDate), YEAR(C.submissionDate), F.facultyID, F.name
            ORDER BY 
                MONTH(C.submissionDate) ASC;
            */
            List<ContributionDate> contributions = new List<ContributionDate>();

            if (status == "All")
            {
                contributions = await _context.Users
                .Join(_context.Faculties, u => u.FacultyId, f => f.FacultyId, (u, f) => new { User = u, Faculty = f })
                .Join(_context.Contributions, uf => uf.User.Id, c => c.UserId, (uf, c) => new { UserFaculty = uf, Contributions = c })
                .Where(uc => uc.Contributions.SubmissionDate.Year == year)
                .GroupBy(uc => new { uc.Contributions.SubmissionDate.Year, uc.Contributions.SubmissionDate.Month, uc.UserFaculty.Faculty.FacultyId, uc.UserFaculty.Faculty.Name })
                .Select(g => new ContributionDate
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    FacultyName = g.Key.Name,
                    TotalByMonth = g.Count()
                })
                .OrderBy(c => c.Year)
                .ThenBy(c => c.Month)
                .ToListAsync();
            }
            else
            {
                contributions = await _context.Users
                .Join(_context.Faculties, u => u.FacultyId, f => f.FacultyId, (u, f) => new { User = u, Faculty = f })
                .Join(_context.Contributions, uf => uf.User.Id, c => c.UserId, (uf, c) => new { UserFaculty = uf, Contributions = c })
                .Where(uc => uc.Contributions.SubmissionDate.Year == year && uc.Contributions.Status == status)
                .GroupBy(uc => new { uc.Contributions.SubmissionDate.Year, uc.Contributions.SubmissionDate.Month, uc.UserFaculty.Faculty.Name })
                .Select(g => new ContributionDate
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    FacultyName = g.Key.Name,
                    TotalByMonth = g.Count()
                })
                .OrderBy(c => c.Year)
                .ThenBy(c => c.Month)
                .ToListAsync();
            }

            if (contributions.Count <= 0)
            {
                contributions.Add(new ContributionDate { Year = year });
            }

            return contributions;
        }


        //Number of Student by Faculty GetStudentByFaculty
        // public async Task<IActionResult> GetStudentByFaculty()
        // {
        //     /*
        //     SELECT 
        //         f.name AS 'Faculty',
        //         COUNT(u.Id) AS 'Total'
        //     FROM 
        //         AspNetUsers u
        //     JOIN 
        //         AspNetUserRoles ur ON u.Id = ur.UserId
        //     JOIN 
        //         AspNetRoles r ON ur.RoleId = r.Id
        //     JOIN 
        //         Faculties f ON u.FacultyId = f.facultyID
        //     WHERE 
        //         u.Role = 'Student'
        //     GROUP BY 
        //         f.name;
        //     */

        //     List<StudentByFaculty> students = await _context.Users
        //         .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { User = u, UserRole = ur })
        //         .Join(_context.Roles, ur => ur.UserRole.RoleId, r => r.Id, (ur, r) => new { UserRole = ur, Role = r })
        //         .Join(_context.Faculties, ur => ur.User.User.FacultyId, f => f.FacultyId, (ur, f) => new { UserRole = ur, Faculty = f })
        //         .Where(urf => urf.UserRole.User.Role == "Student")
        //         .GroupBy(urf => urf.Faculty.Name)
        //         .Select(g => new StudentByFaculty
        //         {
        //             Faculty = g.Key,
        //             Total = g.Count()
        //         })
        //         .ToListAsync();

        //     return Json(students);
        // }
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> StudentSubmissionManagers()
        {
            var pageName = ControllerContext.ActionDescriptor.ActionName;
            var pageVisit = await _context.PageVisits.FirstOrDefaultAsync(p => p.PageName == pageName);

            if (pageVisit == null)
            {
                pageVisit = new PageVisit { PageName = pageName };
                _context.PageVisits.Add(pageVisit);
            }

            pageVisit.VisitCount++;

            await _context.SaveChangesAsync();
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
                    var completeFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "contributionUpload", fileDetail.FilePath);
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
            var approvedContributions = await _context.Contributions
                .Where(c => c.Status == "Approved")
                .ToListAsync();

            var memoryStream = new MemoryStream();
            try
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var contribution in approvedContributions)
                    {
                        var fileDetails = await _context.FileDetails
                            .Where(fd => fd.ContributionId == contribution.ContributionId)
                            .ToListAsync();

                        foreach (var fileDetail in fileDetails)
                        {
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "contributionUpload", fileDetail.FilePath);

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
        [Authorize(Roles = "Manager,Admin,Coordinator")]
        public async Task<IActionResult> ShowProfileAsync()
        {
            ViewData["Title"] = "Profiles";
            var curUser = await _userManager.GetUserAsync(User);
            if (curUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            ViewBag.roles = await _userManager.GetRolesAsync(curUser);
            var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.FacultyId == curUser.FacultyId);
            ViewBag.facultyName = faculty?.Name;
            return View("profile_managers", curUser);
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
        public async Task<IActionResult> UpdateComment(Contribution contribution, string userId)
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
                var userComment = _userManager.FindByIdAsync(userId);
                ViewBag.userFullName = await GetUserFullName();

                return RedirectToAction("StudentSubmissionCoordinators");
            }

            return RedirectToAction("StudentSubmissionCoordinators", "Managers");
        }
        private async Task<string> GetUserFullName()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            return user?.FullName; // This will return null if user is null.
        }


        [HttpPost]
        public async Task<IActionResult> Publish(int id)
        {
            var contribution = await _context.Contributions.FindAsync(id);
            if (contribution == null)
            {
                return NotFound();
            }
            try
            {
                contribution.IsPublished = true;
                var result = await _context.SaveChangesAsync();
                return RedirectToAction("StudentSubmissionManagers");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Fail");
                return View();
            }
        }

    }
}
