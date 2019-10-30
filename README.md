


## XPath Check Tool for desktop applications

This tool allows to check validity of XPaths and highlights found elements.

**Requirements**

 - Windows 10 PC with the latest Windows 10 version
 - Microsoft Visual Studio 2017 or later

**Getting Started**
1. Open XPathConsole.sln in Visual Studio
2. Select **Debug** > **Start Debugging** or simply **Run**

**Application Screen**

![enter image description here](https://lh3.googleusercontent.com/zm6eikA_Km93RFOhij2xjS9SbYMEPGD_6a6SEe0kiTggqhp2EyBC1bxWHGKCXpnSju7QUWH-N32M "Main window")

**Usage**

 1. Fill application **name** (most commonly found on a top of the main window of application)
 2. Fill application .exe absolute **path** in the file system of your computer
 3. **Check** if you want to start an application, if unchecked tool will try to find a running instance of you application
 4. Click on "**Find App**" button and wait for the response.  In some cases application could be in "not responding" state during this process
 5. Fill your **XPath**
 6. Click "**Find Elements**" button
 7. If one or more elements was found than list will be created. Clicking through this list will **highlight** elements on screen.
 8. Highlight overlay can be turned on or off with **checkbox.**
 9. All errors should be thrown to right text box.

**Common Issues** 

 1. **Application not found**: Check if application name and path are correct
 Application name could also contain spaces. To be sure you can copy the name from Windows Accessibility Tools > **inspect.exe**.
 2. **XPath Element not found**: XPath shouldn't contain  outer **quotation marks.**
 
	 Valid formats are **//MenuItem[@Name="File"]** or **//MenuItem[@Name=\\"File\\"]**.
	 
**Download**
You can [download](https://github.com/cngroupdk/Windows-Xpath-check/releases) the most recent version from github releases tab.