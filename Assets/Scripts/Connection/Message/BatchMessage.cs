using System.Collections.Generic;
using UnityEngine;

public class BatchMessage : Message
{
    public override string code { get { return "0"; } }

    public List<SoldierListMessage> content = new List<SoldierListMessage>();
}