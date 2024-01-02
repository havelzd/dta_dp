using System.Collections;
using UnityEngine;

public class MissionStartMessage : Message
{
    public override string code { get { return "1"; } }

    public int mission_id { get; set; }
}