using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberSOnline.Data;
using BarberSOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberSOnline.Controllers
{
    public class AdminController : Controller
    {
        private readonly BarberSOnlineContext _context;
        private readonly IAzureBlobService _azureBlobService;

        public AdminController(BarberSOnlineContext context, IAzureBlobService azureBlobService)
        {
            _context = context;
            _azureBlobService = azureBlobService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Report()
        {
           
            return View(await _context.UserModel.ToListAsync());


        }

        public async Task<ActionResult> MySejateraReport()
        {
            try
            {
                var allBlobs = await _azureBlobService.ListAsync();
                return View(allBlobs);
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
                return RedirectToAction("MySejateraReport");
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
                return RedirectToAction("MySejateraReport");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

    }
}
