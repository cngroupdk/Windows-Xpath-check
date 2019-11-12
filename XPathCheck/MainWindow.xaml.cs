using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SeleniumDriver;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace XPathCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly StateManager<Highlight> _stateManager = new StateManager<Highlight>();
        public TextBox TbXPath => tbXPath;
        public TextBox TbXPathResponse => tbXPathResponse;
        public ListView LvFoundElements => lvFoundElements;
        private List<IWebElement> foundElementsList;
        public TextBox TbAppPath => tbAppPath;
        public TextBox TbAppName => tbAppName;
        public CheckBox CbStartApp => cbStartApp;
        public CheckBox CbOverlay => cbOverlay;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            tbXPath.Text = "//MenuItem[@Name=\"File\"]";
        }

        private void btnFindElementsButton_Click(object sender, RoutedEventArgs e)
        {
            var userXPath = Regex.Unescape(tbXPath.Text);
            TbXPathResponse.Text = "";
            try
            {
                _stateManager.SnapToApp();
                lvFoundElements.Items.Clear();
                var foundElements = _stateManager.GetDriver().FindElements(userXPath, 5);
                foundElementsList = foundElements;
                tbXPathResponse.Text = "";

                foreach (var element in foundElements)
                {
                    lvFoundElements.Items.Add("Text: " + element.Text + " \t " + "TagName: " + element.TagName);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                tbXPathResponse.Text = "";
                tbXPathResponse.Text = exception.Message;
            }
        }

        private void listView_Click(object sender, MouseButtonEventArgs e)
        {
            var appWindow = _stateManager.GetApp();
            var highlight = _stateManager.GetOverlay();
            highlight.SnapToApp(appWindow.Coordinates.LocationInViewport.X, appWindow.Coordinates.LocationInViewport.Y, appWindow.Size.Width, appWindow.Size.Height);
            var index = lvFoundElements.SelectedIndex;
            if (index < 0 || foundElementsList.Count <= index) return;
            var element = foundElementsList[index];
            highlight.DrawRect(element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);
        }

        private void btnFindApp_Click(object sender, RoutedEventArgs e)
        {
            tbXPathResponse.Text = "";
            btnFindApp.Content = "Finding App...";
            btnFindApp.Background = Brushes.Red;
            try
            {
                var appName = tbAppName.Text;
                var appPath = tbAppPath.Text;
                var start = cbStartApp.IsChecked.Value;
                _stateManager.InitApp(appName, appPath, start);
                OnAppFound();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                tbXPathResponse.Text = "";
                tbXPathResponse.Text = exception.Message;
                btnFindApp.Content = "App Not Found!";
                btnFindApp.Background = Brushes.Red;
            }
        }

        private void OnAppFound()
        {
            btnFindApp.Background = Brushes.Green;
            btnFindApp.Content = "App Found!";
            cbOverlay.IsChecked = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _stateManager.Close();
        }

        private void cbOverlay_Click(object sender, RoutedEventArgs e)
        {
            var highlight = _stateManager.GetOverlay();
            if (cbOverlay.IsChecked.GetValueOrDefault())
            {
                var appWindow = _stateManager.GetApp();
                highlight?.SnapToApp(appWindow.Coordinates.LocationInViewport.X, appWindow.Coordinates.LocationInViewport.Y, appWindow.Size.Width, appWindow.Size.Height);
                highlight?.Show();
            }
            else
            {
                highlight?.Hide();
            }
        }

        public void Dispose()
        {
            _stateManager?.Dispose();
        }
    }
}







