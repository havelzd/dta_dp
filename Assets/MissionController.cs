using Connection.WS;
using Google.XR.ARCoreExtensions;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using WebSocketUtils;


public class MissionController : MonoBehaviour
{

    [SerializeField] private AppConfig config;
    [SerializeField] private int missionDelay = 300;

    private Channel wsChannel;

    public AREarthManager arEarthManager;
    public GameObject arAnchorManagerGO;
    public GameObject arCameraGO;
    [SerializeField] GameObject settingsManagerGO;

    public Dictionary<string, OperativeWrapper> operatives = new();
    private List<List<SoldierListMessage>> queue = new();

    public GameObject operative2dPrefab;
    public GameObject operative3dPrefab;
    public Canvas canvas;
    Camera arCamera;
    

    private Mission selectedMission;
    private LocationService locationService;
    ARAnchorManager arAnchorManager;
    WaypointController waypointController;
    SettingsManager settingsManager;

    [SerializeField] WSDataMock wsDataMock;

    public UnityEvent<Dictionary<string, OperativeWrapper>> DataReceivedEvent = new();

    private void Awake()
    {
        locationService = LocationService.Instance;
        arAnchorManager = arAnchorManagerGO.GetComponent<ARAnchorManager>();
        waypointController = arCameraGO.GetComponent<WaypointController>();
        arCamera = arCameraGO.GetComponent<Camera>();
        settingsManager = settingsManagerGO.GetComponent<SettingsManager>();
    }

    void Start()
    {
        // Create WS Channel

        if (config.isDebug)
        {
            wsDataMock.OnMessageRec.AddListener(OnMessageReceived);
        }
        else
        {
            wsChannel = WsConnector.CreateChannel(config.serverHost, config.serverPort, WsConnector.MISSION_DATA);
            wsChannel.OnMessageRec.AddListener(OnMessageReceived);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (queue.Count <= 0)
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
                    Debug.Log("New Operative");
                    operative = new OperativeWrapper();
                    waypointController.operatives.Add(operative);
                }
                UpdateOperative(operative, sd, location);
                operative.data.Add(sd);

                operatives[sd.id] = operative;
            });
        });

        DataReceivedEvent.Invoke(operatives);
    }

    private void UpdateOperative(OperativeWrapper operative, OperativeData data, Vector3 _Location)
    {

        if(operative.data != null && operative.data.Count > 0 && operative.data[operative.data.Count - 1].location.Equals(data.location) ) {
            return;
        }

        GameObject marker3d = operative.Operative3d;
        Vector3 pos;
        if(arEarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled) != FeatureSupported.Supported)
        {
            pos = CoordinatesTransformer.ConvertGeoToCartesian(_Location.x, _Location.y, _Location.z, data.location.lat, data.location.lon, data.location.altitude);
            if (operative.Operative3d == null)
            {
                marker3d = Instantiate(operative3dPrefab, pos, new Quaternion(0f, 0f, 0f, 1f));
                marker3d.GetComponentInChildren<Billboard>().arCamera = arCamera;
                operative.OperativeRenderer = marker3d.GetComponentInChildren<MeshRenderer>();
                operative.objectMarker = marker3d.GetComponentInChildren<OperativeMarker>();
                operative.objectMarker.settings = settingsManager;
                operative.cylinder = marker3d.transform.Find("Cylinder").gameObject;
            }
        } else
        {
            ARGeospatialAnchor anchor = null;
            try
            {
                anchor = ARAnchorManagerExtensions.AddAnchor(arAnchorManager, data.location.lat, data.location.lon, data.location.altitude, new Quaternion(0f, 0f, 0f, 1f));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                Vector3 v = CoordinatesTransformer.ConvertGeoToCartesian(_Location.x, _Location.y, _Location.z, data.location.lat, data.location.lon, data.location.altitude);
                if (marker3d == null)
                {
                    if(anchor == null)
                    {
                        marker3d = Instantiate(operative3dPrefab, v, new Quaternion(0f, 0f, 0f, 1f));
                    }else
                    {
                        marker3d = Instantiate(operative3dPrefab, anchor.transform);
                        marker3d.transform.parent = anchor.transform;

                    }
                    operative.OperativeRenderer = marker3d.GetComponentInChildren<MeshRenderer>();
                    marker3d.GetComponentInChildren<Billboard>().arCamera = arCamera;
                    operative.objectMarker = marker3d.GetComponentInChildren<OperativeMarker>();
                    operative.objectMarker.settings = settingsManager;
                    operative.cylinder = marker3d.transform.Find("Cylinder").gameObject;
                } else
                {
                    if(anchor == null)
                    {
                        marker3d.transform.position = v;
                    } else
                    {
                        marker3d.transform.parent = anchor.transform;
                    }
                    
                }

            }
        }

        operative.Operative3d = marker3d;

        var uiMarker = operative.UiWaypoint;
        if(uiMarker == null)
        {
            uiMarker = Instantiate(operative2dPrefab, canvas.transform);
            operative.waypointMarker = uiMarker.GetComponent<OperativeMarker>();
            operative.waypointMarker.settings = settingsManager;
            operative.UiWaypoint = uiMarker;
        }

        operative.objectMarker.Data = data;
        operative.waypointMarker.Data = data;
    }

    public void StartMission(Mission mission)
    {
        if(config.isDebug)
        {
            wsDataMock.StartMission();
        } else
        {
            Debug.Log("Starting mission " + mission.mission_id); 
            selectedMission = mission;
            wsChannel.Start();
            wsChannel.Send(new SubscribeMissionMessage { mission_id = (int)mission.mission_id, delay = missionDelay });
        }
        
    }

    public void StopMission()
    {
        if(config.isDebug)
        {
            wsDataMock.StopMission();
        } else
        {
            wsChannel.Send(new MissionEndMessage { mission_id = (int)selectedMission.mission_id });
            wsChannel.Stop();
        }
        
        selectedMission = null;
        waypointController.operatives.Clear();
        foreach ( var entry in operatives)
        {
            Destroy(entry.Value.UiWaypoint);
            Destroy(entry.Value.Operative3d);
        }
        operatives.Clear();
        queue.Clear();
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

    private void OnDestroy()
    {
        if (wsChannel != null)
        {
            wsChannel.Stop();
        }

    }

}
