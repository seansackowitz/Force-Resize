using System;
using System.Diagnostics;
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Force_Resize
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        static string FILENAME = "", WINDOWNAME = "";
        static string PARAMETERS = "";
        static int WIDTH, HEIGHT, POSX, POSY;

        static void Main(string[] args)
        {
            if (!ParseXMLFile())
                return;

            Process p = Process.Start(FILENAME, PARAMETERS);
            p.WaitForInputIdle(500);
            while (true)
            {
                foreach (IntPtr h in EnumerateProcessWindowHandles(p.Id))
                {
                    StringBuilder sB = new StringBuilder(256);
                    GetWindowText(h, sB, 256);
                    if (sB.ToString().Contains(WINDOWNAME))
                    {
                        MoveWindow(h, POSX, POSY, WIDTH, HEIGHT, true);
                        return;
                    }
                }
            }
        }

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id, (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }

        static bool ParseXMLFile()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("prefs.xml");
                XmlNode node = doc.SelectSingleNode("/prefs");
                foreach (XmlAttribute x in node.Attributes)
                {
                    switch (x.Name.ToLower())
                    {
                        case "width":
                            WIDTH = int.Parse(x.Value);
                            break;
                        case "height":
                            HEIGHT = int.Parse(x.Value);
                            break;
                        case "posx":
                            POSX = int.Parse(x.Value);
                            break;
                        case "posy":
                            POSY = int.Parse(x.Value);
                            break;
                        case "application":
                            FILENAME = x.Value;
                            break;
                        case "windowname":
                            WINDOWNAME = x.Value;
                            break;
                        case "parameters":
                            PARAMETERS = x.Value;
                            break;
                        default:
                            break;
                    }
                }

                return true;
            }
            catch { return false; }
        }
    }
}
