using Convertor;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MissionScreenController : MonoBehaviour
{

    public AppConfig config;
    
    [SerializeField] private List<Mission> missions;

    public GameObject missionBtnPrefab;
    public GameObject missionBtnsPanel;
    public MissionController missionController;

    public UIManager manager;

    // Start is called before the first frame update
    void Start()
    {
        if (config.isDebug)
        {
            DisplayMissionList(new() { 
                new Mission() { mission_id = 1, name = "Mission Test 1", description = "Test" },
                new Mission() { mission_id = 2, name = "Mission Test 2", description = "Test2" },
                new Mission() { mission_id = 3, name = "Mission Test 3", description = "Test3" },
                new Mission() { mission_id = 4, name = "Mission Test 4", description = "Test4" },
            }) ;
        } else
        {
            StartCoroutine(ListMissions());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator ListMissions()
    {
        string url = String.Format("http://{0}:{1}/api/recorded-mission", config.serverHost, config.serverPort);
        Debug.Log(url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Accept", "text/html");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("GetRequestError " + request.error);
            //uiManager.GetComponent<UIManager>().DisplayWarning(request.result.ToString(), request.error);
        }
        else
        {
            Debug.Log("Received resposne " + request.result);
            List<Mission> missionList = MissionConverter.deserializeReadyMissions(request.downloadHandler.text);
            DisplayMissionList(missionList);
        }
        yield break;
    }

    public void DisplayMissionList(List<Mission> missions)
    {
        Debug.Log("Display Mission List" + missions);
        this.missions = missions;
        missions.ForEach(mission =>
        {
            Debug.Log(mission.name);
            GameObject missionBtn = Instantiate(missionBtnPrefab, missionBtnsPanel.transform);
            TextMeshProUGUI buttonText = missionBtn.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = mission.name;

            Button button = missionBtn.GetComponentInChildren<Button>();
            button.onClick.AddListener((() => OnMissionSelected(mission.name)));
        });
    }

    public void OnMissionSelected(String missionName)
    {
        Debug.Log(missionName + " SELECTED");
        Mission mission = missions.Find(mission => mission.name == missionName);
        if (mission != null)
        {
            missionController.GetComponent<MissionController>().StartMission(mission);
            manager.OnMission(true);
        }

    }
}
