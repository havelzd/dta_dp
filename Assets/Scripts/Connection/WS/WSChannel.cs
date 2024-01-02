using UnityEngine;
using UnityEngine.Events;
using System;

using WebSocketSharp;

namespace WebSocketUtils
{
    public class WSChannel : Channel
    {

        public WSChannel(WebSocket ws)
        {
            this.ws = ws;
            OnMessageRec = new UnityEvent<Message>();

            ws.OnOpen += (sender, e) =>
            {
                if (ws != null && ws.ReadyState == WebSocketState.Open)
                {
                    Debug.Log("WebSocket initialized successfully.");
                }
                else
                {             
                    Debug.Log("Failed to initialize connection.");
                    throw new Exception("Failed to init WS connection.");
                }
            };

            ws.OnMessage += OnMessage;
        }

        protected virtual void OnMessage(object sender, MessageEventArgs e)
        {
            Debug.Log("Received message: " + e.Data);
            Message message = MessageConverter.deserialize(e.Data);
            OnMessageRec.Invoke(message);
        }

        public override void Send(Message message)
        {
            if (message == null) {
                return;
            }

            string jsonMessage = MessageConverter.serialize(message);
            Debug.Log("Sending " + jsonMessage);
            ws.Send(jsonMessage);
        }

        public override void Start()
        {
            ws.Connect();
        }

        public override void Stop()
        {
            ws.Close();
        }

    }

}