using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarberSOnline.Data;
using BarberSOnline.Models;
using BarberSOnline.Services;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BarberSOnline.Areas.Identity.Data;

namespace BarberSOnline.Views.User
{
    public class UserModelsController : Controller
    {
        private readonly BarberSOnlineContent _context;
        private readonly IAzureBlobService _azureBlobService;
        private readonly UserManager<BarberSOnlineUser> _userManager;


        public UserModelsController(BarberSOnlineContent context, IAzureBlobService azureBlobService, UserManager<BarberSOnlineUser> userManager)
        {
            _context = context;
            _azureBlobService = azureBlobService;
            _userManager = userManager;
        }

        public string Username { get; set; }

        private async Task LoadAsync(BarberSOnlineUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;
        }

        



    // GET: UserModels
    public async Task<IActionResult> List()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            

                return View(await _context.UserModel.ToListAsync());

            

            

                // return View(await _context.UserModel.ToListAsync());

            }

            // GET: UserModels/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userModel = await _context.UserModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (userModel == null)
            {
                return NotFound();
            }

            return View(userModel);
        }

        [HttpGet]
        // GET: UserModels/Create
        public IActionResult Create()
        {
            ViewBag.UserId = User.Identity.Name;
            return View();
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadAsynce()
        {
            try
            {
                var request = await HttpContext.Request.ReadFormAsync();
                if (request.Files == null)
                {
                    return BadRequest("Could not upload files");
                }
                var files = request.Files;
                if (files.Count == 0)
                {
                    return BadRequest("Could not upload empty files");
                }

                await _azureBlobService.UploadAsynce(files);
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteImage(string fileUri)
        {
            try
            {
                await _azureBlobService.DeleteAsync(fileUri);
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAll()
        {
            try
            {
                await _azureBlobService.DeleteAllAsync();
                return RedirectToAction("Message");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        public string UserId { get; set; }
        public void OnGet()
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // POST: UserModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Username,Temperature,Status,Visit")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(userModel);
        }

        // GET: UserModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userModel = await _context.UserModel.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }
            return View(userModel);
        }

        // POST: UserModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Username,Temperature,Status,Visit")] UserModel userModel)
        {
            if (id != userModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserModelExists(userModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            return View(userModel);
        }

        // GET: UserModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userModel = await _context.UserModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (userModel == null)
            {
                return NotFound();
            }

            return View(userModel);
        }

        // POST: UserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userModel = await _context.UserModel.FindAsync(id);
            _context.UserModel.Remove(userModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private bool UserModelExists(int id)
        {
            return _context.UserModel.Any(e => e.ID == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
