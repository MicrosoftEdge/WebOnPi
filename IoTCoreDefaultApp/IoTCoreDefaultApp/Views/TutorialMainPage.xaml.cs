// Copyright (c) Microsoft. All rights reserved.


using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using IoTCoreDefaultApp.Utils;

namespace IoTCoreDefaultApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TutorialMainPage : Page
    {
        private DispatcherTimer timer;
        private Uri sampleUri;
        public TutorialMainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            this.DataContext = LanguageManager.GetInstance();

            this.Loaded += (sender, e) =>
            {
                UpdateDateTime();

                timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = TimeSpan.FromSeconds(30);
                timer.Start();
            };
            this.Unloaded += (sender, e) =>
            {
                timer.Stop();
                timer = null;
            };
        }

        private void timer_Tick(object sender, object e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            var t = DateTime.Now;
            this.CurrentTime.Text = t.ToString("t", CultureInfo.CurrentCulture) + Environment.NewLine + t.ToString("d", CultureInfo.CurrentCulture);
        }

        private void ShutdownButton_Clicked(object sender, RoutedEventArgs e)
        {
            ShutdownDropdown.IsOpen = true;
        }

        private void ShutdownDropdown_Opened(object sender, object e)
        {
            var w = ShutdownListView.ActualWidth;
            if (w == 0)
            {
                // trick to recalculate the size of the dropdown
                ShutdownDropdown.IsOpen = false;
                ShutdownDropdown.IsOpen = true;
            }
            var offset = -(ShutdownListView.ActualWidth - ShutdownButton.ActualWidth);
            ShutdownDropdown.HorizontalOffset = offset;
        }

        private void ShutdownHelper(ShutdownKind kind)
        {
            ShutdownManager.BeginShutdown(kind, TimeSpan.FromSeconds(0.5));
        }

        private void ShutdownListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FrameworkElement;
            if (item == null)
            {
                return;
            }
            switch (item.Name)
            {
                case "ShutdownOption":
                    ShutdownHelper(ShutdownKind.Shutdown);
                    break;
                case "RestartOption":
                    ShutdownHelper(ShutdownKind.Restart);
                    break;
            }
        }
        private void SettingsButton_Clicked(object sender, RoutedEventArgs e)
        {
            NavigationUtils.NavigateToScreen(typeof(Settings));
        }

        private void DeviceInfo_Clicked(object sender, RoutedEventArgs e)
        {
            NavigationUtils.NavigateToScreen(typeof(MainPage));
        }
        private void Samples_Clicked(object sender, RoutedEventArgs e)
        {
            showAllSamples();
            NavigationUtils.NavigateToScreen(typeof(TutorialMainPage));
        }
        private void TutorialList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FrameworkElement;
            if (item == null)
            {
                return;
            }
            switch (item.Name)
            {
                case "S1":
                    AppLaunch(new Uri("edgeonpi1:launch"), new Uri("http://seksenov.github.io/WebHelloBlinky/"));
                    break;
                case "S2":
                    AppLaunch(new Uri("edgeonpi2:launch"), new Uri("http://microsoftedge.github.io/JSBrowser/"));
                    break;
                case "S3":
                    NavigateToSample(new Uri("http://codepen.io/seksenov/pen/bERWWw"));
                    break;
                case "S4":
                    AppLaunch(new Uri("edgeonpi4:launch"), new Uri("http://windowstodo.meteor.com/"));
                    break;
                default:
                    NavigationUtils.NavigateToScreen(typeof(TutorialContentPage), item.Name);
                    break;
            }
        }
        private void NavigateToSample(Uri uri)
        {
            showSpinner();
            NavigationUtils.NavigateToWebView(WebViewWithSample, uri);
        }
        async void AppLaunch(Uri uri, Uri fallback)
        {
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (!success)
            {
                // URI launch failed so launch the website in a web view
               NavigateToSample(fallback);
            }
        }
        private void WevViewWithSample_LoadCompleted(object sender, NavigationEventArgs e)
        {
            showSample();
        }
        private void showSample()
        {
            WebViewWithSample.Visibility = Visibility.Visible;
            TutorialList.Visibility = Visibility.Collapsed;
            Spinner.Visibility = Visibility.Collapsed;
        }
        private void showSpinner()
        {
            WebViewWithSample.Visibility = Visibility.Collapsed;
            TutorialList.Visibility = Visibility.Collapsed;
            Spinner.Visibility = Visibility.Visible;
        }
        private void showAllSamples()
        {
            WebViewWithSample.Visibility = Visibility.Collapsed;
            TutorialList.Visibility = Visibility.Visible;
            Spinner.Visibility = Visibility.Collapsed;
        }
    }
}
