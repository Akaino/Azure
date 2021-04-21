using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Identity.Client;
using Microsoft.Graph;

using System.Net.Http;
using System.Net.Http.Headers;


namespace CREUPD
{
    class Program
    {
        
        private static string clientID = "e433aa6a-8116-4c18-82bf-031768fe9ef9";
        private static string tenant = "5d8b74a9-cfa0-4f65-9d4f-56b92787a8c3";
        private static string clientSecret = "zkfIbsZvL=Gy_51COh7K1yGsUHCqZH@@";
        private static string instance = $"https://login.microsoftonline.com/{tenant}/v2.0";
        
        //Permissions: User.Read, User.ReadWrite.All
        private static List<string> scopes = new List<string>();

        static void Main(string[] args)
        {
            scopes.Add("https://graph.microsoft.com/.default");

            var ccs = ConfidentialClientApplicationBuilder.Create(clientID)
                                                          .WithAuthority(instance)
                                                          .WithClientSecret(clientSecret)
                                                          .Build();
            
            GraphServiceClient graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) => {

                    var authResult = await ccs
                        .AcquireTokenForClient(scopes)
                        .ExecuteAsync();

                    requestMessage.Headers.Authorization = 
                        new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                })
            );

            //Wird später zum neuen Manager
            var manager = graphServiceClient.Users["mctest@devtobi.onmicrosoft.com"].Request().GetAsync().GetAwaiter().GetResult();

            var newUserPwdProfile = new PasswordProfile
            {
                Password = "ChangeMe1!",
                ForceChangePasswordNextSignIn = false
            };
            
            var newUser = new User
            {
                DisplayName = "Test User",
                UserPrincipalName = "testu@devtobi.onmicrosoft.com",
                MailNickname = "testu",
                AccountEnabled = true,
                PasswordProfile = newUserPwdProfile,
                GivenName = "Test",
                Department = "TestGuys",
                Surname = "User"
            };

            //User anlegen
            try
            {
                var userCall = graphServiceClient.Users.Request().AddAsync(newUser).GetAwaiter().GetResult();
            }
            catch(Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
            finally
            {
                var timedKillCommand = new Timer((e) => 
                {
                    Console.WriteLine("Killing The User");
                    var result = graphServiceClient.Users[newUser.UserPrincipalName].Request().DeleteAsync();
                    
                   Console.WriteLine("Assimilated");
                    
                    //kill
                    Environment.Exit(0);
                }, null, ((1000 * 60) * 1), Timeout.Infinite);
            }

            Console.WriteLine("User created? I\'m checking");
            var checkUser = graphServiceClient.Users.Request().Filter($"displayName eq '{newUser.DisplayName}'").GetAsync().GetAwaiter().GetResult();
            
            if (checkUser != null && checkUser.Count > 0)
            {
                var checkUserContent = checkUser.CurrentPage[0];
                Console.WriteLine("User created! Let\'s assign a manager");
                graphServiceClient.Users[checkUserContent.Id].Manager.Reference.Request().PutAsync(manager.Id).GetAwaiter();
                
                Console.WriteLine("Manager Assigned! Let\'s rename him ....");
                var userUpdate = new User
                {
                    DisplayName = "Updated thisGuy",
                    OfficeLocation = "Knowhere"
                };

                var updatedUser = graphServiceClient.Users[checkUserContent.Id].Request().UpdateAsync(userUpdate).GetAwaiter();
                checkUserContent = graphServiceClient.Users[checkUserContent.Id].Request().Select(u => new {u.DisplayName, u.Department, u.Id}).GetAsync().GetAwaiter().GetResult();
                Console.WriteLine($"{checkUserContent.DisplayName} working at {checkUserContent.Department}");
            }
            else
            {
                Console.WriteLine("User NOT created!");
            }

            Console.ReadLine();
        }
    }
}

