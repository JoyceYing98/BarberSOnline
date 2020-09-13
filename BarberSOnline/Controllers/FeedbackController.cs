using BarberSOnline.Areas.Identity.Data;
using BarberSOnline.Data;
using BarberSOnline.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BarberSOnline.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly UserManager<BarberSOnlineUser> _userManager;


        public FeedbackController(UserManager<BarberSOnlineUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            CloudTable tableclient = getTableInformation();
            TableQuery<FeedbackModel> query = new TableQuery<FeedbackModel>();
            List<FeedbackModel> flist = new List<FeedbackModel>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<FeedbackModel> tquery = tableclient.ExecuteQuerySegmentedAsync(query,token).Result;
                token = tquery.ContinuationToken;
                foreach (FeedbackModel feedbackModel in tquery.Results)
                {
                    flist.Add(feedbackModel);
                }
            }
            while (token != null); 
            return View(flist);

        }



        //step 1: set the information connection
        private CloudTable getTableInformation()
        {
            //link to appsettings.json file and read the access key
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            //create a link to connect your storage account
            CloudStorageAccount storageaccount =
            CloudStorageAccount.Parse(configure["StorageConnectionString"]);

            //create agent object client to communicate between ur app and ur storage account
            CloudTableClient tableClient = storageaccount.CreateCloudTableClient();

            //refer to the table that related to your app     
            CloudTable tables = tableClient.GetTableReference("Feedback");

            return tables;

        }

        [HttpGet]
        // gey user login id
        public IActionResult Create()
        {
            ViewBag.UserId = User.Identity.Name;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Username,FeedbackContent, Rating, PartitionKey")] FeedbackModel feedbackModel)
        {
            CloudTable tableclient = getTableInformation();
            var createFeedback = new FeedbackModel
            {
                PartitionKey = feedbackModel.PartitionKey,
                RowKey = Guid.NewGuid().ToString(),
                Username = feedbackModel.Username,
                //FeedbackTitle = feedbackModel.FeedbackTitle,
                FeedbackContent = feedbackModel.FeedbackContent,
                Rating =feedbackModel.Rating
            };
            try
            {
                TableOperation insertOperation = TableOperation.Insert(createFeedback);
                TableResult result = tableclient.ExecuteAsync(insertOperation).Result;
                if (result != null)
                {
                    ViewBag.tablename = tableclient.Name;
                    ViewBag.msg = "Data successfullt inserted!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.tablename = tableclient.Name;
                ViewBag.msg = ex.ToString();
            }
           
            return RedirectToAction(nameof(Index));
        }

       
    }
}
