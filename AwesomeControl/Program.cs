using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeepLogic.Net;
using DeepLogic.Object_Wrappers;
using DeepLogic.Shared;
using Fleck;
using WebSocketServer = DeepLogic.Net.WebSocketServer;

namespace AwesomeControl
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Pretty console on windows
            try
            {
                Console.BufferWidth = Console.LargestWindowWidth > 175 ? 175 : Console.LargestWindowWidth;
                Console.BufferHeight = Console.LargestWindowHeight - 5;
                Console.SetWindowSize(Console.BufferWidth, Console.BufferHeight);
            }
            catch
            {
            }

            Logger logger = new Logger(standalone: true)
                                {
                                    Banner = "SparkFun AwesomeScreen Manager",
                                    NodeID =
                                        {
#if DEBUG
                                            Console_ShowDebug = true
#endif
                                        },
                                    showmemusage = false,
                                    showversion = false
                                };
            //Print some sysinfo
            logger.PrintHeader();
            //Take over the console
            logger.Init();
            //Start a WebSocket server
            var server = new AuxSocketServer(1386);
            server.Start();
            var wsserver = new WebSocketServer("ws://192.168.10.77:1385");
            wsserver.Start();
            wsserver.ReceivedDataHandler += WS_Decoder.ReceivedDataHandler;
            //new WS_Server(ws_origin:new Uri("http://awesomescreen.internal.sparkfun.com:1385"));
            var xcontroller = new XController(0, true);
            var ProcessManager = new ProcessManager(xcontroller);

            //Ready to rock
            Logger.Instance.SetStatus(Logger.Status.READY);

            //Set up some monitors...
            xcontroller.AvailableMonitors.Add(new MonitorController(xcontroller, 0, new Point(1200, 200), new Point(200, 200)));
            xcontroller.AvailableMonitors.Add(new MonitorController(xcontroller, 1, new Point(1400, 200), new Point(200, 200)));
            xcontroller.AvailableMonitors.Add(new MonitorController(xcontroller, 2, new Point(1200, 400), new Point(200, 200)));
            xcontroller.AvailableMonitors.Add(new MonitorController(xcontroller, 3, new Point(1400, 400), new Point(200, 200)));

            //Start some default modules
            ProcessManager.StartModule(AwesomeModules.AvailableModules[0], xcontroller.AvailableMonitors[0]);
            ProcessManager.StartModule(AwesomeModules.AvailableModules[1], xcontroller.AvailableMonitors[1]);
            ProcessManager.StartModule(AwesomeModules.AvailableModules[0], xcontroller.AvailableMonitors[2]);
            ProcessManager.StartModule(AwesomeModules.AvailableModules[1], xcontroller.AvailableMonitors[3]);
        }
    }
}
