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
        private readonly SignInManager<BarberSOnlineUser> _signInManager;

        public AppointmentController(BarberSOnlineContext context, UserManager<BarberSOnlineUser> barberManager, SignInManager<BarberSOnlineUser> signInManager)
        {
            _context = context;
            _barberManager = barberManager;
            _signInManager = signInManager;
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
        public async Task<IActionResult> Create([Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,User_Booked_Date")] AppointmentModel appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    appointment.User_Booked_Date = now;
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();//save appointment and respective services record
                    return RedirectToAction("UserAppointment");
                }
                catch(Exception e)
                {
                    ViewBag.msg = "Error: " + e.ToString();
                }
            }
            return View("~/Views/Shared/_LayoutUser");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ViewAll()
        {
            return View(await _context.AppointmentModel.ToListAsync());
        }

        [Authorize(Roles = "Barber")]
        public async Task<IActionResult> GenerateReceipt(int? AppointmentId)
        {
            if (AppointmentId == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel
                .FirstOrDefaultAsync(m => m.ID == AppointmentId);

            if (appointmentModel.Appointment_Status == "Confirmed")
            {
                appointmentModel.Appointment_Status = "Paid";
                _context.Update(appointmentModel);
                await _context.SaveChangesAsync();
            }

            else
            {
                ViewBag.ErrorMessage = "Payment is not pending!";
                return RedirectToAction("ViewAll");
            }

            if (appointmentModel == null)
            {
                return NotFound();
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

        [Authorize(Roles = "User")]
        public async Task<IActionResult> ConfirmPayment(int? userappointmentID)
        {
            if (userappointmentID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel
                .FirstOrDefaultAsync(m => m.ID == userappointmentID);

            if(appointmentModel.Appointment_Status == "PendingPayment")
            {
                appointmentModel.Appointment_Status = "Paid";
                _context.Update(appointmentModel);
                await _context.SaveChangesAsync();
            }
            else
            {
                ViewBag.ErrorMessage = "Payment is not pending!";
                return RedirectToAction("UserAppointment");
            }

            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        public async Task<IActionResult> UserEdit(int? AppointmentId)
        {
            if (AppointmentId == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FindAsync(AppointmentId);
            if (appointmentModel == null)
            {
                return NotFound();
            }
            return View(appointmentModel);
        }

        // POST: UserModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(int AppointmentId, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,User_Booked_Date,Remark,User_Confirmed_Date")] AppointmentModel appointmentModel)
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
                    if (!AppointmentIDExists(appointmentModel.ID))
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

        // GET: UserModels/Edit/5
        public async Task<IActionResult> Edit(int? AppointmentId)
        {
            if (AppointmentId == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FindAsync(AppointmentId);
            if (appointmentModel == null)
            {
                return NotFound();
            }
            return View(appointmentModel);
        }

        // POST: UserModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int AppointmentId, 
            [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,Remark," +
            "User_Booked_Date,User_Confirmed_Date,User_Cancelled_Reason," +
            "Barber_Confirmed_Date,Barber_Check_In_Date,Barber_Cancelled_Reason,AdminEmail,Admin_Cancelled_Reason")] 
            AppointmentModel appointmentModel)
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
                    if (!AppointmentIDExists(appointmentModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var user = await _signInManager.UserManager.FindByEmailAsync(appointmentModel.UserEmail);
                var roles = await _signInManager.UserManager.GetRolesAsync(user);


                if (roles.Any())
                {
                    if (roles.First().Equals("Admin"))
                    {
                        return LocalRedirect("~/Admin/Index");
                    }
                    if (roles.First().Equals("Barber"))
                    {
                        return RedirectToAction(nameof(ViewAll));
                    }
                    if (roles.First().Equals("User"))
                    {
                        return RedirectToAction(nameof(UserAppointment));
                    }
                }
                
            }
            return View(appointmentModel);
        }

        // GET: AppointmentModel/Details/5
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
