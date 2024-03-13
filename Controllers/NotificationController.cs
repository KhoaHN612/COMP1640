using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    public class NotificationController : Controller
    {
        private readonly Comp1640Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NotificationController(Comp1640Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingContributionCount()
        {
            var count = await _context.Contributions.CountAsync(c => c.Status == "Pending");
            return Json(count);
        }
        [HttpGet]
        public IActionResult GetPendingContributions()
        {
            var pendingContributions = _context.Contributions.Where(c => c.Status == "Pending").ToList(); // Lọc các đóng góp có trạng thái là "Pending"
            return Json(pendingContributions); // Trả về danh sách dưới dạng JSON
        }
    }
}
