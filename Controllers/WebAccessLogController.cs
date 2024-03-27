using System;
using Microsoft.AspNetCore.Mvc;
using COMP1640.Models;

namespace COMP1640.Controllers
{
    public class WebAccessLogController : Controller
    {
        private readonly Comp1640Context _context;

        public WebAccessLogController(Comp1640Context context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddVisitLog(int year)
        {
            
            // Lấy thông tin trình duyệt của người dùng từ User-Agent header
            var browserName = Request.Headers["User-Agent"].ToString();

            // Lưu thông tin vào bảng VisitLogs
            var visitLog = new WebAccessLog
            {
                BrowserName = browserName,
                AccessDate = DateTime.Now
            };

            _context.WebAccessLogs.Add(visitLog);
            _context.SaveChanges();

            int selectedYear = DateTime.Now.Year;
            if (year != 0)
            {       
                selectedYear = year;
            }

            //get access count contain Edge
            var Edge = _context.WebAccessLogs.Count(x => x.BrowserName.Contains("Edg") && x.AccessDate.Year == selectedYear);
            //get access count contain Firefox
            var Firefox = _context.WebAccessLogs.Count(x => x.BrowserName.Contains("Firefox") && x.AccessDate.Year == selectedYear);
            //get access all browser
            var accessCount = _context.WebAccessLogs.Count(x => x.AccessDate.Year == selectedYear);
            //get access count contain Chrome
            var Chrome = accessCount - Edge - Firefox;           

            return Ok(new { Edge, Firefox, Chrome});
        } 
    }
}
