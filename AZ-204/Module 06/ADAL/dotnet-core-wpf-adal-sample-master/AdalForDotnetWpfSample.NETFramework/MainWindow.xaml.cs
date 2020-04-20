using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Windows;

namespace AdalForDotnetWpfSample.NETFramework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonSignIn_Click(object sender, RoutedEventArgs e)
        {
            var clientID = "";
            var tenantID = "";
            var authContext = new AuthenticationContext("https://login.microsoftonline.com/common");
            var result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", $"{clientID}", new Uri($"https://login.microsoftonline.com/{tenantID}/oauth2/v2.0/authorize/"), new PlatformParameters(PromptBehavior.Auto));
            MessageBox.Show(result.AccessToken);
        }
    }
}
