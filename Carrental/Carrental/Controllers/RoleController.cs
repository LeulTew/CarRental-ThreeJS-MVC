using Carrental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Carrental.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [Authorize(Roles = ("Admin"))]
        public IActionResult Index()
        {
            return View(roleManager.Roles);
        }

        [Authorize(Roles = ("Admin"))]
        public IActionResult Create() => View();
        [Authorize(Roles = ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View(name);
        }

        [HttpPost, Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
                ModelState.AddModelError("", "No role found");
            return View("Index", roleManager.Roles);
        }

        [HttpGet]
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Update(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();
            foreach (AppUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                var usersInRole = await userManager.GetUsersInRoleAsync(model.RoleName);
                var usersInRoleIds = usersInRole.Select(u => u.Id).ToList();

                var addIds = model.AddIds ?? new string[] { };
                if (addIds != null)
                {
                    foreach (string userId in addIds)
                    {
                        if (!usersInRoleIds.Contains(userId))
                        {
                            AppUser user = await userManager.FindByIdAsync(userId);
                            if (user != null)
                            {
                                result = await userManager.AddToRoleAsync(user, model.RoleName);
                                if (!result.Succeeded)
                                {
                                    foreach (IdentityError error in result.Errors)
                                    {
                                        ModelState.AddModelError(string.Empty, error.Description);
                                    }
                                }
                            }
                        }
                    }
                }

                string[] deleteIds = model.DeleteIds ?? new string[] { };

                foreach (var user in usersInRole)
                {
                    if (!addIds.Contains(user.Id) && !deleteIds.Contains(user.Id))
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (IdentityError error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                }
            }

            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
            {
                return await Update(model.RoleId);
            }
        }
    }
}
