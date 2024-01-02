using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Convertor
{
    public class MissionConverter
    {
        public static List<Mission> deserializeReadyMissions(string message)
        {
            try
            {
                List<Mission> result = null;
                result = JsonConvert.DeserializeObject<List<Mission>>(message);
 

                return result;
            }
            catch (Exception e)
            {
                Debug.Log("Error while deserializing, " + e.ToString());
                return null;
            }


        }
    }
}