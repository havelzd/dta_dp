using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UI;
using UnityEngine;

public class ActiveMissionScreenController : MonoBehaviour
{

    public string missionId;
    public UIManager manager;
    public MissionController missionController;

    public GameObject missionIdText;
    public TextMeshProUGUI detail;

    // Start is called before the first frame update
    void Start()
    {
        missionController.GetComponent<MissionController>().DataReceivedEvent.AddListener(OnDataReceived);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopMission()
    {
        this.manager.OnMission(false);
        this.missionController.StopMission();
    }

    private void OnDataReceived(Dictionary<string, OperativeWrapper> data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Operative \t\t\t Location \t\t\t MLS");

        foreach (var pair in data)
        {
            var operativeKey = int.Parse(pair.Key);
            OperativeData lastData = pair.Value.data.Last();
            Location loc = lastData.location;
            string record = String.Format("Operative: {0} \t at [{1}, {2}, {3}] \t {4}", operativeKey, loc.lat, loc.lon, loc.altitude, lastData.mls);
            sb.AppendLine(record);
        }

        detail.text = sb.ToString();

    }
}
