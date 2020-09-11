using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberSOnline.Areas.Identity.Data;
using BarberSOnline.Data;
using BarberSOnline.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberSOnline.Controllers
{
    public class TryAppointmentController : Controller
    {
        private readonly BarberSOnlineContext _context;
        private readonly UserManager<BarberSOnlineUser> _barberManager;

        public TryAppointmentController(BarberSOnlineContext context, UserManager<BarberSOnlineUser> barberManager)
        {
            _context = context;
            _barberManager = barberManager;
        }

        public string Username { get; set; }

        private async Task LoadAsync(BarberSOnlineUser barber)
        {
            var userName = await _barberManager.GetUserNameAsync(barber);
            Username = userName;
        }

        // GET: Appointment/Create
        public IActionResult Create()
        {
            ViewBag.UserId = User.Identity.Name;
            return View();//first start without user request
        }

        [HttpPost]//this action is for user after key in and submit
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserEmail,Status,User_Confirmed_Date")] TryAppointment appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();//save appointment and respective services record
                    return RedirectToAction("Create");
                }
                catch (Exception e)
                {
                    ViewBag.msg = "Error: " + e.ToString();
                }
            }
            return View("~/Shared/_LayoutBarber");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
