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
        public async Task<IActionResult> Create([Bind("ID,UserEmail,Status,User_Confirmed_Date")] AppointmentModel appointment)
            //public async Task<IActionResult> Create([Bind("ID,UserEmail,Status,User_Confirmed_Date,User_Check_In_Date,User_Cancelled_Reason,ShopEmail,Shop_Confirmed_Date,Shop_Check_In_Date,Shop_Cancelled_Reason,AdminEmail,Admin_Cancelled_Reason")] AppointmentModel appointment)

            //public async Task<IActionResult> Create([Bind("ID,UserEmail,Status,User_Confirmed_Date,User_Check_In_Date,User_Cancelled_Reason,ShopEmail,Shop_Confirmed_Date,Shop_Check_In_Date,Shop_Cancelled_Reason,AdminEmail,")] AppointmentModel appointment, [Bind("ID,AppointmentID,Services,Charges")] AppointmentDetailsModel services)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(appointment);
                    //_context.Add(services);
                    await _context.SaveChangesAsync();//save appointment and respective services record
                }
                catch(Exception e)
                {
                    ViewBag.msg = "Error: " + e.ToString();
                }
                
                return RedirectToAction("Create");
            }
            return View("~/Shared/_LayoutBarber");
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
    }
}
