using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepLogic.Shared;

namespace AwesomeControl
{
    public class ModuleController
    {
        private XController parent;
        private Process x11module;
        private AwesomeModules.AppModule _module;
        public volatile List<string> WindowHandles = new List<string>();
        public AwesomeModules.AppModule Module
        {
            get { return _module; }
            set
            {
                if (x11module != null)
                    x11module.Kill();
                _module = value;
                Environment.SetEnvironmentVariable("DISPLAY", String.Format(":{0}", parent.Display));
                //x11module = Process.Start(new ProcessStartInfo(_module.command) { UseShellExecute = false, RedirectStandardError = true, RedirectStandardInput = true, RedirectStandardOutput = true});
                x11module = Process.Start(new ProcessStartInfo(_module.command) { UseShellExecute = false, RedirectStandardError = true, RedirectStandardInput = true, RedirectStandardOutput = true });
                while (!x11module.StandardError.EndOfStream)
                {
                    string line = x11module.StandardError.ReadLine();
                    if (line.Contains("XCreateWindow"))
                    {
                        Logger.Instance.Log("ModuleController", Logger.SeverityClass.INFO, line);
                        break;
                    }
                }
            }
        }

        public ModuleController(XController parent, AwesomeModules.AppModule module)
        {
            this.parent = parent;
            this._module = module;
            Environment.SetEnvironmentVariable("DISPLAY", String.Format(":{0}", parent.Display));
            Environment.SetEnvironmentVariable("LD_PRELOAD", "./xwrapper.so");
            Logger.Instance.Log("ModuleController", Logger.SeverityClass.INFO, String.Format("Starting module {0}", _module.name));
            try
            {
                x11module = Process.Start(new ProcessStartInfo(_module.command) { UseShellExecute = false, RedirectStandardError = true, RedirectStandardInput = true, RedirectStandardOutput = true });
                Logger.Instance.Log("ModuleController", Logger.SeverityClass.INFO, String.Format("Module {0} started with PID {1}", _module.name, x11module.Id));
                new System.Threading.Thread(ReadHWND).Start();
            }
            catch (Exception e)
            {
                Logger.Instance.Log("ModuleController", Logger.SeverityClass.WARNING, e.ToString());
            }
        }

        private void ReadHWND()
        {
            WindowHandles = new List<string>();
            while (!x11module.StandardError.EndOfStream)
            {
                string s = x11module.StandardError.ReadLine();
                if (s.Contains("HWND"))
                {
                    WindowHandles.Add(s.Split('=')[1]);
                    Logger.Instance.Log("ModuleController", Logger.SeverityClass.INFO, String.Format("Module {0} window {1} discovered", _module.name, s.Split('=')[1]));
                }
            }
        }

        public void Stop()
        {
            Logger.Instance.Log("ModuleController", Logger.SeverityClass.INFO, String.Format("Terminating module {0}", _module.name));
            x11module.Kill();
        }

        public void Start()
        {
            Logger.Instance.Log("ModuleController", Logger.SeverityClass.INFO, String.Format("Restarting module {0}", _module.name));
            Environment.SetEnvironmentVariable("DISPLAY", String.Format(":{0}", parent.Display));
            x11module.Start();
        }

        public void SetPriority(ProcessPriorityClass level)
        {
            Logger.Instance.Log("ModuleController", Logger.SeverityClass.INFO, String.Format("Setting priority for module {0} to {1}", _module.name, level));
            x11module.PriorityClass = level;
        }

        public int GetPID()
        {
            return x11module.Id;
        }

        public override string ToString()
        {
            return Module.name;
        }
    }
}
