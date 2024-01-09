using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

namespace WebSocketUtils
{
    public class WSChannelMock : Channel

        //new Mission() { mission_id = 1, name = "Mission Test 1", description = "Test" },
        //        new Mission() { mission_id = 2, name = "Mission Test 2", description = "Test2" },
        //        new Mission() { mission_id = 3, name = "Mission Test 3", description = "Test3" },
        //        new Mission() { mission_id = 4, name = "Mission Test 4", description = "Test4" },
    {

        private int messageIndex = 0;

        private List<BatchMessage> data = new();

        public WSChannelMock(WebSocket ws)
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

            FillData();

            ws.OnMessage += OnMessage;
        }

        private void FillData()
        {
           
            SoldierListMessage m0 = new();
            m0.payload = new()
            {
                CreateOperative("1", 50.08278f, 14.49621f, 291f, 100),
                CreateOperative("2", 50.08318f, 14.49592f, 309f, 50),
                CreateOperative("3", 50.08401f, 14.49513f, 296f, 3)
            };

            BatchMessage batch = new BatchMessage();
            batch.content = new()
            {
                m0
            };

            data.Add(batch);
        }

        private OperativeData CreateOperative(String id, float lat, float lon, float alt, short mls)
        {
            OperativeData operative = new();
            operative.id = id;
            operative.location = new();
            operative.location.lat = lat;
            operative.location.lon = lon;
            operative.location.altitude = alt;
            operative.mls = mls;

            return operative;
        }

        protected virtual void OnMessage(object sender, MessageEventArgs e)
        {
            Debug.Log("Received message: " + e.Data);
            Debug.Log("Mock " + data[messageIndex]);
            OnMessageRec.Invoke(data[messageIndex]);
            if (messageIndex < data.Count - 1)
            {
                messageIndex++;
            }
        }

        public override void Send(Message message)
        {
            if (message == null)
            {
                return;
            }

            string jsonMessage = MessageConverter.serialize(message);
            Debug.Log("Sending " + jsonMessage);
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