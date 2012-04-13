using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepLogic.Shared;

namespace AwesomeControl
{
    class ProcessManager
    {
        List<XController> XWindows = new List<XController>();
        private XController parent;
        private static ProcessManager instance;
        public ProcessManager(XController parent)
        {
            this.parent = parent;
            instance = this;
        }

        private static bool WaitforHWND(ModuleController controller)
        {
            for (int i = 0; i < 100; i++)
            {
                //Always wait for ~100ms to account for hidden windows that happen to be starting up first
                System.Threading.Thread.Sleep(100);
                if (controller.WindowHandles.Count == 0)
                    continue;
                return true;
            }
            return false;
        }

        public static void SetPos(ModuleController module, System.Drawing.Point offset, System.Drawing.Point size)
        {
            Logger.Instance.Log(new Logger.LogEntry() { Message = String.Format("Setting module: {0} offset {1} size {2}", module, offset, size), Origin = "ProcessManager", Time = DateTime.Now, Type = Logger.SeverityClass.INFO });
            WaitforHWND(module);
            foreach (var HWnd in module.WindowHandles)
            {
                Environment.SetEnvironmentVariable("DISPLAY", String.Format(":{0}", instance.parent.Display));
                Process.Start(new ProcessStartInfo("wmctrl", String.Format("-i -r {0} -e 0,{1},{2},{3},{4}", HWnd, offset.X, offset.Y, size.X, size.Y)) {UseShellExecute = false, RedirectStandardInput = true, RedirectStandardError = true, RedirectStandardOutput = true});
            }
            if (module.WindowHandles.Count == 0)
            {
                Logger.Instance.Log(new Logger.LogEntry() { Message = String.Format("Unable to acquire window handle for module: {0}", module), Origin = "ProcessManager", Time = DateTime.Now, Type = Logger.SeverityClass.WARNING });
            }
        }

        public static void Maximize(ModuleController module)
        {
            Logger.Instance.Log(new Logger.LogEntry() {Message = String.Format("Maximizing module: {0}", module), Origin = "ProcessManager", Time = DateTime.Now, Type = Logger.SeverityClass.INFO});
            WaitforHWND(module);
            foreach (var HWnd in module.WindowHandles)
            {
                Environment.SetEnvironmentVariable("DISPLAY", String.Format(":{0}", instance.parent.Display));
                Process.Start(new ProcessStartInfo("wmctrl", String.Format("-i -r {0} -b add,maximized_vert,maximized_horz", HWnd)) {UseShellExecute = false, RedirectStandardInput = true, RedirectStandardError = true, RedirectStandardOutput = true});
            }
            if (module.WindowHandles.Count == 0)
            {
                Logger.Instance.Log(new Logger.LogEntry() {Message = String.Format("Unable to acquire window handle for module: {0}", module), Origin = "ProcessManager", Time = DateTime.Now, Type = Logger.SeverityClass.WARNING});
            }
        }

        public static void Fullscreen(ModuleController module)
        {
            Logger.Instance.Log(new Logger.LogEntry() { Message = String.Format("Fullscreening module: {0}", module), Origin = "ProcessManager", Time = DateTime.Now, Type = Logger.SeverityClass.INFO });
            WaitforHWND(module);
            foreach (var HWnd in module.WindowHandles)
            {
                Environment.SetEnvironmentVariable("DISPLAY", String.Format(":{0}", instance.parent.Display));
                Process.Start(new ProcessStartInfo("wmctrl", String.Format("-i -r {0} -b add,fullscreen", HWnd)) { UseShellExecute = false, RedirectStandardInput = true, RedirectStandardError = true, RedirectStandardOutput = true });
            }
            if (module.WindowHandles.Count == 0)
            {
                Logger.Instance.Log(new Logger.LogEntry() { Message = String.Format("Unable to acquire window handle for module: {0}", module), Origin = "ProcessManager", Time = DateTime.Now, Type = Logger.SeverityClass.WARNING });
            }
        }

        public static void SetMonitor(ModuleController module, MonitorController monitor)
        {
            Logger.Instance.Log(new Logger.LogEntry() { Message = String.Format("Assigning module: {0} to Monitor: {1}",module,monitor), Origin = "ProcessManager", Time = DateTime.Now, Type = Logger.SeverityClass.INFO });
            monitor.Module = module;
        }

        public void StartModule(AwesomeModules.AppModule app, MonitorController monitor)
        {
            ModuleController module = instance.parent.StartModule(app);
            SetMonitor(module,monitor);
        }
    }
}
