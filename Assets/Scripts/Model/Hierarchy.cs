
using System.Collections.Generic;

[System.Serializable]
public class Squad
{
    public string id { get; set; }
    public string leader { get; set; }
    public string name { get; set; }
    public List<Operative> soldiers = new List<Operative>();
}