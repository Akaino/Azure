using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Extensibility;
using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AdalForDotnetWpfSample
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
            try
            {
                var authContext = new AuthenticationContext("https://login.microsoftonline.com/common");
                var result = await authContext.AcquireTokenAsync("https://graph.microsoft.com",
                    $"{clientID}",
                    new Uri($"https://login.microsoftonline.com/{tenantID}/oauth2/v2.0/authorize/"),
                    new PlatformParameters(PromptBehavior.Auto, new CustomWebUi(Dispatcher)));
                MessageBox.Show(result.AccessToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    class CustomWebUi : ICustomWebUi
    {
        private readonly Dispatcher _dispatcher;

        public CustomWebUi(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public Task<Uri> AcquireAuthorizationCodeAsync(Uri authorizationUri, Uri redirectUri)
        {
            var tcs = new TaskCompletionSource<Uri>();

            _dispatcher.InvokeAsync(() =>
            {
                var webView = new WebView();
                var w = new Window
                {
                    Title = "Auth",
                    WindowStyle = WindowStyle.ToolWindow,
                    Content = webView,
                };
                w.Loaded += (_, __) => webView.Navigate(authorizationUri);


                webView.NavigationCompleted += (_, e) =>
                {
                    System.Diagnostics.Debug.WriteLine(e.Uri);
                    if (e.Uri.Query.Contains("code="))
                    {
                        tcs.SetResult(e.Uri);
                        w.DialogResult = true;
                        w.Close();
                    }
                    if (e.Uri.Query.Contains("error="))
                    {
                        tcs.SetException(new Exception(e.Uri.Query));
                        w.DialogResult = false;
                        w.Close();
                    }
                };
                webView.UnsupportedUriSchemeIdentified += (_, e) =>
                {
                    if (e.Uri.Query.Contains("code="))
                    {
                        tcs.SetResult(e.Uri);
                        w.DialogResult = true;
                        w.Close();
                    }
                    else
                    {
                        tcs.SetException(new Exception($"Unknown error: {e.Uri}"));
                        w.DialogResult = false;
                        w.Close();
                    }
                };

                if (w.ShowDialog() != true && !tcs.Task.IsCompleted)
                {
                    tcs.SetException(new Exception("canceled"));
                }
            });

            return tcs.Task;
        }
    }
}
