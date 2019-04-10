using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Walterlv
{
    public class QQChat
    {
        public string Name { get; private set; }
        private IntPtr Hwnd;

        public async Task<bool> SendMessageAsync(string text)
        {
            SwitchToThisWindow(Hwnd, true);

            Clipboard.SetDataObject(text, true, 3, 100);

            await Task.Delay(80);
            SendKeys.SendWait("^a");
            await Task.Delay(50);
            SendKeys.SendWait("^v");
            await Task.Delay(50);
            SendKeys.SendWait("{Enter}");

            return true;
        }

        public static IReadOnlyList<QQChat> Find()
        {
            var groupList = new List<QQChat>();
            EnumWindows(OnWindowEnum, 0);
            return groupList;

            bool OnWindowEnum(int hwnd, int lparam)
            {
                if (GetParent(hwnd) == 0 && IsWindowVisible(hwnd) == 1)
                {
                    StringBuilder lptrString = new StringBuilder(512);
                    StringBuilder lpString = new StringBuilder(512);
                    GetClassName(hwnd, lpString, lpString.Capacity);
                    if (lpString.ToString().Equals("txguifoundation", StringComparison.InvariantCultureIgnoreCase))
                    {
                        GetWindowText(hwnd, lptrString, lptrString.Capacity);
                        string name = lptrString.ToString().Trim();
                        if (name.Length > 0)
                        {
                            groupList.Add(new QQChat
                            {
                                Name = name,
                                Hwnd = new IntPtr(hwnd),
                            });
                        }
                    }
                }

                return true;
            }
        }

        public delegate bool WndEnumProc(int hwnd, int lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(WndEnumProc lpEnumFunc, int lParam);

        [DllImport("user32")]
        public static extern int GetParent(int hwnd);

        [DllImport("user32")]
        public static extern int IsWindowVisible(int hwnd);

        [DllImport("user32")]
        public static extern int GetWindowText(int hwnd, StringBuilder lptrString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassName(int hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strclassName, string strWindowName);
    }
}