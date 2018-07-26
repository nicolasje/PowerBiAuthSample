using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using System;
using System.Threading.Tasks;

namespace EmbedAPISample
{
    class Program
    {
        private static string authorityUrl = "https://login.windows.net/common/oauth2/authorize/";
        private static string resourceUrl = "https://analysis.windows.net/powerbi/api";
        private static string apiUrl = "https://api.powerbi.com/";
        private static string embedUrlBase = "https://app.powerbi.com/";

        private static string groupId = "27eb16c5-9aad-4f05-9457-1e0b13597eef"; 
        private static string rptId = "ae2df1d5-caf9-4f85-bb2d-6baec208ca25";
        private static string username = "svc-ra-gateway@rockwellautomation.com";

        // Update the Password and Client ID within Secrets.cs

        private static UserPasswordCredential credential = null;
        private static AuthenticationResult authenticationResult = null;
        private static TokenCredentials tokenCredentials = null;

        static void Main(string[] args)
        {

            try
            {
                // Create a user password cradentials.
                credential = new UserPasswordCredential(username, Secrets.Password);

                // Authenticate using created credentials
                Authorize().Wait();

                using (var client = new PowerBIClient(new Uri(apiUrl), tokenCredentials))
                {

                    EmbedToken embedToken = client.Reports.GenerateTokenInGroup(groupId, rptId, new GenerateTokenRequest(accessLevel: "View", datasetId: "4e95884b-784b-44e4-9f71-6a123fc68073"));

                    Report report = client.Reports.GetReportInGroup(groupId, rptId);

                    #region Output Embed Token
                    Console.WriteLine("\r*** EMBED TOKEN ***\r");

                    Console.Write("Report Id: ");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("<REPORT ID>");
                    Console.ResetColor();

                    Console.Write("Report Embed Url: ");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(report.EmbedUrl);
                    Console.ResetColor();

                    Console.WriteLine("Embed Token Expiration: ");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(embedToken.Expiration.Value.ToString());
                    Console.ResetColor();


                    Console.WriteLine("Report Embed Token: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(embedToken.Token);
                    Console.ResetColor();
                    #endregion

                    #region Output Datasets
                    Console.WriteLine("\r*** DATASETS ***\r");

                    // List of Datasets
                    // This method calls for items in a Group/App Workspace. To get a list of items within your "My Workspace"
                    // call GetDatasets()
                    ODataResponseListDataset datasetList = client.Datasets.GetDatasetsInGroup(groupId);

                    foreach (Dataset ds in datasetList.Value)
                    {
                        Console.WriteLine(ds.Id + " | " + ds.Name);
                    }
                    #endregion

                    #region Output Reports
                    Console.WriteLine("\r*** REPORTS ***\r");

                    // List of reports
                    // This method calls for items in a Group/App Workspace. To get a list of items within your "My Workspace"
                    // call GetReports()
                    ODataResponseListReport reportList = client.Reports.GetReportsInGroup(groupId);

                    foreach (Report rpt in reportList.Value)
                    {
                        Console.WriteLine(rpt.Id + " | " + rpt.Name + " | DatasetID = " + rpt.DatasetId);
                    }
                    #endregion

                    #region Output Dashboards
                    Console.WriteLine("\r*** DASHBOARDS ***\r");

                    // List of reports
                    // This method calls for items in a Group/App Workspace. To get a list of items within your "My Workspace"
                    // call GetReports()
                    ODataResponseListDashboard dashboards = client.Dashboards.GetDashboardsInGroup(groupId);

                    foreach (Dashboard db in dashboards.Value)
                    {
                        Console.WriteLine(db.Id + " | " + db.DisplayName);
                    }
                    #endregion

                    #region Output Gateways
                    Console.WriteLine("\r*** Gateways ***\r");

                    ODataResponseListGateway gateways = client.Gateways.GetGateways();

                    Console.WriteLine(gateways.Value[0].Name);

                    //foreach (Gateway g in gateways)
                    //{
                    //    Console.WriteLine(g.Name + " | " + g.GatewayStatus);
                    //}
                    #endregion
                }

            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }

        }

        private static Task Authorize()
        {
            return Task.Run(async () => {
                authenticationResult = null;
                tokenCredentials = null;
                var authenticationContext = new AuthenticationContext(authorityUrl);

                authenticationResult = await authenticationContext.AcquireTokenAsync(resourceUrl, Secrets.ClientID, credential);

                if (authenticationResult != null)
                {
                    tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");
                }
            });
        }






    }
}
