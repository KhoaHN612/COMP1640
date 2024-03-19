using COMP1640.Areas.Identity.Data;
using COMP1640.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    public class NotificationController : Controller
    {
        private readonly Comp1640Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<COMP1640User> _userManager;
        public NotificationController(Comp1640Context context, IWebHostEnvironment webHostEnvironment, UserManager<COMP1640User> UserManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = UserManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingContributionCount()
        {
            var user = await _userManager.GetUserAsync(User);

            var count = await _context.Contributions
                                .Join(_context.Users,
                                    c => c.UserId,
                                    u => u.Id,
                                    (c, u) => new { Contribution = c, User = u })
                                .Where(cu => cu.Contribution.Status == "Pending" &&
                                            cu.User.FacultyId == user.FacultyId)
                                .CountAsync();
            return Json(count);
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingContributionsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var pendingContributions = await _context.Contributions
                                    .Join(_context.Users,
                                        c => c.UserId,
                                        u => u.Id,
                                        (c, u) => new { Contribution = c, User = u })
                                    .Where(cu => cu.Contribution.Status == "Pending" &&
                                                cu.User.FacultyId == user.FacultyId)
                                    .Select(cu => cu.Contribution)
                                    .ToListAsync();
            return Json(pendingContributions);
        }
    }
}
