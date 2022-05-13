using System;
using System.Collections.Generic;
using System.Data;
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
using ActiveModules;

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
        public List<LimitedActivity> limitedActivity = new List<LimitedActivity>();
        SQLiteManager sqLiteManager = null;
        bool _isDataGridSet = false;
        public MainWindow()
        {
            InitializeComponent();
            sqLiteManager = new SQLiteManager();
            sqLiteManager.CreateActivityTable();
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
            GetTitleAndUrl(hwnd,DateTime.Now);
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


        private void GetTitleAndUrl(IntPtr hwnd, DateTime dt)
        {
            string url = "";
            string title = GetActiveWindowTitle();
            if (title == null)
                return;
            string browser = "";
            if (title.Trim() != prevTitle.Trim())
            {

                prevTitle = title;
                if (title.Contains("- Brave"))
                {
                    browser = "brave";

                    url = GetBraveUrl(hwnd);


                }
                else if (title.Contains("- Google Chrome"))
                {
                    browser = "chrome";
                    url = GetChromeUrl(hwnd);


                }
                else if (title.Contains("- Microsoft?"))
                {
                    browser = "edge";
                    url = GetEdgeUrl(hwnd);

                }
                else if (title.Contains("— Mozilla Firefox"))
                {
                    browser = "firefox";
                    url = GetFirefoxUrl(hwnd);
                }
      
                if (!string.IsNullOrEmpty(browser))
                {

                    if (string.IsNullOrEmpty(url))
                        url = GetBrowsedUrlFromHwnd(hwnd).ToString();
                    if (string.IsNullOrEmpty(url))
                    {
                        url = "No Property for Doc:" + searchBrowserForUrl(browser);
                    }
         
                    
    
                    //Log.Text = title + "\r\n\t" + url + "\r\n" + Log.Text;
                }
                
                    //Log.Text = title + "\r\n" + Log.Text;

                Activity activity = new Activity();
                activity.Title = title;
                activity.Url = url;
                activity.Time = dt.ToString();
                sqLiteManager.SaveActivity(activity);
                limitedActivity =sqLiteManager.SelectNumActivity(5);
                DataTable dataTable = new DataTable();


                
       
                    dataGrid.ItemsSource = limitedActivity;
                    dataGrid.Columns[1].Width = 300;
                    dataGrid.Columns[2].Width = 200;
          

                return;
            }


        }

        public string GetEdgeUrl(IntPtr hwnd)
        {
            AutomationElement element = AutomationElement.FromHandle(hwnd);  // intPtr is the MainWindowHandle for FireFox browser

            AutomationElement col = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "Microsoft Edge"));
            if (col == null)
            {
                return "Empty url 0000";
            }

            col = col.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
            col = col.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));


            string url = (string)col.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
            if (string.IsNullOrEmpty(url))
            {
                return "Empty url 0001";
            }
            return url;
        }


        public string GetFirefoxUrl(IntPtr hwnd)
        {
            AutomationElement element = AutomationElement.FromHandle(hwnd);  // intPtr is the MainWindowHandle for FireFox browser
            element.SetFocus();
            AutomationElement col = element.FindFirst(TreeScope.Subtree, new AndCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar), new PropertyCondition(AutomationElement.NameProperty, "Navigation")));
            if (col == null)
            {
                return "Empty url 0000";
            }

            col = col.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox));
            col = col.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));


            string url = (string)col.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
            if (string.IsNullOrEmpty(url))
            {
                return "Empty url 0001";
            }
            return url;
        }
        public string GetChromeUrl(IntPtr hwnd)
        {
            AutomationElement element = AutomationElement.FromHandle(hwnd);  // intPtr is the MainWindowHandle for FireFox browser
            AutomationElement col = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
            if (col == null)
            {
                return "Empty url 0000";
            }
            col = col.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));

            AutomationElement col1 = col.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
            if (col1 == null)
            {
                return "Unable to locate address and search bar";
            }

            string url = (string)col1.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);

            Console.WriteLine(url);
            if (string.IsNullOrEmpty(url))
            {
                return "Empty url 0001";
            }

            return url;
        }
        public string GetBraveUrl(IntPtr hwnd)
        {
            AutomationElement element = AutomationElement.FromHandle(hwnd); 

            AutomationElement col = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Brave"));
            if (col == null)
            {
                return "Empty url 0000";
            }

            AutomationElement col1 = col.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
            //col1 = col1.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Custom));
            if (col1 == null)
            {
                return "Unable to locate address and search bar";
            }
            //cacheRequest.Pop();
            string url = (string)col1.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);

            Console.WriteLine(url);
            if (string.IsNullOrEmpty(url))
            {
                return "Empty url 0001";
            }

            return url;
        }
        public void findAllNameProperty(AutomationElementCollection col)
        {

        }
        public string GetBrowsedUrlFromHwnd(IntPtr hwnd)
        {

            string rString = "";
            if (hwnd == IntPtr.Zero || hwnd == null) { return "No Pointer"; }
            AutomationElement element = AutomationElement.FromHandle(hwnd);
            if (element == null)
                return "ele";

            AutomationElement doc = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));



            if (doc != null)
                rString = (string)element.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);

            if (doc == null)
            {
                return "";
            }

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
        private string searchBrowserForUrl(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            string url = "";
            for (int i = 0; i < processes.Length; i++)
            {
                Process p = processes[i];
                if (p.MainWindowHandle == IntPtr.Zero)
                    continue;

        
                    url = getProcessUrl(p).ToString();
                if (!string.IsNullOrEmpty(url) && url != "zero" && url != "No Property for Doc from Process Search")
                    return url;
            }
            return "No Url Located";
        }
        public string getProcessUrl(Process process)
        {
            Console.Write(process.ProcessName);

            if (process == null)
                throw new ArgumentNullException("process");

            if (process.MainWindowHandle == IntPtr.Zero)
                return "No hwnd";

            AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element == null)
                return "No element from hwnd";

            AutomationElement doc = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
            if (doc == null)
                return "No Property for Doc from Process Search";

            if (doc != null)
                return (string)doc.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);

            return "";



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
   
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            sqLiteManager.CloseConn();
        }
    }



}
