﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace SeleniumDriver
{
    public class AppState : IDisposable
    {
        private WindowsElement _app;
        private WindowsDriver<WindowsElement> _driver;
        private Process _driverProcess;
        private Process _appProcess;

        public WindowsElement GetApp()
        {
            return _app ?? throw new Exception("Sorry dude, bad workflow");
        }

        public WindowsDriver<WindowsElement> GetDriver()
        {
            return _driver ?? throw new Exception("Sorry dude, bad workflow");
        }

        public void InitApp(string appName, string appPathInput, bool startApp =false)
        {
            Dispose();
            var projectPath = Path.GetDirectoryName(Path.GetPathRoot(Directory.GetCurrentDirectory()));
            _driverProcess = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = projectPath + "\\Driver\\WinAppDriver.exe"
                }
            };
            _driverProcess.Start();

            if (startApp)
            {
                _appProcess = Process.Start(appPathInput);

                Thread.Sleep(1000);

            }
            var factory = new WindowsAppFactory();
            _app =  factory.CreateApp(appName);
            _driver = factory.CreateDriver(_app);
        }

        public void Dispose()
        {
            _driver?.Dispose();
            _driverProcess?.Dispose();
            //We do not dispose appProces to do nto close that app when this application is closed
        }
    }
}