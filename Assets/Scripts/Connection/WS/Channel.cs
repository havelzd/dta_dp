using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

namespace WebSocketUtils
{
    public abstract class Channel
    {
        protected WebSocket ws;

        public UnityEvent<Message> OnMessageRec;

        public abstract void Send(Message message);

        public abstract void Start();

        public abstract void Stop();
       
    }
}