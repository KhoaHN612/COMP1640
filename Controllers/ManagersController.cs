using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index(string task, string year)
        {
            ViewData["Title"] = "Dashboard";

            // GET CONTRIBUTION BY FACULTY
            DateTime currentDate = DateTime.Now;

            if (task == "ContributionFaculty" && !string.IsNullOrEmpty(year)) { currentDate = new DateTime(Convert.ToInt32(year), 1, 1); }
            List<ContributionFaculty> contributionFaculty = await GetContributionByFaculty(currentDate);

            //GET ALL YEARS
            List<int> years = await _context.Contributions
                .Select(c => c.SubmissionDate.Year)
                .Distinct()
                .ToListAsync();


            //GET CONTRIBUTIONS BY YEAR
            int selectedYear = DateTime.Now.Year;

            if (task == "ContributionYear" && !string.IsNullOrEmpty(year)) { selectedYear = Convert.ToInt32(year); }
            List<ContributionDate> ContributionByYear = await GetContributionByYear(selectedYear);


            //GET CONTRIBUTIONS BY USER
            int selectedYearUser = DateTime.Now.Year;

            if (task == "ContributionUser" && !string.IsNullOrEmpty(year)) { selectedYearUser = Convert.ToInt32(year); }
            List<ContributionUser> ContributionUser = await GetContributionByUser(selectedYearUser);

            //GET ROLE STATISTICS
            List<RoleStatistics> roleStatistics = await GetRoleStatistics();
            foreach(var i in roleStatistics){
                Console.WriteLine(i.Role + " - " + i.Total);
            }

            ViewData["ContributionFaculty"] = contributionFaculty;
            ViewData["Years"] = years;
            ViewData["ContributionByYear"] = ContributionByYear;
            ViewData["ContributionUser"] = ContributionUser;
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
                .Where(uc => uc.Contributions.SubmissionDate.Year == date.Year && uc.Contributions.Status == "Approved")
                .GroupBy(uc => uc.UserFaculty.Faculty.Name)
                .Select(g => new ContributionFaculty
                {
                    Total = g.Count(),
                    Faculty = g.Key,
                    SubmissionDate = date
                })
                .ToListAsync();

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
                    TotalAccept = g.Where(c => c.Contributions.Status == "Approved").Count(),
                    TotalReject = g.Where(c => c.Contributions.Status == "Rejected").Count(),
                    TotalPending = g.Where(c => c.Contributions.Status == "Pending").Count(),
                    Year = year
                })
                .ToListAsync();

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

        public IActionResult TableFaculty()
        {
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
        public IActionResult CreateFaculty(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                int? maxFacultyId = _context.Faculties.Max(f => (int?)f.FacultyId);
                int newFacultyId = (maxFacultyId ?? 0) + 1;
                faculty.FacultyId = newFacultyId;
                _context.Faculties.Add(faculty);
                _context.SaveChanges();
                return RedirectToAction("TableFaculty");
            }
            return View("admins/form_create_faculty", faculty);
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

        [HttpDelete]
        public IActionResult DeleteFaculty(int id)
        {
            var facultyToDelete = _context.Faculties.Find(id);
            if (facultyToDelete != null)
            {
                _context.Faculties.Remove(facultyToDelete);
                _context.SaveChanges();
            }
            return RedirectToAction("TableFaculty");
        }

        public IActionResult TableSubmissionDate()
        {
            List<AnnualMagazine> annualMagazine = _context.AnnualMagazines.OrderBy(f => f.AnnualMagazineId).ToList();
            ViewData["Title"] = "Submission Date Table page";
            return View("admins/table_submission_date", annualMagazine);
        }
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
                _context.AnnualMagazines.Remove(annualManazineToDelete);
                _context.SaveChanges();
            }
            return RedirectToAction("TableSubmissionDate"); // Chuyển hướng sau khi xóa
        }

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

            //GET ALL CONTRIBUTIONS
            List<ContributionWithoutComment> contributions = await _context.Contributions
                .Where(c => c.SubmissionDate.Year == DateTime.Now.Year)
                .GroupBy(c => new { Date = c.SubmissionDate.Date })
                .Select(g => new ContributionWithoutComment
                {
                    Date = g.Key.Date,
                    Quantity = g.Count()
                })
                .ToListAsync();


            //GET CONTRIBUTION WITHOUT COMMENT
            /*
            SELECT 
				YEAR(c.submissionDate) as 'Year',
                c.submissionDate AS 'Date',
                COUNT(*) AS Quantity
            FROM 
                Contributions c
            WHERE 
                YEAR(c.submissionDate) = YEAR(GETDATE()) 
                AND c.Comment IS NULL 
            GROUP BY 
                YEAR(c.submissionDate), c.submissionDate;
            */

    
            List<ContributionWithoutComment> contributionWithoutComments =  await _context.Contributions
                .Where(c => c.SubmissionDate.Year == DateTime.Now.Year && c.Comment == null)
                .GroupBy(c => new { Date = c.SubmissionDate.Date })
                .Select(g => new ContributionWithoutComment
                {
                    Date = g.Key.Date,
                    Quantity = g.Count()
                })
                .ToListAsync();


            //GET CONTRIBUTION WITHOUT COMMENT AFTER 14 DAYS
            /*
            SELECT 
				YEAR(c.submissionDate) as 'Year',
                c.submissionDate AS 'Date',
                COUNT(*) AS Quantity
            FROM 
                Contributions c
            WHERE 
                YEAR(c.submissionDate) = YEAR(GETDATE()) 
                AND c.Comment IS NULL 
                AND DATEDIFF(day, c.submissionDate, GETDATE()) > 14
            GROUP BY 
                YEAR(c.submissionDate), c.submissionDate;
            */

            List<ContributionWithoutComment> contributionWithoutCommentsAfter14Days = await _context.Contributions
                .Where(c => c.SubmissionDate.Year == DateTime.Now.Year
                            && c.Comment == null
                            && EF.Functions.DateDiffDay(c.SubmissionDate, DateTime.Now.Date) > 14)
                .GroupBy(c => new { Date = c.SubmissionDate.Date })
                .Select(g => new ContributionWithoutComment
                {
                    Date = g.Key.Date,
                    Quantity = g.Count()
                })
                .ToListAsync();

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
            ViewData["ContributionWithoutCommentsAfter14Days"] = contributionWithoutCommentsAfter14Days;
            ViewData["Contributions"] = contributions;
            ViewData["Years"] = years;

            return View("coordinators/index");
        }

        public async Task<List<TotalContribution>> GetTotalContributions(int year, string action)
        {
            IQueryable<Contribution> query = _context.Contributions.Where(c => c.SubmissionDate.Year == year);

            switch (action)
            {
                case "TotalContributionsAccepted":
                    query = query.Where(c => c.Status == "Approved");
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

        public async Task<IActionResult> StudentSubmissionCoordinators(int? id)
        {
            ViewData["Title"] = "List Submission";

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




            return View("coordinators/create_comment", contribution);
        }
        //=============================== POSTS ====================================//
        public IActionResult TablePost()
        {
            ViewData["Title"] = "List of Posts";
            return View("coordinators/table_post");
        }

        public IActionResult FormCreatePost(int? id)
        {
            if (id != null)
            {
                ViewData["Title"] = "Edit Post";
                ViewData["ButtonLabel"] = "Update";
            }
            else
            {
                ViewData["Title"] = "Create Post";
                ViewData["ButtonLabel"] = "Submit";
            }
            Contribution contribution = id != null ? _context.Contributions.Find(id) : new Contribution();
            return View("coordinators/form_create_post", contribution);
        }

        // [HttpPost]
        // public IActionResult CreatePost()
        // {

        // }
        // [HttpPost]
        // public IActionResult UpdatePost()
        // {

        // }

        // [HttpDelete]
        // public IActionResult DeletePost()
        // {

        // }
        //================================ MANAGERS ================================//
        public async Task<IActionResult> IndexManagers(string task, string year)
        {
            ViewData["Title"] = "Dashboard Managers";

            //GET ALL YEARS
            List<int> years = await _context.Contributions
                .Select(c => c.SubmissionDate.Year)
                .Distinct()
                .ToListAsync();

            //GET CONTRIBUTIONS BY YEAR
            int selectedYearAll = DateTime.Now.Year;
            int selectedYearApproved = DateTime.Now.Year;
            int selectedYearRejected = DateTime.Now.Year;
            int selectedYearPending = DateTime.Now.Year;

            if (task == "TotalContribution" && !string.IsNullOrEmpty(year)) { selectedYearAll = Convert.ToInt32(year); }
            List<ContributionDate> allResults = await GetContributionsByStatus(selectedYearAll, "All");

            if (task == "ApprovedContribution" && !string.IsNullOrEmpty(year)) { selectedYearApproved = Convert.ToInt32(year); }
            var approvedResults = await GetContributionsByStatus(selectedYearApproved, "Approved");

            if (task == "RejectedContribution" && !string.IsNullOrEmpty(year)) { selectedYearRejected = Convert.ToInt32(year); }
            var rejectedResults = await GetContributionsByStatus(selectedYearRejected, "Rejected");

            if (task == "PendingContribution" && !string.IsNullOrEmpty(year)) { selectedYearPending = Convert.ToInt32(year); }
            var pendingResults = await GetContributionsByStatus(selectedYearPending, "Pending");

            if (allResults.Count == 0) { allResults.Add(new ContributionDate { Year = int.Parse(year) }); }
            if (approvedResults.Count == 0) { approvedResults.Add(new ContributionDate { Year = int.Parse(year) }); }
            if (rejectedResults.Count == 0) { rejectedResults.Add(new ContributionDate { Year = int.Parse(year) }); }
            if (pendingResults.Count == 0) { pendingResults.Add(new ContributionDate { Year = int.Parse(year) }); }

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
        public async Task<IActionResult> ShowProfileAsync()
        {
            ViewData["Title"] = "Profiles";
            var curUser = await _userManager.GetUserAsync(User);
            if (curUser == null){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            ViewBag.roles = await _userManager.GetRolesAsync(curUser);
            var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.FacultyId == curUser.FacultyId);
            ViewBag.facultyName = faculty.Name;
            return View("profile_managers",curUser);
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

                return RedirectToAction("StudentSubmissionCoordinators");
            }

            return RedirectToAction("StudentSubmissionCoordinators", "Managers");
        }
        [HttpPost]
        public async Task<IActionResult> Publish(int id, bool isPublished)
        {
            var contribution = await _context.Contributions.FindAsync(id);
            if (contribution == null)
            {
                return NotFound();
            }

            try
            {
                contribution.IsPublished = true; 
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Fail");
                return View();
            }
        }
    }
}