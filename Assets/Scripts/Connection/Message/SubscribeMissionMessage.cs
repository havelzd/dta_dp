using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeMissionMessage : Message
{
    public override string code { get { return "100";  } }
    public long mission_id { get; set; }
    public long delay { get; set; }

}
