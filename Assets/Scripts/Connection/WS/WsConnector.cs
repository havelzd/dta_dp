using System;
using WebSocketSharp;
using WebSocketUtils;

namespace Connection.WS
{
    public class WsConnector
    { 

        public const string MISSION_DATA = "mission-data";

        public static Channel CreateChannel(string host, string port, string channel)
        {
            WebSocket ws = new WebSocket(String.Format("ws://{0}:{1}/ws/{2}", host, port, channel));
            return new WSChannel(ws);
        }
    }
}