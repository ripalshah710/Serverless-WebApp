using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SLW_App.Pages
{
    public class HelpModel : PageModel
    {
        public string Message { get; set; }
        private string Connection { get; set; }

        public void OnGet()
        {
            Connection = GetConnectionString().GetAwaiter().GetResult();
            GetDataFromDBPush();
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
                    ViewData["DataSet"] = dataSet;
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
    }
}
