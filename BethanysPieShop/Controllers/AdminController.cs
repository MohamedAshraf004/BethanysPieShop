using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BethanysPieShop.Auth;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BethanysPieShop.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager,
                                        RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
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
            var user = new ApplicationUser()
            {
                    UserName = userViewModel.Email,
                    Email = userViewModel.Email,
                    City=userViewModel.City,
                    Country=userViewModel.Country,
                    Birthdate=userViewModel.Birthdate
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
            var userViewModel = new EditUserViewModel() { Id = user.Id, Email = user.Email,
                                            Birthdate = user.Birthdate, City = user.City, Country = user.Country };

            return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel userViewModel)
        {
            
            var user = await _userManager.FindByIdAsync(userViewModel.Id);
            if (user!=null)
            {
                user.Email = userViewModel.Email;
                user.UserName = userViewModel.Email;
                user.Country = userViewModel.Country;
                user.City = userViewModel.City;
                user.Birthdate = userViewModel.Birthdate;
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

        public IActionResult RoleManagement()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public  IActionResult AddNewRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewRole(AddRoleViewModel addRoleViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalide input");
                return View(addRoleViewModel);
            }
            var role = new IdentityRole() { Name = addRoleViewModel.RoleName };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(RoleManagement), _roleManager.Roles);
            }
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(addRoleViewModel);
        }
        public async Task<IActionResult> EditRole(string id)
        {
            var role =await _roleManager.FindByIdAsync(id);
            if (role==null)
            {
                return RedirectToAction(nameof(RoleManagement), _roleManager.Roles);
            }
            var editRoleViewModel = new EditRoleViewModel() 
            {
                Id=role.Id,
                RoleName=role.Name,
                Users=new List<string>()
            };
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                    editRoleViewModel.Users.Add(user.UserName);               
            }
            return View(editRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel editRoleViewModel)
        {
            var role = await _roleManager.FindByIdAsync(editRoleViewModel.Id);
            if (role==null)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }
            role.Name = editRoleViewModel.RoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(RoleManagement), _roleManager.Roles);
            }
            ModelState.AddModelError("", "Something went wrong while update");
            return View(editRoleViewModel);
        }

        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role ==null)
            {
                ModelState.AddModelError("", "This role can't be found.");
                return View("RoleManagement", _roleManager.Roles);
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(RoleManagement), _roleManager.Roles);
            }
            ModelState.AddModelError("", "Something went wrong while deleting");
            return View(nameof(RoleManagement), _roleManager.Roles);
        }


        public async Task<IActionResult> AddUserToRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return RedirectToAction("RoleManagement", _roleManager.Roles);

            UserRoleViewModel addUserToRoleViewModel = new UserRoleViewModel { RoleId = role.Id };
            foreach (var user in _userManager.Users)
            {
                if (!await _userManager.IsInRoleAsync(user, role.Name))
                    addUserToRoleViewModel.Users.Add(user);                
            }
            return View(addUserToRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(UserRoleViewModel userRoleViewModel)
        {
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId);
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
            if (user==null || role==null)
            {
                return RedirectToAction(nameof(RoleManagement), _roleManager.Roles);
            }
            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userRoleViewModel);
        }

        public async Task<IActionResult> DeleteUserFromRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return RedirectToAction("RoleManagement", _roleManager.Roles);

            var addUserToRoleViewModel = new UserRoleViewModel { RoleId = role.Id };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user,role.Name))
                {
                    addUserToRoleViewModel.Users.Add(user);
                }
            }
            return View(addUserToRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserFromRole(UserRoleViewModel userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId);

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userRoleViewModel);
        }

    }
}
