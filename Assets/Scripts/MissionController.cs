using Google.XR.ARCoreExtensions;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class MissionController : MonoBehaviour
{
    [SerializeField] AREarthManager arEarthManager;
    [SerializeField] GameObject arAnchorManagerGO;
    [SerializeField] GameObject arCameraGO;
    [SerializeField] GameObject connectorGO;

    ARAnchorManager arAnchorManager;
    WaypointController waypointController;
    SettingsManager settingsManager;
    ConnectionHandler connector;
    LocationService locationService;

    public Dictionary<string, OperativeWrapper> operatives = new();
    public Dictionary<string, OperativeWrapper> squads = new();

    private List<List<SoldierListMessage>> queue = new();

    public GameObject operative2dPrefab;
    public GameObject operative3dPrefab;
    public Canvas canvas;
    Camera arCamera;
    

    private Mission selectedMission;
    private bool showSquad = false;
    

    public UnityEvent<Dictionary<string, OperativeWrapper>> DataReceivedEvent = new();

    void Awake()
    {
        locationService = LocationService.Instance;
        arAnchorManager = arAnchorManagerGO.GetComponent<ARAnchorManager>();
        waypointController = arCameraGO.GetComponent<WaypointController>();
        arCamera = arCameraGO.GetComponent<Camera>();
        
        settingsManager = SettingsManager.Instance;
        settingsManager.RegisterSettingsChangedCallback(LoadSettings);
        showSquad = PlayerPrefs.GetInt("showSquad", 0) == 1;

        connector = connectorGO.GetComponent<ConnectionHandler>();
    }

    void Start()
    {
        connector.ListenToMissionData(OnMessageReceived);
    }

    void LoadSettings()
    {
        bool sq = PlayerPrefs.GetInt("showSquad", 0) == 1;
        if (showSquad != sq)
        {
            showSquad = sq;
            ClearMarkers();  
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (queue.Count <= 0 || selectedMission  == null)
        {
            return;
        }

        Vector3 location = new(locationService.Latitude, locationService.Longitude, locationService.Altitude);

        List<SoldierListMessage> msg = queue[0];
        queue.RemoveAt(0);
        msg.ForEach(slm =>
        {
            slm.payload.ForEach(sd =>
            {
                
                OperativeWrapper operative;

                if (operatives.ContainsKey(sd.id))
                {
                    operative = operatives[sd.id];
                } else
                {
                    operative = new OperativeWrapper();
                }
                operative.lastData = sd;
                operatives[sd.id] = operative;
            });
        });

        UpdateSquadInfo();    

        if(!showSquad)
        {
            UpdateMarkers(operatives, location);
        }else
        {
            UpdateMarkers(squads, location);
        }

        DataReceivedEvent.Invoke(operatives);
    }

    private void UpdateSquadInfo()
    {
        foreach (var squad in selectedMission.military_hieararchy)
        {
            float lat = 0.0f, lon = 0.0f, alt = 0.0f;
            short mls = 0;
            foreach (var soldier in squad.soldiers)
            {
                Location loc = operatives[soldier.callSign].lastData.location;
                lat += loc.lat;
                lon += loc.lon;
                alt += loc.altitude;
                mls = Math.Max(mls, operatives[soldier.callSign].lastData.mls);
            }
            squads[squad.id].lastData.mls = mls;
            squads[squad.id].lastData.location = new() { lon = lon, lat = lat, altitude = alt };
        }
    }

    private void UpdateMarkers(Dictionary<string, OperativeWrapper> markers, Vector3 location)
    {
        foreach (var operative in markers.Values)
        {
            UpdateWrapper(operative, operative.lastData, location);
        }
    }

    private void UpdateWrapper(OperativeWrapper operative, OperativeData data, Vector3 _Location)
    {
        GameObject marker3d = operative.Operative3d;
        Vector3 pos;

        pos = CoordinatesTransformer.ConvertGeoToCartesian(_Location.x, _Location.y, _Location.z, data.location.lat, data.location.lon, data.location.altitude - arCamera.transform.position.y);
        if (operative.Operative3d == null)
        {
            marker3d = CreateMarker3D(operative, pos);
        }
        else
        {
            marker3d.transform.position = pos;
        }

        if (arEarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled) == FeatureSupported.Supported)
        {
            ARGeospatialAnchor anchor = null;
            try
            {
                anchor = ARAnchorManagerExtensions.AddAnchor(arAnchorManager, data.location.lat, data.location.lon, data.location.altitude, Quaternion.identity);
            }
            catch (Exception e)
            {
                ErrorDialog.Instance.ShowMessage(e.Message);
                Debug.LogError(e);
            }
            finally
            {

                if (anchor != null)
                {
                    if (marker3d.transform.parent != null)
                    {
                        Destroy(marker3d.transform.parent.gameObject);
                    }
                    marker3d.transform.parent = anchor.transform;
                }
            }
        }

        operative.Operative3d = marker3d;

        var uiMarker = operative.UiWaypoint;
        if (uiMarker == null)
        {
            uiMarker = Instantiate(operative2dPrefab, canvas.transform);
            operative.waypointMarker = uiMarker.GetComponent<OperativeMarker>();
            operative.UiWaypoint = uiMarker;
            waypointController.operatives.Add(operative);
            uiMarker.transform.SetAsFirstSibling();
        }

        operative.objectMarker.Data = data;
        operative.waypointMarker.Data = data;
    }

    private GameObject CreateMarker3D(OperativeWrapper operative, Vector3 pos)
    {
        var marker = Instantiate(operative3dPrefab, pos, Quaternion.identity);
        marker.GetComponentInChildren<Billboard>().arCamera = arCamera;
        operative.OperativeRenderer = marker.GetComponentInChildren<MeshRenderer>();
        operative.objectMarker = marker.GetComponentInChildren<OperativeMarker>();
        operative.cylinder = marker.transform.Find("Cylinder").gameObject;
        operative.Operative3d = marker;

        return marker;
    }

    public void StartMission(Mission mission)
    {
        Debug.Log("Starting mission " + mission.mission_id);
        StartCoroutine(connector.GetMissionDetail((int)mission.mission_id, null, null));
        selectedMission = mission;
        Debug.Log(mission.military_hieararchy.Count);
        mission.military_hieararchy.ForEach(squad =>
        {
            var wrapper = new OperativeWrapper();
            wrapper.lastData = new OperativeData();
            wrapper.lastData.id = squad.id;
            squads[squad.id] = wrapper;
            squad.soldiers.ForEach(s => Debug.Log(s.callSign));
        });

        connector.StartMission((int)mission.mission_id);  
    }

    public void StopMission()
    {
        connector.StopMission((int)selectedMission.mission_id);
        
        selectedMission = null;
        ClearMarkers();
        queue.Clear();
        operatives.Clear();
        squads.Clear();
    }

    private void ClearMarkers()
    {
        waypointController.operatives.Clear();
        foreach (var entry in operatives)
        {
            Destroy(entry.Value.UiWaypoint);
            Destroy(entry.Value.Operative3d);
        }

        foreach (var entry in squads)
        {
            Destroy(entry.Value.UiWaypoint);
            Destroy(entry.Value.Operative3d);
        }
    }



    private void OnMessageReceived(Message message)
    {
        if (message.GetType() == typeof(BatchMessage))
        {
            handle((BatchMessage)message);
        }
    }

    private void handle(BatchMessage message)
    {
        queue.Add(message.content);
    }

}
