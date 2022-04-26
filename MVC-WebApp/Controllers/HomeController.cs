using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_WebApp.Models;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;

namespace MVC_WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string Connection { get; set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            Connection = GetConnectionString().GetAwaiter().GetResult();
        }

        public IActionResult Index()
        {
            return View();
        }

        [ActionName("db-connection")]
        public IActionResult DBConnection()
        {
            GetDataFromDBPush();
            return View("DBConnection");
        }

        private void GetDataFromDBPush()
        {
            using (SqlCommand command = new SqlCommand("[/sysmail/tx_r16/sessionlist]", new SqlConnection(Connection)))
            {
                command.CommandType = CommandType.StoredProcedure;
                try
                {
                    DataSet dataSet = new DataSet("courses");
                    command.Connection.Open();
                    new SqlDataAdapter(command).Fill(dataSet, "session");
                    ViewBag.DataSet = dataSet;
                }
                catch (Exception)
                {
                    //context.Logger.LogLine("Error saving xml to S3" + exception.Message + "<br>InnerException<br>" + exception.InnerException + "<br>source:<br>" + exception.Source);
                }
                finally
                {
                    if (command.Connection.State != ConnectionState.Closed)
                    {
                        command.Connection.Close();
                    }
                }
            }
        }

        private static async Task<string> GetConnectionString()
        {
            string url = @"https://7be1ndpjqh.execute-api.us-east-1.amazonaws.com/dev/?customer=tx_r16&environment=DEV&type=SQL";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return "";
        }

        [ActionName("s3-upload")]
        public IActionResult S3Upload()
        {
            return View("S3Upload");
        }

        [ActionName("s3-download")]
        public IActionResult S3Download()
        {
            return View("S3Download");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
