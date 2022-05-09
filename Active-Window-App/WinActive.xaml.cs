using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
    /// Interaction logic for WinActive.xaml
    /// </summary>
    public partial class WinActive : UserControl
    {
        public WinActive()
        {
            InitializeComponent();
            dele = new WinEventDelegate(WinEventProc);

            m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);

        }
     
            delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
            WinEventDelegate dele = null;
            string prevTitle = "";
            private Object thisLock = new Object();
            IntPtr hand = IntPtr.Zero;
            StringBuilder sBuffLog = new StringBuilder();
            IntPtr m_hhook = IntPtr.Zero;


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



                //Thread tParm = new Thread(DoTransactions2);
                ////Start thread execution
                //GCHandle handleHwnd = (GCHandle)hwnd;
                //tParm.Start();

            }
            static void threadFunc(object arg)
            {
                Console.WriteLine(arg);
            }
            private void TestThree()
            {
                return;
            }

            private void DoTransactions(object hwndobj)
            {

                GCHandle handleHwnd = GCHandle.Alloc(hwndobj);
                IntPtr hwnd = (IntPtr)handleHwnd;

                string title = GetActiveWindowTitle();
                if (title == null)
                    return;

                //lock (thisLock)
                //{
                if (title.Trim() != prevTitle.Trim())
                {


                    prevTitle = title;
                    //Console.WriteLine("test");
                    //if (title.Contains("- Brave") || title.Contains("- Google Chrome") || title.Contains("- Microsoft?"))
                    //{
                    //    string url = "";
                    //    //if (hwnd == IntPtr.Zero)
                    //    //{ 
                    //    //   url= searchForBrowserURL("brave");
                    //    //}


                    //    if (string.IsNullOrEmpty(url))
                    //        url = GetBrowsedUrlFromHwnd(hwnd).ToString();
                    //    sBuffLog.Append(GetActiveWindowTitle() + "\r\n\t" + url + "\r\n");
                    //}
                    //else
                    sBuffLog.Append(GetActiveWindowTitle() + "\r\n");

                    return;
                }

                //}
            }
            private void DoTransactions2()
            {
                string title = GetActiveWindowTitle();
                //string title = GetActiveWindowTitle();
                //if (title == null)
                //    return;

                //lock (thisLock)
                //{
                //    if (title.Trim() != prevTitle.Trim())
                //    {


                //        prevTitle = title;
                //        Console.WriteLine("test");
                //        if (title.Contains("- Brave") || title.Contains("- Google Chrome") || title.Contains("- Microsoft?"))
                //        {
                //            string url = "";

                //            url= searchForBrowserURL("brave");

                //            //if (string.IsNullOrEmpty(url))
                //            //    url = GetBrowsedUrlFromHwnd(hwnd).ToString();
                //            sBuffLog.Append(GetActiveWindowTitle() + "\r\n\t" + url + "\r\n");
                //        }
                //        else
                //            sBuffLog.Append(GetActiveWindowTitle() + "\r\n");

                //        return;
                //    }

                //}
            }
            private static string GetActiveWindowTitle()
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

            private string searchForBrowserURL(string name)
            {
                Process[] process = Process.GetProcessesByName(name);
                string url = "";
                for (int i = 0; i < process.Length; i++)
                {
                    Process process2 = process[i];

                    url = GetBrowsedUrl(process2).ToString();
                    if (!string.IsNullOrEmpty(url) && url != "zero" && url != "No Property for Docz")
                        return url;
                }
                return "";
            }


            public string GetBrowsedUrlFromHwnd(IntPtr hwnd)
            {
                string rString = "";
                if (hwnd == IntPtr.Zero || hwnd == null) { return "No Pointer"; }
                AutomationElement element = AutomationElement.FromHandle(hwnd);
                if (element == null)
                    return "ele";

                AutomationElement doc = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));


                //if(doc == null)
                //{
                //    doc = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));
                //}
                if (doc == null)
                {
                    string url = "";
                    //if (hwnd == IntPtr.Zero)
                    //{ 
                    url = searchForBrowserURL("brave");
                    //}
                    return url + "No Property for Doc";
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

            public string GetBrowsedUrl(Process process)
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

        
    }
}
