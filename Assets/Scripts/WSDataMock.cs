using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WSDataMock : MonoBehaviour, ConnectionHandler
{

    public UnityEvent<Message> OnMessageRec = new UnityEvent<Message>();
    private List<BatchMessage> data = new();
    private int messageIndex = 0;
    private float updateInterval = 1f / 30f; // Update 30 times per second
    private float timeSinceLastUpdate = 0f;
    private bool missionStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        FillData();
    }

    // Update is called once per frame
    void Update()
    {

        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate < updateInterval)
        {
            return;
        }

        timeSinceLastUpdate = 0f;
        if (missionStarted)
        {
            YieldMessage();
        }
    }


    private void FillData()
    {

        SoldierListMessage m0 = new();
        m0.payload = new()
            {
                CreateOperative("1", 50.08257f, 14.49622f, 316.33f, 90),
                //CreateOperative("2", 50.08255f, 14.49644f, 297f, 20),
            };

        BatchMessage batch = new BatchMessage();
        batch.content = new()
            {
                m0
            };

        data.Add(batch);
    }

    private OperativeData CreateOperative(String id, float lat, float lon, float alt, short mls)
    {
        OperativeData operative = new();
        operative.id = id;
        operative.location = new();
        operative.location.lat = lat;
        operative.location.lon = lon;
        operative.location.altitude = alt;
        operative.mls = mls;

        return operative;
    }

    void YieldMessage()
    {
        OnMessageRec.Invoke(data[messageIndex]);
        if (messageIndex < data.Count - 1)
        {
            messageIndex++;
            messageIndex %= data.Count;
        }
    }

    public void StartMission(int missionId)
    {
        missionStarted = true;
    }

    public void StopMission(int missionId)
    {
        messageIndex = 0;
        missionStarted = false;
    }

    public void ListenToMissionData(UnityAction<Message> call)
    {
        OnMessageRec.AddListener(call);
    }

    public IEnumerator ListMissions(UnityAction<List<Mission>> onResult, UnityAction<string> onError)
    {

        Mission mission1 = new() { name = "Test 1", mission_id = 1 };
        Mission mission2 = new() { name = "Test 2", mission_id = 2 };
        List<Mission> missions = new() {mission1, mission2 };
        onResult?.Invoke(missions);

        yield break;
    }

    public IEnumerator GetMissionDetail(int missionId, UnityAction onResult, UnityAction<string> onError)
    {
        yield break;
    }
}
