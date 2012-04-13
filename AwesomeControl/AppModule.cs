using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeControl
{
    public class AwesomeModules
    {
        public static List<AppModule> AvailableModules = new List<AppModule>()
                                                             {
                                                                 new AppModule() {name = "XTerm", description = "xterminal for testing", command = "/usr/bin/xterm", thumbnail = "xterm.png", windowhint = "xterm"},
                                                                 new AppModule() {name = "GLXGears", description = "GLX \"Gears\" Benchmark", command = "/usr/bin/glxgears", thumbnail = "glxgears.png", windowhint = "glxgears"}
                                                             };
        public struct AppModule
        {
            public string name;
            public string description;
            public string command;
            public string thumbnail;
            public string windowhint;
        }
    }
}
