using System;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Borderless
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start(args);
        }

        void Start(string[] args)
        {
            Process target;

            // No args given, display list of processes.
            if (args.Count() == 0)
            {
                DisplayProcesses();
                Console.WriteLine("Usage:");
                Console.WriteLine("    borderless [process name | id]");
                Console.WriteLine("    Example: borderless Albion-Online");
                return;
            }
            // Args were given process id.
            else if (HasArgPid(args[0]))
            {
                Console.WriteLine("Getting process with pid: " + args[0]);
                target = GetProcess(int.Parse(args[0]));

            }
            // Args were given process name.
            else
            {
                Console.WriteLine("Getting process with name: " + args[0]);
                target = GetProcess(args[0]);
            }

            if (target != null)
            {
                SetWindowStyle(target);
                SetWindowPosition(target);
            }

        }

        bool HasArgPid(string arg)
        {
            int pid;
            bool isNumber = int.TryParse(arg, out pid);
            if (isNumber && pid > 0)
                return true;
            else
                return false;
        }

        void DisplayProcesses()
        {
            foreach (Process p in Process.GetProcesses())
                PrintProcess(p);
        }

        Process GetProcess(int pid)
        {
            Process p = Process.GetProcessById(pid);
            if (p == null)
                Console.WriteLine("No process found with pid: " + pid);

            return p;
        }

        Process GetProcess(string name)
        {
            Process[] foundProcesses = Process.GetProcessesByName(name);

            // One match.
            if (foundProcesses.Count() == 1)
            {
                return foundProcesses[0];
            }
            // Multiple matches.
            else if (foundProcesses.Count() > 1)
            {
                foreach (Process p in foundProcesses)
                    PrintProcess(p);
                Console.WriteLine("Found multiple processes with name: " + name + ". Use pid instead");

                return null;
            }
            // No match.
            else
            {
                Console.WriteLine("No process found");
                return null;
            }
        }

        void SetWindowStyle(Process p)
        {
            Console.WriteLine("Setting window style");

            const int GWL_STYLE = -16;

            const int WS_CAPTION = 0x00C00000;
            const int WS_THICKFRAME = 0x00040000;
            const int WS_MINIMIZE = 0x20000000;
            const int WS_MAXIMIZE = 0x01000000;
            const int WS_SYSMENU = 0x00080000;

            IntPtr hWnd = p.MainWindowHandle;
            int style = GetWindowLong(hWnd, GWL_STYLE);

            SetWindowLong(hWnd, GWL_STYLE, (style & ~(WS_CAPTION | WS_THICKFRAME | WS_MINIMIZE | WS_MAXIMIZE | WS_SYSMENU)));
        }

        void SetWindowPosition(Process p)
        {
            Console.WriteLine("Setting window position");

            const int SWP_FRAMECHANGED = 0x0020;

            IntPtr hWnd = p.MainWindowHandle;

            Rectangle rect = new Rectangle(0,0,1920,1080);
            // Main display
            SetWindowPos(hWnd, 0, rect.X, rect.Y, rect.Width, rect.Height, SWP_FRAMECHANGED);
        }

        void PrintProcess(Process p)
        {
            int id = p.Id;
            string name = p.ProcessName;

            Console.WriteLine("{0, 5} " + name, id);
        }

        // EXTERNS
        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("USER32.DLL")]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int flags);
    }
}
