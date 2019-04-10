using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Walterlv
{
    public class QQGroup
    {
        public string Name { get; private set; }
        private int Hwnd;

        public static IReadOnlyList<QQGroup> Find()
        {
            var groupList = new List<QQGroup>();
            EnumWindows(OnWindowEnum, 0);
            return groupList;

            bool OnWindowEnum(int hwnd, int lparam)
            {
                if (GetParent(hwnd) == 0 && IsWindowVisible(hwnd) == 1)
                {
                    StringBuilder lptrString = new StringBuilder(512);
                    StringBuilder lpString = new StringBuilder(512);
                    GetClassName(hwnd, lpString, lpString.Capacity);
                    if (lpString.ToString().ToLower() == "txguifoundation")
                    {
                        GetWindowText(hwnd, lptrString, lptrString.Capacity);
                        string name = lptrString.ToString().Trim();
                        if (name.Length > 0)
                        {
                            groupList.Add(new QQGroup
                            {
                                Name = name,
                                Hwnd = hwnd,
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
    }
}