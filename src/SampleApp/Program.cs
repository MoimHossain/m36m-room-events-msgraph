using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace SampleApp
{
    class Program
    {
        const string restrictedClientID = "";
        const string restrictedClientSecret ="";

        const string clientID = "";
        const string clientSecret = "";
        const string authority = "https://login.microsoftonline.com/cloudoven.onmicrosoft.com";

        static void Main(string[] args)
        {
            //var app = ConfidentialClientApplicationBuilder.Create(clientID)
            //   .WithClientSecret(clientSecret)
            //   .WithAuthority(new Uri(authority))
            //   .Build();

            var app = ConfidentialClientApplicationBuilder.Create(restrictedClientID)
               .WithClientSecret(restrictedClientSecret)
               .WithAuthority(new Uri(authority))
               .Build();

            GetEventsAsync(app).Wait();
            //RegisterSubscriptionAsync(app).Wait();
        }

        private static async Task RegisterSubscriptionAsync(IConfidentialClientApplication app)
        {
            try
            {
                // https://graph.microsoft.com/v1.0/users/atrium@cloudoven.onmicrosoft.com/calendar/events
                var authProvider = new ClientCredentialProvider(app);
                GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                var subscription = new Subscription
                {
                    ChangeType = "created",
                    NotificationUrl = "https://m365eventlistener.azurewebsites.net/",
                    Resource = "users/atrium@cloudoven.onmicrosoft.com/events",
                    ExpirationDateTime = DateTimeOffset.Now.AddMinutes(4000),
                    ClientState = "secretClientValue",
                    LatestSupportedTlsVersion = "v1_2"
                };

                await graphClient.Subscriptions
                    .Request()
                    .AddAsync(subscription);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        private static async Task GetEventsAsync(IConfidentialClientApplication app)
        {
            try
            {
                // https://graph.microsoft.com/v1.0/users/atrium@cloudoven.onmicrosoft.com/calendar/events
                var authProvider = new ClientCredentialProvider(app);
                var graphClient = new GraphServiceClient(authProvider);

                var events = await graphClient.Users["atrium@cloudoven.onmicrosoft.com"].Calendar.Events
                    .Request()
                    .GetAsync();

                var all = events.ToArray();
                Console.WriteLine(all.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task DoStuffsAsync(IConfidentialClientApplication app)
        {
            // With client credentials flows, the scope is always of the shape "resource/.default" because the
            // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
            // a tenant administrator.
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };


            try
            {
                var result = await app.AcquireTokenForClient(scopes)
                                 .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // The application doesn't have sufficient permissions.
                // - Did you declare enough app permissions during app creation?
                // - Did the tenant admin grant permissions to the application?
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
                // Mitigation: Change the scope to be as expected.
            }
        }
    }
}
