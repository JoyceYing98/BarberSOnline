﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BarberSOnline.Areas.Identity.Data;
using BarberSOnline.Data;
using BarberSOnline.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
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
            ManagementClient managementClient1 = new ManagementClient(ServiceBusConnectionString);//retrieve connection string
            var managementClient = managementClient1;
            var runtimeInfo = await managementClient.GetQueueRuntimeInfoAsync(QueueName);//retrieve queue name created

            //will be added whenever a new appointment is created
            var messagesInQueueCount = runtimeInfo.MessageCountDetails.ActiveMessageCount;

            ViewBag.MessageCount = messagesInQueueCount;
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
            if (UserAppointment == null)
            {
                return NotFound();
            }

            ManagementClient managementClient1 = new ManagementClient(ServiceBusConnectionString);
            var managementClient = managementClient1;
            var runtimeInfo = await managementClient.GetQueueRuntimeInfoAsync(QueueName);

            var messagesInQueueCount = runtimeInfo.MessageCountDetails.ActiveMessageCount;

            ViewBag.MessageCount = messagesInQueueCount;

            return View(UserAppointment);
        }

        public async Task<IActionResult> UserEdit(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FindAsync(ID);
            if (appointmentModel.Appointment_Status != "Booked" && appointmentModel.Appointment_Status != "Approved")
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
        public async Task<IActionResult> UserEdit(int ID, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,Remark,User_Booked_Date,Barber_Approved_Date,Barber_Check_In_Date")] AppointmentModel appointmentModel)
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
            if (appointmentModel.Appointment_Status != "Booked" && appointmentModel.Appointment_Status != "Approved")
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
        public async Task<IActionResult> Cancel(int ID, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,Remark,User_Booked_Date,Barber_Approved_Date,User_Cancelled_Reason,BarberEmail,Barber_Cancelled_Reason,AdminEmail,Admin_Cancelled_Reason,Appointment_Status")] AppointmentModel appointmentModel)
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
                
                var user = await _signInManager.UserManager.FindByEmailAsync(User.Identity.Name);
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
            }
            return View(appointmentModel);
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FirstOrDefaultAsync(m => m.ID == ID);
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

        public async Task<IActionResult> BarberServe(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FindAsync(ID);
            if (appointmentModel.Appointment_Status != "Confirmed")
            {
                ViewBag.ErrorMessage = "Appointment Status is " + appointmentModel.Appointment_Status + "! You can only serve the customer when the appointment status is confirmed. ";
            }
            else
            {
                DateTime thisDay = DateTime.Today;
                if (appointmentModel.Appointment_Date.Date != thisDay)
                {
                    ViewBag.ErrorMessage = "Appointment Date is " + appointmentModel.Appointment_Date.ToString() + "! You can only serve the customer on the same day. ";
                }
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
        public async Task<IActionResult> BarberServe(int ID, [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,Remark,User_Booked_Date,User_Confirmed_Date,Barber_Approved_Date,BarberEmail")] AppointmentModel appointmentModel)
        {
            if (ID != appointmentModel.ID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    appointmentModel.Appointment_Status = "Served";
                    DateTime now = DateTime.Now;
                    appointmentModel.Barber_Check_In_Date = now;
                    appointmentModel.BarberEmail = User.Identity.Name;
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
        public async Task<IActionResult> GenerateReceipt(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FirstOrDefaultAsync(m => m.ID == ID);

            if (appointmentModel.Appointment_Status == "Served")
            {
                appointmentModel.Appointment_Status = "Payment Pending";
                _context.Update(appointmentModel);
                await _context.SaveChangesAsync();
            }

            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> ConfirmPayment(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.AppointmentModel.FirstOrDefaultAsync(m => m.ID == ID);

            if (appointmentModel.Appointment_Status == "Payment Pending")
            {
                appointmentModel.Appointment_Status = "Paid";
                _context.Update(appointmentModel);
                await _context.SaveChangesAsync();
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

        //Part 2: Received Message from the Service Bus - cal get data function
        public async Task<IActionResult> ProcessMsg()
        {
            //queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
            items = new List<string>();
            await Task.Factory.StartNew(() =>
            {
                //to ensure the message is removed from the service bus
                queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
                var options = new MessageHandlerOptions(ExceptionMethod)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false//so that can execute the processing
                };
                queueClient.RegisterMessageHandler(ExecuteMessageProcessing, options);
            });

            return RedirectToAction("ProcessMsgResult");
        }

        //Part 2: Received Message from the Service Bus - get data step
        private static async Task ExecuteMessageProcessing(Message message, CancellationToken arg2)
        {
            //var result = JsonConvert.DeserializeObject<Ostring>(Encoding.UTF8.GetString(message.Body));
            // Console.WriteLine($"Order Id is {result.OrderId}, Order name is {result.OrderName} and quantity is {result.OrderQuantity}");
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);

            //to display in admin and barber appointment page
            items.Add("Received message: SequenceNumber:" + message.SystemProperties.SequenceNumber + " Body:" + Encoding.UTF8.GetString(message.Body));

        }

        //Part 2: Received Message from the Service Bus
        private static async Task ExceptionMethod(ExceptionReceivedEventArgs arg)
        {
            await Task.Run(() =>
           Console.WriteLine($"Error occured. Error is {arg.Exception.Message}")
           );
        }

        //Part 2: Received Message from the Service Bus - Display step
        //however, there is a bug where you have to reload the page for second time only can see the result.
        public IActionResult ProcessMsgResult()
        {
            return View(items);
        }
    }
}
