
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class WaypointController : MonoBehaviour
{

    public List<OperativeWrapper> operatives = new();
    public Camera arCamera;
    public GameObject settingsGO;

    private SettingsManager settings;

    [SerializeField] GameObject arSessionOrigin;
    AROcclusionManager arOcclusionManager;
    ARRaycastManager arRaycastManager;

    [SerializeField] private Sprite dotMarkerPrefab;
    [SerializeField] private Sprite arrowMarkerPrefab;

    private float timer = 0.0f;
    private float updateInterval = 1f / 30f; // 30 FPS

    private float clampAt;
    private float displayAt;

    private void Awake()
    {
        arOcclusionManager = arSessionOrigin.GetComponent<AROcclusionManager>();
        arRaycastManager = arSessionOrigin.GetComponent<ARRaycastManager>();
        settings = settingsGO.GetComponent<SettingsManager>();
    }

    private void Start()
    {
        arCamera.useOcclusionCulling = true;
        arOcclusionManager.enabled = true;

        LoadSettings();
        settings.RegisterSettingsChangedCallback(LoadSettings);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            operatives.ForEach(operative => {
                UpdateWaypointPosition(operative);
            });
            timer = 0f;
        }
        
    }

    private void UpdateWaypointPosition(OperativeWrapper operative)
    {

        if(operative.Operative3d == null || operative.UiWaypoint == null)
        {
            return;
        }

        if(operative.lastData.mls < displayAt)
        {
            operative.UiWaypoint.SetActive(false);
            return;
        }

        float dot = Vector3.Dot((operative.Operative3d.transform.position - arCamera.transform.position), arCamera.transform.forward);
        Vector2 pos = arCamera.WorldToScreenPoint(operative.Operative3d.transform.position);

        if (dot < 0)
        {
            if(operative.lastData != null && (operative.lastData.mls >= clampAt))
            {
                UpdateMarker(operative);
            } else
            {
                operative.UiWaypoint.SetActive(false);
            }
        } else
        {
            if (IsObjectVisible(operative.cylinder, operative.OperativeRenderer, pos))
            {
                operative.UiWaypoint.SetActive(false);
            }
            else
            {
                UpdateMarker(operative);
            }
        }
    }

    private bool IsObjectVisible(GameObject target, Renderer renderer, Vector2 pos)
    {
        Bounds bounds = renderer.bounds;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(arCamera);
        bool isInCameraFrustrum = GeometryUtility.TestPlanesAABB(planes, bounds);
        
        if(!isInCameraFrustrum)
        {
            return false;
        }

        List<ARRaycastHit> hits = new();
        if (arRaycastManager.Raycast(pos, hits, TrackableType.AllTypes)) {
            if(hits.Count > 0) {
                if (hits[0].trackable.gameObject == target)
                {
                    return true;
                }   
            }
        }

        return false;
    }

    private void UpdateMarker(OperativeWrapper operative)
    {
        GameObject marker = operative.UiWaypoint;
        Transform target = operative.Operative3d.transform;

        marker.SetActive(true);

        Vector2 pos = arCamera.WorldToScreenPoint(operative.Operative3d.transform.position);
        float dot = Vector3.Dot((target.position - arCamera.transform.position), arCamera.transform.forward);

        float minX = marker.GetComponent<RectTransform>().rect.width / 2;
        float maxX = Screen.width - minX;

        float minY = marker.GetComponent<RectTransform>().rect.height / 2;
        float maxY = Screen.height - minY;

        if (dot < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            } else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // If clamped, change marker to arrow
        if (pos.x == minX || pos.x == maxX || pos.y == minY || pos.y == maxY)
        {
            marker.GetComponentInChildren<Image>().sprite = arrowMarkerPrefab;
            marker.GetComponentInChildren<Image>().transform.rotation = Quaternion.Euler(0, 0, GetRotationZ(pos, minX, maxX, minY, maxY));

        }
        else
        {
            marker.GetComponentInChildren<Image>().transform.Rotate(new(0, 0, 0), Space.Self);
            marker.GetComponentInChildren<Image>().sprite = dotMarkerPrefab;
        }

        operative.UiWaypoint.transform.position = pos;
    }

    private int GetRotationZ(Vector3 pos, float minX, float maxX, float minY, float maxY) 
    {
        var rotateZ = 45;
        // Rotate marker to face side of the screen
        if (pos.x == minX)
        {
            // Left
            rotateZ += 90;
        } else if (pos.x == maxX)
        {
            // Right
            rotateZ += 270;
        } else if (pos.y == minY)
        {
            // Bottom
            rotateZ += 180;
        } else if (pos.y == maxY)
        {
            // Top
            rotateZ += 0;
        } else
        {
            rotateZ += 0;
        }

        return rotateZ;
    }

    void LoadSettings()
    {
        clampAt = PlayerPrefs.GetFloat("clampAt", 0f);
        displayAt = PlayerPrefs.GetFloat("displayAt", 0f);
    }

}
