using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepLogic.Shared;

namespace AwesomeControl
{
    public class XController
    {
        private Process x11;
        private List<ModuleController> x11modules = new List<ModuleController>();
        private List<MonitorController> x11monitors = new List<MonitorController>();
        public List<MonitorController> AvailableMonitors
        {
            get { return x11monitors; }
        }
        public readonly int Display;

        public XController(int Display, bool useExisting = false, bool killExisting = true)
        {
            this.Display = Display;
            if (!useExisting)
            {
                if (killExisting)
                {
                    //Kill KDM/GDM/LXDM
                    Logger.Instance.Log(new Logger.LogEntry() {Message = "Nuking DMs", Origin = "ProcessManager", Time = DateTime.Now, Type = Logger.SeverityClass.INFO});

                    System.Diagnostics.Process.Start(new ProcessStartInfo("service", "kdm stop") {UseShellExecute = false, RedirectStandardError = true, RedirectStandardOutput = true});
                    System.Diagnostics.Process.Start(new ProcessStartInfo("service", "lxdm stop") {UseShellExecute = false, RedirectStandardError = true, RedirectStandardOutput = true});
                    System.Diagnostics.Process.Start(new ProcessStartInfo("service", "gdm stop") {UseShellExecute = false, RedirectStandardError = true, RedirectStandardOutput = true});
                    System.Diagnostics.Process.Start(new ProcessStartInfo("pkill", "X") {UseShellExecute = false, RedirectStandardError = true, RedirectStandardOutput = true});
                }
                Logger.Instance.Log("XController", Logger.SeverityClass.INFO, String.Format("Starting X11 Server on DISPLAY {0}", Display));
                ProcessStartInfo processStart = new ProcessStartInfo("X", String.Format(":{0} -nr -nolisten tcp", Display)) {UseShellExecute = false, RedirectStandardError = true, RedirectStandardInput = true, RedirectStandardOutput = true};
                x11 = Process.Start(processStart);
            }
        }

        public void Stop()
        {
            foreach (var x11Module in x11modules)
            {
                x11Module.Stop();
            }
            if (x11 != null)
                x11.Kill();
        }

        public void Start()
        {
            if (x11 != null)
                x11.Start();
        }

        public ModuleController StartModule(AwesomeModules.AppModule module)
        {
            ModuleController m = new ModuleController(this, module);
            x11modules.Add(m);
            return m;
        }

        public void SetPriority(ProcessPriorityClass level)
        {
            if (x11 != null)
                x11.PriorityClass = level;
        }

        public void AddMonitor(int ID, System.Drawing.Point Size, System.Drawing.Point Offset)
        {
            var monitor = new MonitorController(this, ID, Size, Offset);
            Logger.Instance.Log("XController", Logger.SeverityClass.INFO, String.Format("Adding a new monitor: {0}", monitor));
            AvailableMonitors.Add(monitor);
        }
    }
}
