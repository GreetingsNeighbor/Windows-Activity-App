using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Active_Window_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>.
    /// 
    public delegate void Del(IntPtr hwnd);

    public partial class MainWindow : Window
    {
        WinEventDelegate winDel = null;
        string prevTitle = "";

        public MainWindow()
        {
            InitializeComponent();
            winDel = new WinEventDelegate(WinEventProc);
            IntPtr _eventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero, winDel, 0, 0, WINEVENT_OUTOFCONTEXT);
        }


        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        private const uint EVENT_OBJECT_STATECHANGE = 0x800A;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd == IntPtr.Zero)
                return;
            GetTitleAndUrl(hwnd);
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 512;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }


        private void GetTitleAndUrl(IntPtr hwnd)
        {
            string title = GetActiveWindowTitle();
            if (title == null)
                return;
            string browser = "";
            if (title.Trim() != prevTitle.Trim())
            {

                prevTitle = title;
                Console.WriteLine("test");
                if (title.Contains("- Brave"))
                {
                    browser = "brave";
                }
                else if (title.Contains("- Google Chrome"))
                {
                    browser = "chrome";
                }
                else if (title.Contains("- Microsoft?"))
                {
                    browser = "edge";
                }
                if (!string.IsNullOrEmpty(browser))
                {
                    string url = "";
                    //if (string.IsNullOrEmpty(url))

                    url = GetBrowsedUrlFromHwnd(hwnd, browser).ToString();
                    Log.Text = GetActiveWindowTitle() + "\r\n\t" + url + "\r\n" + Log.Text;
                }
                else
                    Log.Text = GetActiveWindowTitle() + "\r\n" + Log.Text;

                return;
            }


        }


        private string searchBrowserForUrl(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            string url = "";
            for (int i = 0; i < processes.Length; i++)
            {
                Process process2 = processes[i];
                if (process2.MainWindowHandle == IntPtr.Zero)
                    continue;
                url = getProcessUrl(process2).ToString();
                if (!string.IsNullOrEmpty(url) && url != "zero" && url != "No Property for Doc From Process Search")
                    return url;
            }
            return "No Url Located";
        }


        public string GetBrowsedUrlFromHwnd(IntPtr hwnd, string browser)
        {
            string rString = "";
            if (hwnd == IntPtr.Zero || hwnd == null) { return "No Pointer"; }
            AutomationElement element = AutomationElement.FromHandle(hwnd);
            if (element == null)
                return "ele";

            AutomationElement doc = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));



            //doc = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));

            if (doc == null)
            {
                string url = "";
                //if (hwnd == IntPtr.Zero)
                //{ 

                url = searchBrowserForUrl(browser);
                //}
                return "No Property for Doc:" + url;
            }

            if ((bool)doc.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
            {
                return "keyboard";
            }


            if (doc != null)
                rString = (string)element.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);

            if (String.IsNullOrEmpty(rString))
            {
                try
                {

                    return ((ValuePattern)doc.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
                }
                catch { return "No Pattern"; }
            }
            return rString;



        }
        public string getProcessUrl(Process process)
        {
            Console.Write(process.ProcessName);
            if (process.ProcessName.ToLower() == "brave")
            {
                if (process == null)
                    throw new ArgumentNullException("process");

                if (process.MainWindowHandle == IntPtr.Zero)
                    return "zero";

                AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
                if (element == null)
                    return "ele";


                //string nameProp1;
                //object namePropNoDefault =
                //    element.GetCurrentPropertyValue(AutomationElement.NameProperty, false);
                //if (namePropNoDefault == AutomationElement.NotSupported)
                //{
                //    nameProp1 = "No name.";
                //}
                //else
                //{
                //    nameProp1 = namePropNoDefault as string;
                //}
                //Console.WriteLine(nameProp1 + "dsdsd " + namePropNoDefault);
                //AutomationElement doc = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));

                AutomationElement doc = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
                if (doc == null)
                    return "No Property for Docz";

                if (doc != null)
                    return (string)doc.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);


                //AutomationPattern[] patterns = doc.GetSupportedPatterns();
                //if(patterns.Length == 0)
                //{
                //    return "z";
                //}else
                //{
                //    Console.WriteLine("completing");
                //}

                //return ((ValuePattern)doc.GetCurrentPattern(patterns[0])).Current.Value as string;
            }

            //else
            //{
            //    if (process == null)
            //        throw new ArgumentNullException("process");

            //    if (process.MainWindowHandle == IntPtr.Zero)
            //        return null;

            //    AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            //    if (element == null)
            //        return null;

            //    AutomationElement edit = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));


            //    string result = ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
            //    return result;
            //}
            return "Other";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }



}
