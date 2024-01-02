using System.Collections.Generic;
using UnityEngine;

public class SoldierListMessage : Message
{
    public override string code { get { return "3"; } }

    public List<OperativeData> payload = new List<OperativeData>();
}
