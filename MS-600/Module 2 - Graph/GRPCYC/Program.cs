using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace GRPCYC
{
  class Program
  {
    static JObject azureConfig = JObject.Parse(File.ReadAllText("appSettings.json"));
    static String clientID = azureConfig.GetValue("CLIENT_ID").ToString();
    static String authority = azureConfig.GetValue("AUTHORITY").ToString();
    static String secret = azureConfig.GetValue("SECRET").ToString();

    static void Main(string[] args)
    {
      MainAsync().GetAwaiter().GetResult();
    }

    private static async Task MainAsync()
    {
      await Authenticate();

      HaltLine("Create Group");
      string newGroupId = await CreateGroup("beGone");

      HaltLine("Add Members to Group");
      await AddMembersToGroupWithId(newGroupId);

      HaltLine("List Groups and Members");
      await ListGroups();

      HaltLine("Delete Group");
      await DeleteGroup(newGroupId);

      HaltLine("Press ENTER to quit ...");
    }

    private static void HaltLine(string _text)
    {
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine(_text);
      Console.ReadLine();
    }
    
    private static void InfoLine(string _text)
    {
      Console.ForegroundColor = ConsoleColor.DarkMagenta;
      Console.WriteLine(_text);
    }
   
    private static async Task Authenticate()
    {
      var scopes = new[] { "https://graph.microsoft.com/.default" };

      // Initialize the auth provider with values from appsettings.json
      var authProvider = new Auth(clientID, authority, secret);
      
      // Request a token 
      var accessToken = await authProvider.GetAccessToken();
      InfoLine(accessToken);

      GraphHelper.Initialize(authProvider);
    }
    
    private static async Task ListGroups()
    {
      int groupCount = 0;
      
      foreach (var group in await GraphHelper.GetGroupsAsync())
      {
        groupCount++;

        InfoLine("Group: " + group.DisplayName + " created on " + DateTime.Parse(group.CreatedDateTime.ToString()).ToString("dddd, dd MMMM yyyy"));
        InfoLine("    Members:");

        foreach (var member in await GraphHelper.GetMembersForGroupWithId(group.Id))
        {
          InfoLine("        --> " + member.DisplayName);
        }
      }

      InfoLine("found " + groupCount + " groups");
    }
   
    private static async Task<string> CreateGroup(string name)
    {
      Microsoft.Graph.Group newGroup = await GraphHelper.CreateNewGroup(name);
      InfoLine("New Group '" + newGroup.DisplayName + "' created");
     
      return newGroup.Id;
    }
   
    private static async Task AddMembersToGroupWithId(string _id)
    {
      await GraphHelper.AddMembers(_id);
      InfoLine("Members added");
    }
    
    private static async Task DeleteGroup(string _id)
    {
      await GraphHelper.DeleteGroup(_id);
      InfoLine("deleted group with id: " + _id);
    }
  }
}
