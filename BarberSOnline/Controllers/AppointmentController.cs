using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BarberSOnline.Areas.Identity.Data;
using BarberSOnline.Data;
using BarberSOnline.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberSOnline.Controllers
{
    public class AppointmentController : Controller
    {

        private readonly BarberSOnlineContext _context;
        private readonly UserManager<BarberSOnlineUser> _barberManager;

        public AppointmentController(BarberSOnlineContext context, UserManager<BarberSOnlineUser> barberManager)
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
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            ViewBag.UserId = User.Identity.Name;
            return View();//first start without user request
        }

        // POST: Appointment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //POST & GET - method to transmit data
        [HttpPost]//this action is for user after key in and submit
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status")] AppointmentModel appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();//save appointment and respective services record
                    return RedirectToAction("Create");
                }
                catch(Exception e)
                {
                    ViewBag.msg = "Error: " + e.ToString();
                }
            }
            return RedirectToAction("Create");
            //return View(appointment);
        }

        //// GET: AppointmentModels
        ////store to a list if found a username
        ////if list not equal null and display the list
        //public async Task<IActionResult> List()
        //{
        //    List<AppointmentModel> umlist = new List<AppointmentModel>();
        //    var userModel = await _context.UserModel.ToListAsync();
        //    foreach (UserModel user in userModel)
        //    {
        //        if (user.Username == User.Identity.Name)
        //        {
        //            umlist.Add(user);
        //        }
        //    }
        //    if (umlist != null)
        //    {

        //        return View(umlist);
        //    }
        //    return NotFound();
        //}

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ViewAll()
        {
            return View(await _context.AppointmentModel.ToListAsync());
        }

        public async Task<IActionResult> GenerateReceipt(int? AppointmentId)
        {
            if (AppointmentId == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel
                .FirstOrDefaultAsync(m => m.ID == AppointmentId);
            appointmentModel.Appointment_Status = "PendingPayment";
 
            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateReceipt(int AppointmentId, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status")] AppointmentModel appointmentModel)
        {
            if (AppointmentId != appointmentModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointmentModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentIDExists(AppointmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewAll));
            }
            return View(appointmentModel);
        }

        public async Task<IActionResult> UserAppointment()
        {
            List<AppointmentModel> UserAppointment = new List<AppointmentModel>();
            var appointmentModel = await _context.AppointmentModel.ToListAsync();
            foreach (AppointmentModel user in appointmentModel)
            {
                if (user.UserEmail == User.Identity.Name)
                {
                    UserAppointment.Add(user);
                }
            }
            if (UserAppointment != null)
            {
                return View(UserAppointment);
            }
            return NotFound();
        }

        //[Authorize(Roles = "User")]
        public async Task<IActionResult> ConfirmPayment(int? userappointmentID)
        {
            if (userappointmentID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel
                .FirstOrDefaultAsync(m => m.ID == userappointmentID);
            appointmentModel.Appointment_Status = "Paid";

            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int userappointmentID, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status")] AppointmentModel appointmentModel)
        {
            if (userappointmentID != appointmentModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointmentModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentIDExists(userappointmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserAppointment));
            }
            return View(appointmentModel);
        }

        // GET: UserModels/Details/5
        public async Task<IActionResult> Details(int? AppointmentId)
        {
            if (AppointmentId == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel
                .FirstOrDefaultAsync(m => m.ID == AppointmentId);
            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        // GET: UserModels/Delete/5
        public async Task<IActionResult> Delete(int? AppointmentId)
        {
            if (AppointmentId == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel
                .FirstOrDefaultAsync(m => m.ID == AppointmentId);
            if (AppointmentId == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        // POST: UserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int AppointmentId)
        {
            var appointmentModel = await _context.AppointmentModel.FindAsync(AppointmentId);
            _context.AppointmentModel.Remove(appointmentModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewAll));
        }

        private bool AppointmentModelExists(string UserEmail)
        {
            return _context.AppointmentModel.Any(e => e.UserEmail == UserEmail);
        }

        private bool AppointmentIDExists(int appointmentId)
        {
            return _context.AppointmentModel.Any(e => e.ID == appointmentId);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
