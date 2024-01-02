using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location
{
    public float lat {  get; set; }
    public float lon { get; set; }
    public float altitude { get; set; }

    public bool Equals(Location other)
    {
        return lat == other.lat && lon == other.lon && altitude == other.altitude;
    }
}
