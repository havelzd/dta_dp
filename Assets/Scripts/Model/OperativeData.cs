using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

public class OperativeData
{
    public string id { get; set; }
    
    public Location location { get; set; }
    
    public short mls { get; set; }

    public int missionId { get; set; }

    public long lastTimestamp { get; set; }
}
