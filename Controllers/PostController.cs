using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP1640.Models;
using COMP1640.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Drawing.Printing;

namespace COMP1640.Controllers
{
    public class PostController : Controller
    {
        private readonly Comp1640Context _context;
        private readonly UserManager<COMP1640User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(Comp1640Context context, UserManager<COMP1640User> UserManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = UserManager;
            _webHostEnvironment = webHostEnvironment;
        }
        private async Task<string> GetUserFullName()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            return user?.FullName; // This will return null if user is null.
        }
        // GET: Post
        public async Task<IActionResult> Index()
        {
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            var comp1640Context = _context.Posts.Include(p => p.User);
            return View(await comp1640Context.ToListAsync());

        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        public async Task<IActionResult> PostList()
        {
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            var comp1640Context = _context.Posts.Include(p => p.User);
            return View(await comp1640Context.ToListAsync());
        }

        public async Task<IActionResult> PostDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFullName = await GetUserFullName();
            if (userFullName != null)
            {
                ViewBag.userFullName = userFullName;
            }
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            var imagePath = post.ImagePath;
            ViewBag.imagePath = imagePath;

            return View(post);
        }

        // GET: Post/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = _userManager.GetUserId(User);
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Content,PostedAt,UserId")] Post post, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                post.ImageFile = imageFile;
                string uniqueFileName = GetUniqueFileName(post.ImageFile.FileName);
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "postUpload", uniqueFileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await post.ImageFile.CopyToAsync(fileStream);
                }
                post.ImagePath = uniqueFileName;
                _context.Add(post);
                await _context.SaveChangesAsync();
            }
            ViewData["UserId"] = _userManager.GetUserId(User);
            return RedirectToAction("Index");
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = _userManager.GetUserId(User);
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content,PostedAt,UserId")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = _userManager.GetUserId(User);
            return View(post);
        }


        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }
    }
}
