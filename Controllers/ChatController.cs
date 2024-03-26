using COMP1640.Areas.Identity.Data;
using COMP1640.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Controllers
{
    public class ChatController : Controller
    {
        private readonly Comp1640Context _context;
        private readonly UserManager<COMP1640User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ChatController> _logger;

        public ChatController(Comp1640Context context, ILogger<ChatController> logger,
        UserManager<COMP1640User> UserManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = UserManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        [Authorize]
        // GET: ChatController
        public ActionResult Index()
        {
            return RedirectToAction(nameof(Find));
        }
        [Authorize]
        public IActionResult Find()
        {
            ViewData["Title"] = "User Table page";
            var users = _userManager.Users.Include(u => u.Faculty).ToList();
            return View(users);
        }
        [Authorize]
        public async Task<IActionResult> Chatwith(string Id)
        {
            var OrUserId = Id;
            if (OrUserId == null)
            {
                return RedirectToAction("Find");
            }
            var curUserId = _userManager.GetUserId(User);

            ViewBag.receiver = await _userManager.FindByIdAsync(OrUserId); 
            ViewBag.sender = await _userManager.FindByIdAsync(curUserId);

            var chatId = await _context.UserChat
                .AsNoTracking()
                .Where(uc => uc.UserId == curUserId || uc.UserId == OrUserId)
                .GroupBy(uc => uc.ChatId)
                .Where(g => g.Count() == 2)
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (chatId != 0)
            {
                // If a chat is found, return it
                var chat = await _context.Chats
                    .Include(c => c.Users)
                    .ThenInclude(uc => uc.User)
                    .Include(c => c.Messages)
                    .ThenInclude(m => m.User)
                    .FirstOrDefaultAsync(c => c.Id == chatId);

                return View(chat);
            }
            else
            {
                // No chat found, create a new one
                var newChat = new Chat
                {
                    UpdateAt = DateTime.Now,
                    Users = new List<UserChat>
                {
                    new UserChat { UserId = curUserId },
                    new UserChat { UserId = OrUserId }
                }
                };

                _context.Chats.Add(newChat);
                await _context.SaveChangesAsync();

                return View(newChat);
            }
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> CreateMessage(int ChatId, string SenderId, string Content)
        // {
        //     var Message = new COMP1640.Models.Message
        //     {
        //         Content = Content,
        //         SentAt = DateTime.Now,
        //         ChatId = ChatId,
        //         UserId = SenderId
        //     };
        //     _context.Messages.Add(Message);
        //     var result = await _context.SaveChangesAsync();

        //     return Ok();
        // }

        // GET: ChatController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ChatController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChatController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ChatController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChatsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ChatsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChatsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
