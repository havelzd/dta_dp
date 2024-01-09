using Connection.WS;
using Convertor;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using WebSocketUtils;
using System.Collections;

public class ServerConnector : MonoBehaviour, ConnectionHandler
{
    [SerializeField] private AppConfig config;
    
    private Channel missionChannel;
    private int missionId = -1;

    void Awake()
    {
        missionChannel = WsConnector.CreateChannel(config.serverHost, config.serverPort, WsConnector.MISSION_DATA);
    }

    public void StartMission(int missionId)
    {
        this.missionId = missionId;
        missionChannel.Start();
        missionChannel.Send(new SubscribeMissionMessage { mission_id = missionId, delay =  config.delay});
    }

    public void StopMission(int missionId)
    {
        missionChannel.Send(new MissionEndMessage { mission_id = missionId });
        missionChannel.Stop();
        missionId = -1;
    }

    public void ListenToMissionData(UnityAction<Message> call)
    {
        missionChannel.OnMessageRec.AddListener(call);
    }

    public IEnumerator ListMissions(UnityAction<List<Mission>> onResult, UnityAction<string> onError)
    {
        string url = string.Format("http://{0}:{1}/api/recorded-mission", config.serverHost, config.serverPort);

        Debug.Log("Sending mission List message");

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Accept", "text/html");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("GetRequestError " + request.error);
            onError?.Invoke(request.error);
        }
        else
        {
            Debug.Log("Received response " + request.result + " " + request.downloadHandler.text);
            List<Mission> missionList = MissionConverter.deserializeReadyMissions(request.downloadHandler.text);
            onResult?.Invoke(missionList);
        }
        yield break;
    }

    public IEnumerator GetMissionDetail(int missionId, UnityAction onResult, UnityAction<string> onError)
    {
        string url = string.Format("http://{0}:{1}/api/recorded-mission/{2}", config.serverHost, config.serverPort, missionId);

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Accept", "text/html");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("GetRequestError " + request.error);
            onError?.Invoke(request.error);
        }
        else
        {
            Debug.Log("Received response " + request.result + " " + request.downloadHandler.text);
            //List<Mission> missionList = MissionConverter.deserializeReadyMissions(request.downloadHandler.text);
            //onResult?.Invoke(missionList);
        }
        yield break;
    }

    void OnDestroy()
    {
        if(missionId != -1)
        {
            StopMission(this.missionId);
        }
        
        missionChannel.Stop();
    }





}
