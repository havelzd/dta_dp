using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public long mission_id{ get; set; }

    public string name{ get; set; }

    public string description { get; set; }

    public Squad[] military_hieararchy { get; set; }
}
