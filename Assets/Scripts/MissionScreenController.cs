using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class MissionScreenController : MonoBehaviour
{
    [SerializeField] GameObject connectorGO;    
    

    private List<Mission> missions;
    private ConnectionHandler connector;

    public GameObject missionBtnPrefab;
    public GameObject missionBtnsPanel;
    public MissionController missionController;

    public UIManager manager;

    private void Awake()
    {
        connector = connectorGO.GetComponent<ConnectionHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(connector.ListMissions(DisplayMissionList, ErrorDialog.Instance.ShowMessage));
    }

    public void DisplayMissionList(List<Mission> missions)
    {
        this.missions = missions;
        missions.ForEach(mission =>
        {
            Debug.Log(mission.name);
            Debug.Log(mission.military_hieararchy);
            GameObject missionBtn = Instantiate(missionBtnPrefab, missionBtnsPanel.transform);
            TextMeshProUGUI buttonText = missionBtn.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = mission.name;

            Button button = missionBtn.GetComponentInChildren<Button>();
            button.onClick.AddListener((() => OnMissionSelected(mission.name)));
        });
    }

    public void OnMissionSelected(String missionName)
    {
        Mission mission = missions.Find(mission => mission.name == missionName);
        if (mission != null)
        {
            missionController.GetComponent<MissionController>().StartMission(mission);
            manager.OnMission(true);
        }

    }
}
