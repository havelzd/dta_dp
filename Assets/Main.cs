using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Connection;
using Convertor;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketUtils;

public class Main : MonoBehaviour
{

    public AppConfig config;
    
    private readonly WSChannel wsChannel = null;
    
    public GameObject uiManager;
    
    public Main()
    {
        //wsChannel = WsConnector.createChannel(WsConnector.MISSION_DATA);
    }
    
    // Start is called before the first frame update
    void Start()
    {
     
        //StartCoroutine(ListMissions());
        
        if (wsChannel == null)
        {
        }
        else
        {
            wsChannel.OnMessageRec.AddListener(OnMessageReceived);
            Debug.Log("Starting Channel");
            wsChannel.Start();
            // subscribe to given mission
            // TODO: remove hardcoded mission, come up with better approach
            wsChannel.Send(new SubscribeMissionMessage { mission_id = 20, delay = 1000 }) ;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMessageReceived(Message message)
    {
        Debug.Log("Received Message in Main");
        Debug.Log(message);
    }
    
    private void OnDestroy()
    {
        if(wsChannel != null)
        {
            this.wsChannel.Stop();
        }
        
    }
    
    //private IEnumerator ListMissions()
    //{
    //    string url = String.Format("http://{0}:{1}/api/recorded-mission", config.serverHost, config.serverPort);
    //    Debug.Log(url);

    //    UnityWebRequest request = UnityWebRequest.Get(url);
    //    request.SetRequestHeader("Accept", "text/html");

    //    yield return request.SendWebRequest();

    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log("GetRequestError " + request.error);
    //        uiManager.GetComponent<UIManager>().DisplayWarning(request.result.ToString(), request.error);
    //    }
    //    else
    //    {
    //        List<Mission> missionList = MissionConverter.deserializeReadyMissions(request.downloadHandler.text);
    //        Debug.Log("missionList");
    //        Debug.Log(missionList);
    //        uiManager.GetComponent<UIManager>().DisplayMissionList(missionList);
    //    }
    //    yield break;
    //}
    
}
