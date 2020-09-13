using System;
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

        //Part 2: Received Message from the Service Bus - cal get data function
        public async Task<IActionResult> ProcessMsg()
        {
            //queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
            items = new List<string>();
            await Task.Factory.StartNew(() =>
            {
                queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
                var options = new MessageHandlerOptions(ExceptionMethod)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
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
            if (UserAppointment == null)
            {
                return NotFound();
            }

            try
            {
                
                ManagementClient managementClient1 = new ManagementClient(ServiceBusConnectionString);
                var managementClient = managementClient1;
                var runtimeInfo = await managementClient.GetQueueRuntimeInfoAsync(QueueName);

                var messagesInQueueCount = runtimeInfo.MessageCountDetails.ActiveMessageCount;

                ViewBag.MessageCount = messagesInQueueCount;
            }
            catch (Exception exception)
            {
                ViewBag.ErrorMessage = exception.ToString();
            }
            return View(UserAppointment);
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
            }

            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
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

        // POST: AppointmentModel/UserEdit/5
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
                    if(appointmentModel.Appointment_Status == "Confirmed")
                    {
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

        //// GET: UserModels/Edit/5
        //public async Task<IActionResult> Edit(int? AppointmentId)
        //{
        //    if (AppointmentId == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointmentModel = await _context.AppointmentModel.FindAsync(AppointmentId);
        //    if (appointmentModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(appointmentModel);
        //}

        //// POST: UserModels/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int AppointmentId, 
        //    [Bind("ID,UserEmail,Type,Services,Charges,Appointment_Date,Appointment_Status,Remark," +
        //    "User_Booked_Date,User_Confirmed_Date,User_Cancelled_Reason," +
        //    "Barber_Confirmed_Date,Barber_Check_In_Date,Barber_Cancelled_Reason,AdminEmail,Admin_Cancelled_Reason")] 
        //    AppointmentModel appointmentModel)
        //{
        //    if (AppointmentId != appointmentModel.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(appointmentModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AppointmentIDExists(appointmentModel.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        var user = await _signInManager.UserManager.FindByEmailAsync(appointmentModel.UserEmail);
        //        var roles = await _signInManager.UserManager.GetRolesAsync(user);


        //        if (roles.Any())
        //        {
        //            if (roles.First().Equals("Admin"))
        //            {
        //                return LocalRedirect("~/Admin/Index");
        //            }
        //            if (roles.First().Equals("Barber"))
        //            {
        //                return RedirectToAction(nameof(ViewAll));
        //            }
        //            if (roles.First().Equals("User"))
        //            {
        //                return RedirectToAction(nameof(UserAppointment));
        //            }
        //        }
                
        //    }
        //    return View(appointmentModel);
        //}

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
