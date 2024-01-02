using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

public class WSDataMock : MonoBehaviour
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

    public void StartMission()
    {
        missionStarted = true;
    }

    public void StopMission()
    {
        messageIndex = 0;
        missionStarted = false;
    }

    private void FillData()
    {

        SoldierListMessage m0 = new();
        m0.payload = new()
            {
                CreateOperative("1", 50.08278f, 14.49621f, 291f, 100),
                CreateOperative("2", 50.08318f, 14.49592f, 309f, 50),
                CreateOperative("3", 50.08401f, 14.49513f, 296f, 25)
            };

        //m0.payload = new()
        //{
        //     CreateOperative("1", 50.08252f, 14.49616f, 296f, 1)
        //};

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
}
