using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TEATEM
{
  class Program
  {
    static JObject azureConfig = JObject.Parse(File.ReadAllText("appSettings.json"));
    static String clientID = azureConfig.GetValue("CLIENT_ID").ToString();
    static String authority = azureConfig.GetValue("AUTHORITY").ToString();
    static String secret = azureConfig.GetValue("SECRET").ToString();
    static DateTime stopAfter = System.DateTime.Now.AddMinutes(16);
    static Timer timer;
    static void Main(string[] args)
    {
      MainAsync().GetAwaiter().GetResult();
    }
    private static async Task MainAsync()
    {
      await Authenticate();

      HaltLine("Create Group");
      string newGroupId = await CreateGroup("soonToBeTeam1");
      
      HaltLine("Add Members to Group");
      await AddMembersToGroup(newGroupId);

      HaltLine("Create Team");
      string newTeamId = await CreateTeam("MyShortlivedTeam1");

      HaltLine("Create Channel and Tab");
      await CreateChannelAndTab(newTeamId);

      HaltLine("List Channel and Tab");
      await ListChannelsAndTabsForTeam(newTeamId);

      HaltLine("Delete Team");
      await DeleteTeam(newTeamId);

      timer = new Timer(new TimerCallback(CheckGroupToTeam), newGroupId, 0, 5000);
      
      HaltLine("Press ENTER to quit (but wait ~15 min) ...");
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

    private static async Task ListTeams()
    {
      int teamCount = 0;

      foreach (var team in await GraphHelper.ListTeams())
      {
        teamCount++;

        InfoLine("Group: " + team.DisplayName);
        InfoLine("    Members:");

        foreach (var member in await GraphHelper.ListTeamMembers(team.Id))
        {
          InfoLine("        --> " + member.DisplayName);
        }
      }

      InfoLine("found " + teamCount + " teams");
    }

    private static async Task<string> CreateGroup(string name)
    {
      Microsoft.Graph.Group newGroup = await GraphHelper.CreateNewGroup(name);
      InfoLine("New Group '" + newGroup.DisplayName + "' created");

      return newGroup.Id;
    }

    private static async Task AddMembersToGroup(string _id)
    {
      await GraphHelper.AddMembers(_id);
      InfoLine("Members added");
    }

    private static async Task<string> CreateTeam(string name)
    {
      Microsoft.Graph.Team newGroup = await GraphHelper.CreateNewTeam(name);
      InfoLine("New Team '" + newGroup.DisplayName + "' created");

      return newGroup.Id;
    }

    private static async Task<string> CreateTeamFromGroup(string _id)
    {
      Microsoft.Graph.Team newTeam = await GraphHelper.ChangeGroupToTeam(_id);

      if (newTeam == null)
      {
        return null;
      }

      InfoLine("New Team (from Group) '" + newTeam.DisplayName + "' created");

      return newTeam.Id;
    }

    private static async Task CreateChannelAndTab(string _id)
    {
      await GraphHelper.AddTeamsChannelAndTab(_id);
      InfoLine("Channel and Tab created");
    }

    private static async Task ListChannelsAndTabsForTeam(string _id)
    {
      var result = await GraphHelper.ListTeamChannelAndTabs(_id);

      foreach (var key in result.Keys)
      {
        InfoLine("Channel: " + key.DisplayName);
        InfoLine("    Tabs:");

        foreach (var tab in result[key])
        {
          InfoLine("        --> " + tab.DisplayName);
        }
      }
    }

    private static async Task ListTeamMembers(string _id)
    {
      InfoLine("    Members:");

      foreach (var member in await GraphHelper.ListTeamMembers(_id))
      {
        InfoLine("        --> " + member.DisplayName);
      }
    }

    private static async Task DeleteTeam(string _id)
    {
      await GraphHelper.DeleteTeam(_id);
      InfoLine("Team with id '" + _id + "' deleted");
    }

    private static async void CheckGroupToTeam(object obj)
    {
      string _id = obj.ToString();

      if (System.DateTime.Now >= stopAfter)
      {
        timer.Dispose();
        return;
      }

      var newGrpTeamID = await CreateTeamFromGroup(_id);

      if (!String.IsNullOrEmpty(newGrpTeamID))
      {
        timer.Dispose();

        InfoLine("Group changed to Team");
        InfoLine("GroupID: " + _id);
        InfoLine("TeamId " + newGrpTeamID);

        InfoLine("Listing TeamMembers");
        await ListTeamMembers(newGrpTeamID);

        InfoLine("Delete Team");
        await DeleteTeam(newGrpTeamID);
        
        return;
      }
    }
  }
}
