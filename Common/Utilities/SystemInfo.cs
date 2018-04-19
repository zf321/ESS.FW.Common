//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Management;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;

//namespace ESS.FW.Common.Utilities
//{
//    public class SystemInfo
//    {
//        private const int GW_HWNDFIRST = 0;
//        private const int GW_HWNDNEXT = 2;
//        private const int GWL_STYLE = (-16);
//        private const int WS_VISIBLE = 268435456;
//        private const int WS_BORDER = 8388608;
//        private readonly PerformanceCounter pcCpuLoad; //CPU计数器

//        #region 构造函数

//        /// <summary>
//        ///     构造函数，初始化计数器等
//        /// </summary>
//        public SystemInfo()
//        {
//            //初始化CPU计数器
//            pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");
//            pcCpuLoad.MachineName = ".";
//            pcCpuLoad.NextValue();

//            //CPU个数
//            ProcessorCount = Environment.ProcessorCount;

//            //获得物理内存
//            var mc = new ManagementClass("Win32_ComputerSystem");
//            var moc = mc.GetInstances();
//            foreach (ManagementObject mo in moc)
//            {
//                if (mo["TotalPhysicalMemory"] != null)
//                {
//                    PhysicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString());
//                }
//            }
//        }

//        #endregion

//        #region CPU个数

//        /// <summary>
//        ///     获取CPU个数
//        /// </summary>
//        public int ProcessorCount { get; private set; }

//        #endregion

//        #region CPU占用率

//        /// <summary>
//        ///     获取CPU占用率
//        /// </summary>
//        public float CpuLoad
//        {
//            get { return pcCpuLoad.NextValue(); }
//        }

//        #endregion

//        #region 可用内存

//        /// <summary>
//        ///     获取可用内存
//        /// </summary>
//        public long MemoryAvailable
//        {
//            get
//            {
//                long availablebytes = 0;
//                //ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PerfRawData_PerfOS_Memory");
//                //foreach (ManagementObject mo in mos.Get())
//                //{
//                //    availablebytes = long.Parse(mo["Availablebytes"].ToString());
//                //}
//                var mos = new ManagementClass("Win32_OperatingSystem");
//                foreach (ManagementObject mo in mos.GetInstances())
//                {
//                    if (mo["FreePhysicalMemory"] != null)
//                    {
//                        availablebytes = 1024*long.Parse(mo["FreePhysicalMemory"].ToString());
//                    }
//                }
//                return availablebytes;
//            }
//        }

//        #endregion

//        #region 物理内存

//        /// <summary>
//        ///     获取物理内存
//        /// </summary>
//        public long PhysicalMemory { get; private set; }

//        #endregion

//        #region 结束指定进程

//        /// <summary>
//        ///     结束指定进程
//        /// </summary>
//        /// <param name="pid">进程的 Process ID</param>
//        public static void EndProcess(int pid)
//        {
//            try
//            {
//                var process = Process.GetProcessById(pid);
//                process.Kill();
//            }
//            catch
//            {
//            }
//        }

//        #endregion

//        #region 查找所有应用程序标题

//        /// <summary>
//        ///     查找所有应用程序标题
//        /// </summary>
//        /// <returns>应用程序标题范型</returns>
//        public static List<string> FindAllApps(int Handle)
//        {
//            var Apps = new List<string>();

//            int hwCurr;
//            hwCurr = GetWindow(Handle, GW_HWNDFIRST);

//            while (hwCurr > 0)
//            {
//                var IsTask = (WS_VISIBLE | WS_BORDER);
//                var lngStyle = GetWindowLongA(hwCurr, GWL_STYLE);
//                var TaskWindow = ((lngStyle & IsTask) == IsTask);
//                if (TaskWindow)
//                {
//                    var length = GetWindowTextLength(new IntPtr(hwCurr));
//                    var sb = new StringBuilder(2*length + 1);
//                    GetWindowText(hwCurr, sb, sb.Capacity);
//                    var strTitle = sb.ToString();
//                    if (!string.IsNullOrEmpty(strTitle))
//                    {
//                        Apps.Add(strTitle);
//                    }
//                }
//                hwCurr = GetWindow(hwCurr, GW_HWNDNEXT);
//            }

//            return Apps;
//        }

//        #endregion

//        #region AIP声明

//        [DllImport("IpHlpApi.dll")]
//        public static extern uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

//        [DllImport("User32")]
//        private static extern int GetWindow(int hWnd, int wCmd);

//        [DllImport("User32")]
//        private static extern int GetWindowLongA(int hWnd, int wIndx);

//        [DllImport("user32.dll")]
//        private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

//        [DllImport("user32", CharSet = CharSet.Auto)]
//        private static extern int GetWindowTextLength(IntPtr hWnd);

//        #endregion
//    }
//}