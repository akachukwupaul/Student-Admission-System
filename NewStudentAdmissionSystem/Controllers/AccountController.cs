using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewStudentAdmissionSystem.Models;
using NewStudentAdmissionSystem.ViewModels;

namespace NewStudentAdmissionSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);

            // Core sign-in operation: attempts to sign the user in using their credentials.
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (await userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("AdminHome", "Admin");
                else if (await userManager.IsInRoleAsync(user, "User"))
                    return RedirectToAction("Dashboard", "Student");
            }
            ModelState.AddModelError(string.Empty, "Invalid Login attempt");
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Creates a new instance of the custom user entity (Users) and maps data from the ViewModel.
            var user = new Users
            {
                FullName = model.Name,
                UserName = model.Email,
                NormalizedUserName = model.Email.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper()
            };
            
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleExist = await roleManager.RoleExistsAsync("User");
                if (!roleExist)
                {
                    var role = new IdentityRole("User");
                    await roleManager.CreateAsync(role);
                }
               
                await userManager.AddToRoleAsync(user, "User");

                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Account");
            }
           
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "user not found");
                return View(model);
            }
            else
            {
                return RedirectToAction("ChangePassword", "Account", new { Username = user.UserName });
            }
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "something went wrong");
                return View(model);
            }
            // Retrieves the user object from the database using their email/username.
            var user = await userManager.FindByNameAsync(model.Email);
             if (user == null)
             {
                ModelState.AddModelError("", "User not found");
                return View(model);
             }
            // Removes the user's existing password hash from the database.
            var result = await userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddPasswordAsync(user, model.NewPassword);
                return RedirectToAction("Login","Account");
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
      
    } 
}
