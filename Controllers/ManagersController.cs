using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using COMP1640.Models;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Controllers
{
    public class ManagersController : Controller
    {
        private readonly Comp1640Context _context;

        public ManagersController(Comp1640Context context)
        {
            _context = context;
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
        //================================ PROFILES ================================//
        public IActionResult ShowProfile()
        {
            ViewData["Title"] = "Profiles";
            return View("profile_managers");
        }
    }
}