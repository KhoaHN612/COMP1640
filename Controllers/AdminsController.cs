using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP1640.Areas.Identity.Data;
using COMP1640.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace COMP1640
{
    public class AdminsController : Controller
    {
        private readonly Comp1640Context _context;
        private readonly UserManager<COMP1640User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<COMP1640User> _userStore;
        private readonly IUserEmailStore<COMP1640User> _emailStore;
        private readonly IEmailSenderCustom _emailSender;
        private readonly ILogger<AdminsController> _logger;

        public AdminsController(Comp1640Context context, ILogger<AdminsController> logger,
        UserManager<COMP1640User> UserManager, RoleManager<IdentityRole> roleManager, 
        IUserStore<COMP1640User> userStore, IEmailSenderCustom EmailSender)
        {
            _context = context;
            _logger = logger;
            _userManager = UserManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _emailStore = GetEmailStore();
            _emailSender = EmailSender;
        }

        private IUserEmailStore<COMP1640User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<COMP1640User>)_userStore;
        }
        //================================ ADMIN ================================//
        public async Task<IActionResult> CreateRole()
        {
            string[] roleNames = { "Guest", "Manager", "Coordinator", "Student", "Admin" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            return RedirectToAction("TableUser");
        }

        public IActionResult TestEmail()
        {
            var message = new Message(new string[] { "comp1640k@gmail.com" }, "Test async email", "This is the content from our async email.");
            _emailSender.SendEmail(message);
            return RedirectToAction("TableUser");
        }

        public IActionResult TableUser()
        {
            ViewData["Title"] = "User Table page";
            var users = _userManager.Users.ToList();
            ViewBag.Context = _context;
            return View("table_user", users);
        }
        public IActionResult FormCreateUser()
        {
            ViewData["Title"] = "Create User page";
            ViewBag.Roles = _roleManager.Roles.ToList();
            var faculties = _context.Faculties.ToList();
            if (faculties != null && faculties.Any())
            {
                ViewBag.FacultyId = new SelectList(faculties, "FacultyId", "Name");
            }
            else
            {
                ViewBag.FacultyId = new SelectList(new List<Faculty>(), "FacultyId", "Name");
            }
            return View("form_create_user");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormCreateUser(COMP1640User model, string Role, string Password)
        {
            if (ModelState.IsValid)
            {
                // Create a new IdentityUser instance
                var user = new COMP1640User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    ProfileImagePath = model.ProfileImagePath,
                    PhoneNumber = model.PhoneNumber,
                    DayOfBirth = model.DayOfBirth,
                    Address = model.Address,
                    FacultyId = model.FacultyId,
                };

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password." + Password);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                    var resultEmail = await _userManager.ConfirmEmailAsync(user, code);

                    // Find the role by its ID
                    var selectedRole = await _roleManager.FindByIdAsync(Role);

                    if (selectedRole != null)
                    {
                        // Assign the user to the selected role
                        await _userManager.AddToRoleAsync(user, selectedRole.Name);
                    }

                    // Redirect to a success page or return a success message
                    return RedirectToAction("TableUser");
                }
                else
                {
                    // If creation of user fails, add errors to model state
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // return RedirectToAction("FormCreateUser");
            // If ModelState is invalid, return to the create user form with errors
            ViewData["Title"] = "Create User page";
            ViewBag.Roles = _roleManager.Roles.ToList();
            var faculties = _context.Faculties.ToList();
            if (faculties != null && faculties.Any())
            {
                ViewBag.FacultyId = new SelectList(faculties, "FacultyId", "Name");
            }
            else
            {
                ViewBag.FacultyId = new SelectList(new List<Faculty>(), "FacultyId", "Name");
            }
            return View("form_create_user", model);
        }
    }
}
