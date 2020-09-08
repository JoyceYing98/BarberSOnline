using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberSOnline.Areas.Identity.Data;
using BarberSOnline.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberSOnline.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly UserManager<BarberSOnlineUser> _userManager;
        private readonly RoleManager<BarberSOnlineUser> _roleManager;
        public UserRolesController(UserManager<BarberSOnlineUser> userManager, RoleManager<BarberSOnlineUser> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<ProfileModel>();
            foreach (BarberSOnlineUser user in users)
            {
                var thisViewModel = new ProfileModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(thisViewModel);


            }
            return View(userRolesViewModel);
        }
        private async Task<List<string>> GetUserRoles(BarberSOnlineUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
}

