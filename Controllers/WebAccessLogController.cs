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
        public IActionResult AddVisitLog([FromBody] int year)
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

            //crete dictionary to store browser name and count
            var browserCount = new Dictionary<string, int>();
            browserCount.Add(key: "Year", selectedYear);

            //get access count contain Edge
            var Edge = _context.WebAccessLogs.Count(x => x.BrowserName.Contains("Edg") && x.AccessDate.Year == selectedYear);
            if (Edge != 0)
            {
                browserCount.Add("Microsoft Edge", Edge);
            }
            //get access count contain Firefox
            var Firefox = _context.WebAccessLogs.Count(x => x.BrowserName.Contains("Firefox") && x.AccessDate.Year == selectedYear);
            if (Firefox != 0)
            {
                browserCount.Add("Mozilla Firefox", Firefox);
            }
            //get access all browser
            var accessCount = _context.WebAccessLogs.Count(x => x.AccessDate.Year == selectedYear);
            //get access count contain Chrome
            var Chrome = accessCount - Edge - Firefox;   
            if (Chrome != 0)
            {
                browserCount.Add("Google Chrome", Chrome);
            }

            return Ok(browserCount);
        } 
    
        [HttpPost]
        public IActionResult PageVisitData([FromBody] string action)
        {
            Console.WriteLine("Action: " + action);
            List<PageVisit> pageVisits = new List<PageVisit>();

            if (action == "asc")
            {
                pageVisits = _context.PageVisits.OrderBy(p => p.VisitCount).ToList();
            }
            else
            {
                pageVisits = _context.PageVisits.OrderByDescending(p => p.VisitCount).ToList();
            }

            return Ok(pageVisits);
            
        }
    }
}
