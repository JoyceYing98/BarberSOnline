using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberSOnline.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberSOnline.Controllers
{
    public class AdminController : Controller
    {
        private readonly BarberSOnlineContext _context;

        public AdminController(BarberSOnlineContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Report()
        {
           
            return View(await _context.UserModel.ToListAsync());


        }
    }
}
