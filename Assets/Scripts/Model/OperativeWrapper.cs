using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperativeWrapper
{
    public List<OperativeData> data = new();
    
    public GameObject Operative3d { get; set; }
    public Renderer OperativeRenderer { get; set; }
    public GameObject cylinder { get; set; }
    public OperativeMarker objectMarker { get; set; }
    
    public GameObject UiWaypoint { get; set; }
    public OperativeMarker waypointMarker { get; set; }
};