using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebviewInvestigation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string NOT_RESPONDING_ERROR_CODE = "0x800706BA";

        private WebView webview;

        private DispatcherTimer timer;

        private readonly Uri HomeUri = new Uri("https://google.com");

        public MainPage()
        {
            this.InitializeComponent();
            InitializeNewWebview();
        }

        private void InitializeNewWebview()
        {
            var uri = HomeUri;

            if (webview != null)
            {
                uri = webview.Source;
                webview.SeparateProcessLost -= OnWebviewSeparateProcessLost;
                Root.Children.Remove(webview);
                webview = null;
            }

            webview = new WebView(WebViewExecutionMode.SeparateProcess);
            webview.SeparateProcessLost += OnWebviewSeparateProcessLost;
            webview.Navigate(uri);
            Root.Children.Add(webview);            
        }

        private void OnWebviewSeparateProcessLost(WebView sender, WebViewSeparateProcessLostEventArgs args)
        {
            if (rb1.IsChecked == true)
            {
                InitializeNewWebview();
            }
        }

        private void RegisterTimerChecker()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void ReleaseTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer = null;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            if (rb2.IsChecked == true)
            {
                try
                {
                    webview.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    if (ex.HResult.ToHex() == NOT_RESPONDING_ERROR_CODE)
                    {
                        InitializeNewWebview();
                    }
                }
            }
        }

        private void RbChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && webview != null)
            {
                if (rb.Name == "rb1") // separate process approach
                {
                    ReleaseTimer();
                    webview.SeparateProcessLost += OnWebviewSeparateProcessLost;
                }
                else if (rb.Name == "rb2") // timer approach
                {
                    webview.SeparateProcessLost -= OnWebviewSeparateProcessLost;
                    RegisterTimerChecker();
                }
            }
        }
    }


    public static class Extensions
    {
        public static string ToHex(this int number)
        {
            return $"0x{number.ToString("X")}";
        }

        //private static List<T> GetChildrenOfType<T>(this DependencyObject depObj) where T : DependencyObject
        //{
        //    List<T> result = new List<T>();

        //    if (depObj == null) return result;

        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
        //        if (child is T tChild)
        //        {
        //            result.Add(tChild);
        //        }
        //        else
        //        {
        //            result.AddRange(GetChildrenOfType<T>(child));
        //        }
        //    }
        //    return result;
        //}
    }
}
