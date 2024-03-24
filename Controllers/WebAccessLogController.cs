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
        public IActionResult AddVisitLog()
        {
            // Lấy thông tin trình duyệt của người dùng từ User-Agent header
            var browserName = Request.Headers["User-Agent"].ToString();

            Console.WriteLine(browserName);

            // Lưu thông tin vào bảng VisitLogs
            var visitLog = new WebAccessLog
            {
                BrowserName = browserName,
                AccessDate = DateTime.Now
            };

            //print out the WebAccessLog object
            foreach (var prop in visitLog.GetType().GetProperties())
            {
                Console.WriteLine($"{prop.Name} = {prop.GetValue(visitLog, null)}");
            }

            _context.WebAccessLogs.Add(visitLog);
            _context.SaveChanges();

            return Ok();
        }
    }
}
