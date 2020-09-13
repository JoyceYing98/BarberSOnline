using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarberSOnline.Areas.Identity.Data;
using BarberSOnline.Data;
using BarberSOnline.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;

namespace BarberSOnline.Controllers
{
    public class AppointmentController : Controller
    {

        private readonly BarberSOnlineContext _context;
        private readonly UserManager<BarberSOnlineUser> _barberManager;
        private readonly SignInManager<BarberSOnlineUser> _signInManager;
        const string ServiceBusConnectionString = "Endpoint=sb://barbersonlineservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=0DpfJH+V2QR12KRbvFBIwpg3w5x2BWjslvDRb282/0U=";
        const string QueueName = "appointmentqueue";
        static IQueueClient queueClient;
        static List<string> items;


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

        public IActionResult Index()
        {
            return View();
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
                    DateTime now = DateTime.Now;
                    appointment.User_Booked_Date = now;
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();//save appointment and respective services record
                    //return RedirectToAction("UserAppointment");
                }
                catch(Exception e)
                {
                    ViewBag.msg = "Error: " + e.ToString();
                }

                try
                {
                    queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
                    // Create a new message to send to the queue.
                    string messageBody = $"Message: {appointment.UserEmail} is requesting appointment on {appointment.Appointment_Date}.";
                        var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                        // Write the body of the message to the console.
                        Console.WriteLine($"Sending message: {messageBody}");

                        // Send the message to the queue.
                        await queueClient.SendAsync(message);
                        ViewBag.msg = "success";
                }
                catch (Exception exception)
                {
                    ViewBag.msg = exception.ToString();
                }
            }
            return View();
        }

        public async Task<IActionResult> ViewAll()
        {
            return View(await _context.AppointmentModel.ToListAsync());
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

        public async Task<IActionResult> UserEdit(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FindAsync(ID);
            if (appointmentModel.Appointment_Status != "Booked")
            {
                ViewBag.ErrorMessage = "Appointment Status is " + appointmentModel.Appointment_Status + "! You cannot make changes anymore. ";
            }

            if (appointmentModel == null)
            {
                return NotFound();
            }
            return View(appointmentModel);
        }

        // POST: Appointment/UserEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(int ID, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,User_Booked_Date")] AppointmentModel appointmentModel)
        {
            if (ID != appointmentModel.ID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if(appointmentModel.Appointment_Status == "Approved")
                    {
                        appointmentModel.Appointment_Status = "Confirmed";
                        DateTime now = DateTime.Now;
                        appointmentModel.User_Confirmed_Date = now;
                    }
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

        public async Task<IActionResult> Cancel(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FindAsync(ID);
            if (appointmentModel.Appointment_Status != "Booked" && appointmentModel.Appointment_Status != "Approved" && appointmentModel.Appointment_Status != "Confirmed")
            {
                ViewBag.ErrorMessage = "You cannot cancel the appointment because the appointment status is " + appointmentModel.Appointment_Status + "!";
            }

            if (appointmentModel == null)
            {
                return NotFound();
            }
            return View(appointmentModel);
        }

        // POST: Appointment/Cancel/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int ID, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,User_Booked_Date,User_Cancelled_Reason,BarberEmail,Barber_Cancelled_Reason,AdminEmail,Admin_Cancelled_Reason,Appointment_Status")] AppointmentModel appointmentModel)
        {
            if (ID != appointmentModel.ID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    appointmentModel.Appointment_Status = "Cancelled";
                    if (appointmentModel.Admin_Cancelled_Reason != null)
                    {
                        appointmentModel.AdminEmail = User.Identity.Name;
                    }
                    else if (appointmentModel.Barber_Cancelled_Reason != null)
                    {
                        appointmentModel.BarberEmail = User.Identity.Name;
                    }
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
                    if (roles.First().Equals("Admin") || roles.First().Equals("Barber"))
                    {
                        return RedirectToAction(nameof(ViewAll));
                    }
                    else if (roles.First().Equals("User"))
                    {
                        return RedirectToAction(nameof(UserAppointment));
                    }
                }

                return RedirectToAction(nameof(UserAppointment));
            }
            return View(appointmentModel);
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int? AppointmentId)
        {
            if (AppointmentId == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FirstOrDefaultAsync(m => m.ID == AppointmentId);
            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        public async Task<IActionResult> BarberApproval(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FindAsync(ID);
            if (appointmentModel.Appointment_Status != "Booked")
            {
                ViewBag.ErrorMessage = "Appointment Status is " + appointmentModel.Appointment_Status + "! You cannot approve anymore. ";
            }

            if (appointmentModel == null)
            {
                return NotFound();
            }
            return View(appointmentModel);
        }

        // POST: Appointment/BarberApproval/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BarberApproval(int ID, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,Remark,User_Booked_Date")] AppointmentModel appointmentModel)
        {
            if (ID != appointmentModel.ID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    appointmentModel.Appointment_Status = "Approved";
                    DateTime now = DateTime.Now;
                    appointmentModel.Barber_Approved_Date = now;
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
                return RedirectToAction(nameof(ViewAll));
            }
            return View(appointmentModel);
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
            }

            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
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

            if (appointmentModel.Appointment_Status == "PendingPayment")
            {
                appointmentModel.Appointment_Status = "Paid";
                _context.Update(appointmentModel);
                await _context.SaveChangesAsync();
            }
            else
            {
                ViewBag.ErrorMessage = "Payment is not pending!";
            }

            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
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
