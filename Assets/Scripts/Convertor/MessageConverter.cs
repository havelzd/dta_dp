using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using WebSocketSharp;

public class MessageConverter 
{
    
    public static Message deserialize(string message)
    {
        try
        {
            JObject jObject = JObject.Parse(message);
            string code = jObject.GetValue("code").ToString();
            Message result = null;
            if (!code.IsNullOrEmpty())
            {
                switch (code)
                {
                    case "0":
                        result = JsonConvert.DeserializeObject<BatchMessage>(message);
                        break;
                    case "1":
                        result = JsonConvert.DeserializeObject<MissionStartMessage>(message);
                        break;
                    case "3":
                        result = JsonConvert.DeserializeObject<SoldierListMessage>(message);
                        break;
                    case "Welcome":
                    default:
                        return new HelloMessage();
                }
            }

            return result;
        } catch (Exception e)
        {
            Debug.Log("Error while deserializing, " + e.ToString());
            return null;
        }

        
    }

    public static string serialize(Message message)
    {
        return JsonConvert.SerializeObject(message);
    }

}