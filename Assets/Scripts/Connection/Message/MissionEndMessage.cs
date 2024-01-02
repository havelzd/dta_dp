using UnityEngine;

public class MissionEndMessage: Message
{

    public override string code { get { return "2"; } }

    public int mission_id { get; set; }
}
