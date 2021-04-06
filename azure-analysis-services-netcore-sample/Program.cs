using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Data;

namespace azure_analysis_services_netcore_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string JSONString_result = GetDataFromAzureAnalysisService("select ([Measures].[EmployeesLeft]) on Columns,([VW_fact_attrition_till_live].[Year].[Year].members,[VW_Fact_Attrition_till_live].[month].[month].members) on rows from [Model]");
            Console.WriteLine(JSONString_result);
            Console.ReadKey();
        }
        private static string GetDataFromAzureAnalysisService(string query)
        {
            //Grab the token
            //Get servername from Azure Analysis Service (Overview) resource
            //Format: asazure://.asazure.windows.net/
            var serverName = "asazure://westus.asazure.windows.net/agghactice";
            var token = GetAccessToken("https://westus.asazure.windows.net");
            var connectionString = $"Provider=MSOLAP;Data Source={serverName};Initial Catalog=databasename;User ID=;Password={token};Persist Security Info=True;Impersonation Level=Impersonate";
            DataTable _dt = new DataTable();
            string JSONString_result = string.Empty;
            try
            {
                //read data from AAS
                AdomdConnection connection = new AdomdConnection(connectionString);

                connection.Open();
                using (AdomdDataAdapter ad = new AdomdDataAdapter(query, connection))
                {
                    ad.Fill(_dt);
                    if (_dt.Rows.Count > 0)
                    {
                        JSONString_result = JsonConvert.SerializeObject(_dt);
                    }
                }

            }
            catch (Exception ex)
            {
                string ss = ex.Message.ToString();
                Console.WriteLine(ex.Message.ToString());
            }
            return JSONString_result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aasUrl">azure analysis service URL</param>
        /// <returns></returns>
        private static string GetAccessToken(string aasUrl)
        {
            var tenantId = "21ssd4788-da1f-4187-rete8-ret776465ca0";
            var appId = "06252235-0f11-4b76-8v1d-5uyrtrsxc9051xsda";
            var appSecret = "n3GnVPxkmtiEU5ITsHlddggdt23OIP9p36afY1onkHD0lbE=";
            string authorityUrl = $"https://login.microsoftonline.com/{tenantId}";

            var clientCredential = new ClientCredential(appId, appSecret);
            var context = new AuthenticationContext(authorityUrl);
            var result = context.AcquireTokenAsync(aasUrl, clientCredential);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.Result.AccessToken;
            return token;
        }
    }
}

