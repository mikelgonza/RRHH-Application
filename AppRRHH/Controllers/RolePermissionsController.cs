using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppRRHH.Data;
using AppRRHH.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AppRRHH.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolePermissionsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RolePermissionsController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<AppUser> usuarios = await _userManager.Users.ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> AddRole(string id, string role)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            // Remove all roles of user
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            // Add new role to user
            await _userManager.AddToRoleAsync(user, role);
            return RedirectToAction(nameof(Index));
        }
    }
}
