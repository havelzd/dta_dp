using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public long mission_id{ get; set; }

    public string name{ get; set; }

    public string description { get; set; }

    public List<Squad> military_hieararchy = new();
}
