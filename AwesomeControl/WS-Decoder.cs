using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepLogic.Shared;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AwesomeControl
{
    class WS_Decoder
    {
        public static void ReceivedDataHandler(string data, IWebSocketConnection client)
        {
            try
            {
                
                AwesomePacket packet = Newtonsoft.Json.JsonConvert.DeserializeObject<AwesomePacket>(data);
                Logger.Instance.Log(new Logger.LogEntry()
                {
                    Message = packet.Body.ToString(),
                    Origin = "Decoder",
                    Time = DateTime.Now,
                    Type = Logger.SeverityClass.DEBUG
                });
                switch (packet.Header)
                {
                    case PacketType.GET_MODULES:
                        client.Send(Newtonsoft.Json.JsonConvert.SerializeObject(new AwesomePacket()
                                                                                    {
                                                                                        Header = PacketType.GET_MODULES,
                                                                                        Body = AwesomeModules.AvailableModules
                                                                                    }));
                        break;
                    case PacketType.ADD_MODULE:
                        AwesomeModules.AvailableModules.Add(new AwesomeModules.AppModule()
                                                                {
                                                                    command = packet.Body.Command,
                                                                    description = packet.Body.Description,
                                                                    name = packet.Body.Name,
                                                                    thumbnail = packet.Body.Thumbnail
                                                                });
                        break;
                    case PacketType.DELETE_MODULE:
                        AwesomeModules.AvailableModules.Remove(AwesomeModules.AvailableModules.Find(module => module.name == packet.Body.Name));
                        break;
                    case PacketType.MODULE_ACTION:
                        AppPacket ap = new AppPacket()
                                           {
                                               Request = packet.Body.Request,
                                               Command = packet.Body.Command,
                                               Monitor = packet.Body.Monitor
                                           };
                        break;
                    case PacketType.SHUTDOWN:
                        throw new NotImplementedException();
                        break;
                    case PacketType.TERMINAL:
                        throw new NotImplementedException();
                        break;
                    default:
                        Logger.Instance.Log(new Logger.LogEntry()
                        {
                            Message = String.Format("Unknown request received by {0}: {1}", client.ConnectionInfo.ClientIpAddress, packet.Header),
                            Origin = "Decoder",
                            Time = DateTime.Now,
                            Type = Logger.SeverityClass.WARNING
                        });
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Log(new Logger.LogEntry()
                {
                    Message = String.Format("Malformed packet received by {0}", client.ConnectionInfo.ClientIpAddress),
                    Origin = "Decoder",
                    Time = DateTime.Now,
                    Type = Logger.SeverityClass.WARNING
                });
            }
        }

        public class AwesomePacket
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public PacketType Header;
            public dynamic Body;
        }

        public class AppPacket
        {
            public string Request;
            public string Monitor;
            public string Command;

            public override string ToString()
            {
                return String.Format("Request [{0}], Monitor [{1}], Command: [{2}]", Request ?? "N/A", Monitor ?? "N/A", Command ?? "N/A");
            }
        }

        public enum PacketType
        {
            GET_MODULES,
            ADD_MODULE,
            DELETE_MODULE,
            MODULE_ACTION,
            SHUTDOWN,
            TERMINAL
        }

        public void ProcessAppPacket(AppPacket packet)
        {
            switch (packet.Request.ToUpper())
            {
                case "ADD":
                    break;
                case "REMOVE":
                    break;
            }
        }
    }
}
