using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BethanysPieShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UserManagement()
        {
            var users = _userManager.Users;

            return View(users);            
        }
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel userViewModel)
        {
            if (!ModelState.IsValid) return View(userViewModel);
            var user = new IdentityUser()
            {
                    UserName = userViewModel.UserName,
                    Email = userViewModel.Email
            };
            IdentityResult result =await  _userManager.CreateAsync(user, userViewModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(UserManagement), _userManager.Users);
            }
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(userViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return RedirectToAction(nameof(UserManagement), _userManager.Users);
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(string id,string Email ,string UserName)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user!=null)
            {
                user.Email = Email;
                user.UserName = UserName;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserManagement", _userManager.Users);
                }
                ModelState.AddModelError("", "User not updated, something went wrong.");

                return View(user);
            }
            return RedirectToAction("UserManagement", _userManager.Users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                ModelState.AddModelError("", "This user can't be found.");              
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(UserManagement));
            }
            ModelState.AddModelError("", "Something went wrong while deleting this user.");
            return View(nameof(UserManagement),_userManager.Users);
        }

    }
}
